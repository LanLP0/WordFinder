namespace WordFinder;

[Flags]
public enum ExcludeDirection
{
    Right = 1,
    Left = 1<<2,
    Up = 1<<3,
    Down = 1<<4,
    DownLeft = 1<<5,
    DownRight = 1<<6,
    UpLeft = 1<<7,
    UpRight = 1<<8
}