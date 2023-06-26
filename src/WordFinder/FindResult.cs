namespace WordFinder;

public sealed class FindResult
{
    public int Pos { get; init; }
    public Direction Direction { get; init; }
    public string Word { get; init; }

    public FindResult(int pos, Direction direction, string word)
    {
        Pos = pos;
        Direction = direction;
        Word = word;
    }
}