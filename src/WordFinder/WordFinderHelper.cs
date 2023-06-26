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

    public static (int x, int y) IndexToPos(int index, int width)
    {
        return (index % width, index / width);
    }
}