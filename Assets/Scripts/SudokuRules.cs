class SudokuRules
{
    private readonly SudokuValues values;

    public SudokuRules(int[,] board)
    {
        values = new SudokuValues(board);
    }

    public bool SetIsValid(int[] set)
    {
        for (int i = 0; i < 8; i++)
        {
            int val1 = set[i];
            if (val1 == 0) { continue; }
            for (int j = i + 1; j < 9; j++)
            {
                int val2 = set[j];
                if (val2 == 0) { continue; }
                if (val1 == val2) { return false; }
            }
        }
        return true;
    }

    public bool RowIsValid(int y)
    {
        return SetIsValid(values.GetRow(y));
    }

    public bool ColIsValid(int x)
    {
        return SetIsValid(values.GetCol(x));
    }

    public bool SquareIsValid(int x, int y)
    {
        return SetIsValid(values.GetSquare(x, y));
    }

    public bool PlacementIsValidInSet(int[] set, int value)
    {
        if (value == 0)
            return true;

        for (int i = 0; i < 9; i++)
        {
            if (set[i] == value)
                return false;
        }
        return true;
    }

    public bool PlacementIsValidInRow(int x, int y, int value)
    {
        return PlacementIsValidInSet(values.GetRow(y), value);
    }

    public bool PlacementIsValidInCol(int x, int y, int value)
    {
        return PlacementIsValidInSet(values.GetCol(x), value);
    }

    public bool PlacementIsValidInSquare(int x, int y, int value)
    {
        return PlacementIsValidInSet(values.GetSquare(x, y), value);
    }

    public bool PlacementIsValid(int x, int y, int value)
    {
        if (value == 0)
            return true;

        return (
            PlacementIsValidInCol(x, y, value)
            && PlacementIsValidInRow(x, y, value)
            && PlacementIsValidInSquare(x, y, value));
    }

    public bool IsFilled()
    {
        foreach (int value in values.values)
        {
            if (value == 0) { return false; }
        }
        return true;
    }

    public bool IsValid()
    {
        for (int i = 0; i < 9; i++)
        {
            if (!RowIsValid(i)) { return false; }
            if (!ColIsValid(i)) { return false; }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (!SquareIsValid(i * 3, j * 3)) { return false; }
            }
        }
        return true;
    }

    public bool IsSolved() { return IsFilled() && IsValid(); }
}
