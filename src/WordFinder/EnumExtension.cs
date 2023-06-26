namespace WordFinder;

public static class EnumExtension
{
    public static bool HasFlagFast(this ExcludeDirection exclusion, ExcludeDirection direction)
    {
        return (exclusion & direction) != 0;
    }
}