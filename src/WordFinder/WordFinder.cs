using Spectre.Console;

namespace WordFinder;

public sealed class WordFinder
{
    private string[] _words;

    public WordFinder(string pathToWordFile, int minLetter, Progress progressBar)
    {
        progressBar.Start(ctx =>
        {
            var words = Parse(pathToWordFile);

            var task = ctx.AddTask("Parse word file", true, words.Length);
            VerifyAndSet(words, minLetter, task);
        });
    }

    public IReadOnlyCollection<string> Words => _words;

    private string[] Parse(string pathToWordFile)
    {
        using var fs = File.OpenText(pathToWordFile);
        var words = fs.ReadToEnd().ToLowerInvariant();

        if (words.Length is 0)
            throw new Exception("The word file cannot be empty");

        return words.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private void VerifyAndSet(string[] words, int minLetter, ProgressTask task)
    {
        var tmp = new HashSet<string>();

        foreach (var word in words)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                task.Increment(1);
                continue;
            }

            if (word.Length < minLetter)
            {
                task.Increment(1);
                continue;
            }

            tmp.Add(word);
            task.Increment(1);
        }

        _words = tmp.ToArray();
    }

    public IReadOnlyList<FindResult> Search(ReadOnlySpan<char> chars, int dimX, int dimY, bool wrap,
        ExcludeDirection exclusion)
    {
        var results = new List<FindResult>();

        foreach (var word in _words)
        {
            var firstChar = word[0];
            for (var i = 0; i < chars.Length; i++)
            {
                var c = chars[i];
                if (c != firstChar)
                    continue;

                var (x, y) = WordFinderHelper.IndexToPos(dimX, i);
                
                if (!exclusion.HasFlagFast(ExcludeDirection.Up) && CheckUp(chars, dimX, x, y, 0, word))
                    results.Add(new FindResult(i, Direction.Up, word));

                if (!exclusion.HasFlagFast(ExcludeDirection.Down) && CheckDown(chars, dimX, dimY, x, y, 0, word))
                    results.Add(new FindResult(i, Direction.Down, word));
                    
                if (!exclusion.HasFlagFast(ExcludeDirection.Left) && CheckLeft(chars, dimX, i, 0, word, wrap))
                    results.Add(new FindResult(i, Direction.Left, word));
                
                if (!exclusion.HasFlagFast(ExcludeDirection.Right) && CheckRight(chars, dimX, i, 0, word, wrap))
                    results.Add(new FindResult(i, Direction.Right, word));
                
                if (!exclusion.HasFlagFast(ExcludeDirection.UpLeft) && CheckUpLeft(chars, dimX, dimY, x, y, 0, word))
                    results.Add(new FindResult(i, Direction.UpLeft, word));

                if (!exclusion.HasFlagFast(ExcludeDirection.UpRight) && CheckUpRight(chars, dimX, dimY, x, y, 0, word))
                    results.Add(new FindResult(i, Direction.UpRight, word));

                if (!exclusion.HasFlagFast(ExcludeDirection.DownLeft) && CheckDownLeft(chars, dimX, dimY, x, y, 0, word))
                    results.Add(new FindResult(i, Direction.DownLeft, word));

                if (!exclusion.HasFlagFast(ExcludeDirection.DownRight) && CheckDownRight(chars, dimX, dimY, x, y, 0, word))
                    results.Add(new FindResult(i, Direction.DownRight, word));
            }
        }
        
        return results.AsReadOnly();
    }

    private bool CheckUp(ReadOnlySpan<char> chars, int dimX, int x, int y, int i, string word)
    {
        for (;;)
        {
            if (i >= word.Length)
                return true;

            var index = WordFinderHelper.PosToIndex(dimX, x, y);
            if (chars[index] != word[i])
                return false;

            // Move to next value
            y--;
            i++;
            if (y < 0)
                return i >= word.Length;
        }
    }

    private bool CheckDown(ReadOnlySpan<char> chars, int dimX, int dimY, int x, int y, int i, string word)
    {
        for (;;)
        {
            if (i >= word.Length)
                return true;

            var index = WordFinderHelper.PosToIndex(dimX, x, y);
            if (chars[index] != word[i])
                return false;

            // Move to next value
            y++;
            i++;
            if (y >= dimY)
                return i >= word.Length;
        }
    }

    private bool CheckLeft(ReadOnlySpan<char> chars, int width, int index, int i, string word, bool wrap)
    {
        for (;;)
        {
            if (i >= word.Length)
                return true;

            if (chars[index] != word[i])
                return false;

            var (_, y) = WordFinderHelper.IndexToPos(width, index);

            // Move to next value
            index--;
            i++;
            if (WordFinderHelper.IndexToPos(width, index).y != y && !wrap) return i >= word.Length;
            if (index <= 0)
                return i >= word.Length;
        }
    }

    private bool CheckRight(ReadOnlySpan<char> chars, int width, int index, int i, string word, bool wrap)
    {
        for (;;)
        {
            if (i >= word.Length)
                return true;

            if (chars[index] != word[i])
                return false;

            var (_, y) = WordFinderHelper.IndexToPos(width, index);

            // Move to next value
            index++;
            i++;
            if (WordFinderHelper.IndexToPos(width, index).y != y && !wrap)
                return i >= word.Length;
            if (index >= chars.Length)
                return i >= word.Length;
        }
    }

    private bool CheckUpLeft(ReadOnlySpan<char> chars, int dimX, int dimY, int x, int y, int i, string word)
    {
        for (;;)
        {
            if (i >= word.Length)
                return true;

            var index = WordFinderHelper.PosToIndex(dimX, x, y);
            if (chars[index] != word[i])
                return false;

            // Move to next value
            x--;
            y--;
            i++;
            if (!WordFinderHelper.PosInbound(x, y, dimX, dimY))
                return i >= word.Length;
        }
    }

    private bool CheckUpRight(ReadOnlySpan<char> chars, int dimX, int dimY, int x, int y, int i, string word)
    {
        for (;;)
        {
            if (i >= word.Length)
                return true;

            var index = WordFinderHelper.PosToIndex(dimX, x, y);
            if (chars[index] != word[i])
                return false;

            // Move to next value
            x++;
            y--;
            i++;
            if (!WordFinderHelper.PosInbound(x, y, dimX, dimY))
                return i >= word.Length;
        }
    }

    private bool CheckDownLeft(ReadOnlySpan<char> chars, int dimX, int dimY, int x, int y, int i, string word)
    {
        for (;;)
        {
            if (i >= word.Length)
                return true;

            var index = WordFinderHelper.PosToIndex(dimX, x, y);
            if (chars[index] != word[i])
                return false;

            // Move to next value
            x--;
            y++;
            i++;
            if (!WordFinderHelper.PosInbound(x, y, dimX, dimY))
                return i >= word.Length;
        }
    }

    private bool CheckDownRight(ReadOnlySpan<char> chars, int dimX, int dimY, int x, int y, int i, string word)
    {
        for (;;)
        {
            if (i >= word.Length)
                return true;

            var index = WordFinderHelper.PosToIndex(dimX, x, y);
            if (chars[index] != word[i])
                return false;

            // Move to next value
            x++;
            y++;
            i++;
            if (!WordFinderHelper.PosInbound(x, y, dimX, dimY))
                return i >= word.Length;
        }
    }
}