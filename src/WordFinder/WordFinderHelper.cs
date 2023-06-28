using System.Diagnostics.CodeAnalysis;
using Spectre.Console;

namespace WordFinder;

public static class WordFinderHelper
{
    public static bool PosInbound(int x, int y, int dimX, int dimY)
    {
        return x >= 0 && x < dimX && y >= 0 && y < dimY;
    }

    public static int PosToIndex(int width, int x, int y)
    {
        return y * width + x;
    }

    public static (int x, int y) IndexToPos(int width, int index)
    {
        return (index % width, index / width);
    }
    
    public static bool TryParseColorFromString(string colorString, [NotNullWhen(true)] out Color? color,
        [NotNullWhen(false)] out string? error)
    {
        if (colorString.Contains(' '))
        {
            color = default;
            error = "Color cannot contains whitespace(s)";
            return false;
        }

        if (!Style.TryParse(colorString, out var style))
        {
            color = default;
            error = $"Could not find color `{colorString}`";
            return false;
        }

        color = style!.Foreground;
        error = null;
        return true;
    }
}