namespace WordFinder;

public static class StringExtension
{
    public static bool IsAsciiLetter(this string s)
    {
        foreach (var c in s)
            if (!char.IsAsciiLetter(c))
                return false;

        return true;
    }
}