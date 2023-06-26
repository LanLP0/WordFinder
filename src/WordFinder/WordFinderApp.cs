using System.ComponentModel;
using System.Diagnostics;
using Spectre.Console;
using Spectre.Console.Cli;

namespace WordFinder;

public sealed class WordFinderApp : Command<WordFinderApp.WordFinderConfig>
{
    public override int Execute(CommandContext context, WordFinderConfig settings)
    {
        try
        {
            AnsiConsole.MarkupLine("Press [yellow]Ctrl-C[/] to stop");
            settings.WordListPath = Path.GetFullPath(settings.WordListPath);
            AnsiConsole.MarkupLine("Word list path: {0}", settings.WordListPath);
            AnsiConsole.Write("Commandline: {0}", Environment.CommandLine);
            var progressBar = AnsiConsole.Progress()
                .Columns(new TaskDescriptionColumn(), new ProgressBarColumn(), new PercentageColumn(),
                    new RemainingTimeColumn(), new SpinnerColumn());
            var finder = new WordFinder(settings.WordListPath, settings.MinLetter, progressBar);
            if (settings.Verbose) AnsiConsole.WriteLine("Parsed {0} word(s)", finder.Words.Count);

            var x = -1;
            var y = -1;
            if (settings.Size is not null)
            {
                var split = settings.Size.Split('x');
                if (split.Length is not 2)
                {
                    AnsiConsole.MarkupLine("[red]Error:[/] Invalid size format");
                    return -1;
                }

                if (!int.TryParse(split[0], out x))
                {
                    AnsiConsole.MarkupLine("[red]Error:[/] Invalid size for width: {0}", split[0]);
                    return -1;
                }

                if (x <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Error:[/] Width must be at least 1");
                    return -1;
                }

                if (!int.TryParse(split[0], out y))
                {
                    AnsiConsole.MarkupLine("[red]Error:[/] Invalid size for height: {0}", split[1]);
                    return -1;
                }

                if (y <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Error:[/] Height must be at least 1");
                    return -1;
                }
            }

            if (settings.Verbose) AnsiConsole.WriteLine("Size: Height {0} x Width {1}", x, y);

            var characters = settings.Characters;
            if (characters is null)
                characters = AnsiConsole.Ask<string>("Characters: ");
            else if (settings.Verbose) AnsiConsole.WriteLine("Characters: {0}", characters);

            characters = characters.ToLowerInvariant();

            if (x is -1) // No value provided
            {
                x = characters.Length;
                y = 1;
            }

            var results = finder.FindAll(characters, x, y, settings.DiagonalSearch).OrderBy(a => a.Word).ToArray();

            if (results.Length is 0)
            {
                AnsiConsole.WriteLine("No word(s) found");
                return 0;
            }

            if (settings.Verbose) AnsiConsole.WriteLine("{0} Result(s)", results.Length);

            var heatmap = BuildCharHeatmap(characters, results, x, settings);

            DisplayHeatmap(characters, heatmap, x, y);

            var colorTable = new Table().Title("[yellow]COLORS[/]")
                .AddColumn("Color").AddColumn("Level")
                .AddRow("[aqua]Aqua[/]", "1")
                .AddRow("[blue]Blue[/]", "2")
                .AddRow("[green]Green[/]", "3")
                .AddRow("[yellow]Yellow[/]", "4")
                .AddRow("[red]Red[/]", "5")
                .AddRow("[purple]Purple[/]", "6+");
            AnsiConsole.Write(colorTable);

            var wordTable = new Table();
            wordTable.AddColumn("Word").AddColumn("Position").AddColumn("Direction");
            wordTable.Title("WORDS", new Style(Color.Yellow));
            foreach (var result in results)
                wordTable.AddRow(result.Word, (result.Pos + 1).ToString(), result.Direction.ToString());
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

    private void DisplayHeatmap(ReadOnlySpan<char> characters, int[] heatmap, int x, int y)
    {
        var paragraph = new Paragraph();

        for (var i = 0; i < y; i++)
        {
            for (var j = 0; j < x; j++)
            {
                var index = i * x + j;

                var color = GetColorForHeatmapLevel(heatmap[index]);

                paragraph.Append(characters[index].ToString(), new Style(color));
            }

            paragraph.Append("\n");
        }

        AnsiConsole.Write(paragraph);
    }

    private Color GetColorForHeatmapLevel(int level)
    {
        if (level >= 6)
            return Color.Purple;

        return level switch
        {
            0 => Color.Default,
            1 => Color.Aqua,
            2 => Color.Blue,
            3 => Color.Green,
            4 => Color.Yellow,
            5 => Color.Red,
            _ => throw new UnreachableException()
        };
    }

    private int[] BuildCharHeatmap(ReadOnlySpan<char> characters, IReadOnlyList<FindResult> results, int dimX,
        WordFinderConfig settings)
    {
        var map = new int[characters.Length];

        foreach (var result in results)
        {
            var (x, y) = WordFinderHelper.IndexToPos(result.Pos, dimX);

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
                var index = WordFinderHelper.PosToIndex(dimX, x, y);
                map[index]++;

                x += adjX;
                y += adjY;
            }
        }

        return map;
    }

    public sealed class WordFinderConfig : CommandSettings
    {
        [Description("Path to word list file (default: words.txt)")]
        [CommandArgument(2, "[wordListPath]")]
        [DefaultValue("words.txt")]
        public string WordListPath { get; set; }

        [Description("The characters")]
        [CommandArgument(0, "[characters]")]
        public string? Characters { get; init; }

        [Description("Size of character box (WidthxHeight)")]
        [CommandArgument(1, "[size]")]
        public string? Size { get; init; }

        [Description("Search for words along diagonals")]
        [CommandOption("-d|--diagonal")]
        [DefaultValue(false)]
        public bool DiagonalSearch { get; init; }

        [CommandOption("-V|--verbose")]
        [DefaultValue(false)]
        public bool Verbose { get; init; }

        [Description("Minimum amount of letter a word needs to have in order to be parsed")]
        [CommandOption("-m|--min-letter")]
        [DefaultValue(2)]
        public int MinLetter { get; init; }

        [Description("Only show the first character on the result")]
        [CommandOption("-M|--minimal")]
        [DefaultValue(false)]
        public bool Minimal { get; init; }

        public override ValidationResult Validate()
        {
            if (!File.Exists(WordListPath))
                return ValidationResult.Error("Word file not found");

            return ValidationResult.Success();
        }
    }
}