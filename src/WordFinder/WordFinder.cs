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

    public IReadOnlyList<FindResult> FindAll(ReadOnlySpan<char> chars, int x, int y, bool diagonal)
    {
        if (x <= 1 || y <= 1 || !diagonal)
            return FindStraight(chars);

        return FindStraight(chars).Concat(FindDiagonal(chars, x, y)).ToList().AsReadOnly();
    }

    private IReadOnlyList<FindResult> FindStraight(ReadOnlySpan<char> chars)
    {
        var results = new List<FindResult>();

        foreach (var word in _words)
        {
            var index = chars.IndexOf(word);
            if (index is -1)
                continue;

            results.Add(new FindResult(index, Direction.Right, word));
        }

        return results.AsReadOnly();
    }

    private IReadOnlyList<FindResult> FindDiagonal(ReadOnlySpan<char> chars, int dimX, int dimY)
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

                var (x, y) = WordFinderHelper.IndexToPos(i, dimX);

                if (CheckDownLeft(chars, dimX, dimY, x, y, 0, word))
                {
                    results.Add(new FindResult(i, Direction.DownLeft, word));
                    break;
                }

                if (!CheckDownRight(chars, dimX, dimY, x, y, 0, word))
                    continue;

                results.Add(new FindResult(i, Direction.DownRight, word));
                break;
            }
        }

        return results;
    }

    private bool CheckDownLeft(ReadOnlySpan<char> chars, int dimX, int dimY, int x, int y, int i, string word)
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

        return CheckDownLeft(chars, dimX, dimY, x, y, i, word);
    }

    private bool CheckDownRight(ReadOnlySpan<char> chars, int dimX, int dimY, int x, int y, int i, string word)
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

        return CheckDownRight(chars, dimX, dimY, x, y, i, word);
    }
}