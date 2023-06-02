public struct Position
{
    public int Row { get; }
    public int Col { get; }

    public Position(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static Position operator +(Position a, Position b) =>
        new Position(a.Row + b.Row, a.Col + b.Col);

    public static bool operator ==(Position a, Position b) =>
        a.Row == b.Row && a.Col == b.Col;

    public static bool operator !=(Position a, Position b) =>
        !(a == b);
}
