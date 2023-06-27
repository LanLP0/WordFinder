using System.Diagnostics;
using Spectre.Console;
using Spectre.Console.Cli;

namespace WordFinder;

public sealed partial class WordFinderApp : Command<WordFinderApp.WordFinderConfig>
{
    private static readonly Color[] Colors = {
        Color.Default,
        Color.Yellow,
        Color.Aqua,
        Color.Blue,
        Color.Lime,
        Color.Red,
        Color.Olive,
        Color.Fuchsia,
        Color.Grey,
        Color.Maroon,
        Color.Navy
    };
    
    public override int Execute(CommandContext context, WordFinderConfig settings)
    {
        try
        {
            AnsiConsole.MarkupLine("Press [yellow]Ctrl-C[/] to stop");
            
            if (settings.Verbose)
                AnsiConsole.WriteLine("Commandline: {0}", Environment.CommandLine);
            
            // Setup
            
            settings.WordListPath = Path.GetFullPath(settings.WordListPath);
            AnsiConsole.MarkupLine("Word list path: {0}", settings.WordListPath);

            if (settings.NoWrap)
                settings.Wrap = false;
            
            // Characters

            var characters = settings.Characters;
            if (characters is null)
                characters = AnsiConsole.Ask<string>("Characters: ");
            else if (settings.Verbose)
                AnsiConsole.WriteLine("Characters: {0}", characters);

            characters = characters.ToLowerInvariant();
            
            if (settings.Verbose)
                AnsiConsole.WriteLine("Characters length: {0}", characters.Length);
            
            // Dimension

            var dimX = -1;
            var dimY = -1;
            if (settings.Size == null)
            {
                var prompt = new TextPrompt<string?>("[aqua][[OPTIONAL]][/] Character box size? ")
                    .AllowEmpty().HideDefaultValue().DefaultValue(null);
                settings.Size = prompt.Show(AnsiConsole.Console);
            }
            
            if (settings.Size is not null)
            {
                var split = settings.Size.Split('x');
                if (split.Length is not 2)
                {
                    AnsiConsole.MarkupLine("[red]Error:[/] Invalid size format");
                    return -1;
                }

                if (!int.TryParse(split[0], out dimX))
                {
                    AnsiConsole.MarkupLine("[red]Error:[/] Invalid size for width: {0}", split[0]);
                    return -1;
                }

                if (dimX <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Error:[/] Width must be at least 1");
                    return -1;
                }

                if (!int.TryParse(split[1], out dimY))
                {
                    AnsiConsole.MarkupLine("[red]Error:[/] Invalid size for height: {0}", split[1]);
                    return -1;
                }

                if (dimY <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Error:[/] Height must be at least 1");
                    return -1;
                }
            }

            if (dimX is -1) // No value provided
            {
                dimX = characters.Length;
                dimY = 1;
            }
            else if (characters.Length != dimX * dimY)
            {
                AnsiConsole.MarkupLine("[red]Error:[/] Invalid size");
                return -1;
            }
            
            if (settings.Verbose) AnsiConsole.WriteLine("Size: Height {0} x Width {1}", dimX, dimY);
            
            // Exclusion

            var exclusion = (ExcludeDirection)0;
            if (settings.Exclusion is not null)
            {
                var exclusions = settings.Exclusion.Split(',');

                foreach (var ex in exclusions)
                {
                    if (!Enum.TryParse(ex, true, out ExcludeDirection exTmp))
                    {
                        AnsiConsole.MarkupLine("[red]Error:[/] Invalid direction {0}", ex);
                        AnsiConsole.WriteLine("Options are: Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight");
                        return -1;
                    }

                    exclusion |= exTmp;
                }
            }

            if (settings.NoDiag)
                exclusion |= ExcludeDirection.DownLeft | ExcludeDirection.DownRight |
                           ExcludeDirection.UpLeft | ExcludeDirection.UpRight;
            if (settings.NoBack)
                exclusion |= ExcludeDirection.Left | ExcludeDirection.UpLeft |
                           ExcludeDirection.Up | ExcludeDirection.UpRight;
            
            // Main logic

            var progressBar = AnsiConsole.Progress()
                .Columns(new TaskDescriptionColumn(), new ProgressBarColumn(), new PercentageColumn(),
                    new RemainingTimeColumn(), new SpinnerColumn());
            
            var finder = new WordFinder(settings.WordListPath, settings.MinLetter, progressBar);
            if (settings.Verbose)
                AnsiConsole.WriteLine("Parsed {0} word(s)", finder.Words.Count);

            var resultEnum = finder.Search(characters, dimX, dimY, settings.Wrap, exclusion)
                .OrderBy(a => a.Word);

            var results = settings.Single ? RemoveDuplicates(resultEnum).ToArray() : resultEnum.ToArray();
            
            if (results.Length is 0)
            {
                AnsiConsole.WriteLine("No word(s) found");
                return 0;
            }

            if (settings.Verbose) AnsiConsole.WriteLine("{0} Result(s)", results.Length);

            var heatmap = BuildCharHeatmap(characters, results, dimX, settings);

            DisplayHeatmap(characters, heatmap, dimX, dimY, settings);

            var wordTable = new Table();
            wordTable.AddColumn("Word").AddColumn("Position").AddColumn("Direction");
            wordTable.Title("WORDS", new Style(Color.Yellow));
            foreach (var result in results)
            {
                var (x, y) = WordFinderHelper.IndexToPos(dimX, result.Pos);
                wordTable.AddRow(result.Word, $"{y + 1}:{x + 1}", result.Direction.ToString());
            }

            if (!settings.SingleColor)
            {
                var maxLevel = heatmap.Max();
                var colorTable = new Table().Title("COLORS", new Style(Color.Yellow))
                    .AddColumn("Color").AddColumn("Level");

                for (var i = 1; i < Colors.Length && i <= maxLevel; i++)
                {
                    var color = Colors[i];
                    colorTable.AddRow($"[{color}]{color}[/]",
                        i != Colors.Length - 1 ? i.ToString() : $"{i}+");
                }
                
                var columns = new Columns(wordTable, colorTable);
                columns.Expand = false;
                AnsiConsole.Write(columns);
                return 0;
            }
            
            AnsiConsole.Write(wordTable);
            return 0;
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine("[red]An error occurred, please try again[/]\n{0}", e.Message);
#if DEBUG
            AnsiConsole.WriteLine(e.StackTrace ?? "No stack trace");
#endif
            return 1;
        }
    }

    private void DisplayHeatmap(ReadOnlySpan<char> characters, int[] heatmap, int x, int y, WordFinderConfig settings)
    {
        var paragraph = new Paragraph();

        for (var i = 0; i < y; i++)
        {
            for (var j = 0; j < x; j++)
            {
                var index = i * x + j;

                Color color;
                if (settings.SingleColor)
                    color = heatmap[index] > 0 ? Colors[1] : Colors[0];
                else
                    color = GetColorForHeatmapLevel(heatmap[index]);
                
                paragraph.Append(characters[index].ToString(), new Style(color));
                paragraph.Append(" ");
            }

            paragraph.Append("\n");
        }

        AnsiConsole.Write(paragraph);
    }

    private Color GetColorForHeatmapLevel(int level)
    {
        if (level >= Colors.Length)
            return Colors[Colors.Length - 1];

        return Colors[level];
    }

    private int[] BuildCharHeatmap(ReadOnlySpan<char> characters, IReadOnlyList<FindResult> results, int width,
        WordFinderConfig settings)
    {
        var map = new int[characters.Length];

        foreach (var result in results)
        {
            var (x, y) = WordFinderHelper.IndexToPos(width, result.Pos);

            var (adjX, adjY) = result.Direction switch
            {
                Direction.Right => (1, 0),
                Direction.Left => (-1, 0),
                Direction.Up => (0, -1),
                Direction.Down => (0, 1),
                Direction.DownLeft => (-1, 1),
                Direction.DownRight => (1, 1),
                Direction.UpLeft => (-1, -1),
                Direction.UpRight => (1, -1),
                _ => throw new UnreachableException()
            };

            var count = result.Word.Length;
            if (settings.Minimal)
                count = 1;

            for (var i = 0; i < count; i++)
            {
                var index = WordFinderHelper.PosToIndex(width, x, y);
                map[index]++;

                if (adjY is 0 && settings.Wrap) // Special case
                {
                    index += adjX;
                    (x, y) = WordFinderHelper.IndexToPos(width, index);
                }
                else
                {
                    x += adjX;
                    y += adjY;
                }
            }
        }

        return map;
    }

    private IEnumerable<FindResult> RemoveDuplicates(IEnumerable<FindResult> results)
    {
        var lastWord = string.Empty; // This is invalid

        foreach (var result in results)
        {
            if (result.Word == lastWord)
                continue;

            lastWord = result.Word;
            yield return result;
        }
    }
}