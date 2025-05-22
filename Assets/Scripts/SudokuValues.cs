class SudokuValues
{
    public readonly int[,] values;

    public SudokuValues(int[,] values)
    {
        this.values = values;
    }

    public int[] GetValuesFromPositions(Pos[] positions)
    {
        int[] posValues = new int[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            posValues[i] = values[positions[i].X, positions[i].Y];
        }
        return posValues;
    }

    public Pos[] GetRowPositions(int y)
    {
        Pos[] rowPositions = new Pos[9];
        for (int i = 0; i < 9; i++)
        {
            rowPositions[i] = new Pos(i, y);
        }
        return rowPositions;
    }

    public int[] GetRow(int y)
    {
        return GetValuesFromPositions(GetRowPositions(y));
    }

    public Pos[] GetColPositions(int x)
    {
        Pos[] colPositions = new Pos[9];
        for (int i = 0; i < 9; i++)
        {
            colPositions[i] = new Pos(x, i);
        }
        return colPositions;
    }

    public int[] GetCol(int x)
    {
        return GetValuesFromPositions(GetColPositions(x));
    }

    public Pos[] GetSquarePositions(int x, int y)
    {
        Pos[] squarePositions = new Pos[9];
        int squareX = (x / 3) * 3;
        int squareY = (y / 3) * 3;
        int i = 0;
        for (int dx = 0; dx < 3; dx++)
        {
            for (int dy = 0; dy < 3; dy++)
            {
                squarePositions[i++] = new Pos(squareX + dx, squareY + dy);
            }
        }
        return squarePositions;
    }

    public int[] GetSquare(int x, int y)
    {
        return GetValuesFromPositions(GetSquarePositions(x, y));
    }

    public int GetValue(int x, int y) { return values[x, y]; }
    public void SetValue(int x, int y, int value) { values[x, y] = value; }
}
