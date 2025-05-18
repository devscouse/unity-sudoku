class SudokuRules
{
    private readonly int[,] board;

    public SudokuRules(int[,] board)
    {
        this.board = board;
    }

    public bool SetIsValid(int[] values)
    {
        for (int i = 0; i < 8; i++)
        {
            int val1 = values[i];
            if (val1 == 0) { continue; }
            for (int j = i + 1; j < 9; j++)
            {
                int val2 = values[j];
                if (val2 == 0) { continue; }
                if (val1 == val2) { return false; }
            }
        }
        return true;
    }

    public bool RowIsValid(int y)
    {
        int[] values = new int[9];
        for (int i = 0; i < 9; i++) { values[i] = board[i, y]; }
        return SetIsValid(values);
    }

    public bool ColIsValid(int x)
    {
        int[] values = new int[9];
        for (int i = 0; i < 9; i++) { values[i] = board[x, i]; }
        return SetIsValid(values);
    }

    public bool SquareIsValid(int x, int y)
    {
        int[] values = new int[9];
        int gridX = (x / 3) * 3;
        int gridY = (y / 3) * 3;
        int i = 0;
        for (int dx = 0; dx < 3; dx++)
        {
            for (int dy = 0; dy < 3; dy++)
            {
                values[i++] = board[gridX + dx, gridY + dy];
            }
        }
        return SetIsValid(values);
    }

    public bool PlacementIsValidInRow(int x, int y, int value)
    {
        if (value == 0)
            return true;

        for (int x2 = 0; x2 < 9; x2++)
        {
            if (x == x2)
                continue;

            if (value == board[x2, y])
                return false;
        }
        return true;
    }

    public bool PlacementIsValidInCol(int x, int y, int value)
    {
        if (value == 0)
            return true;

        for (int y2 = 0; y2 < 9; y2++)
        {
            if (y == y2)
                continue;

            if (value == board[x, y2])
                return false;
        }
        return true;
    }

    public bool PlacementIsValidInSquare(int x, int y, int value)
    {
        if (value == 0)
            return true;

        int gridX = (x / 3) * 3;
        int gridY = (y / 3) * 3;
        for (int dx = 0; dx < 3; dx++)
        {
            for (int dy = 0; dy < 3; dy++)
            {
                int x2 = gridX + dx;
                int y2 = gridY + dy;
                if (x2 == x && y2 == y)
                    continue;

                if (value == board[x2, y2])
                    return false;
            }
        }
        return true;
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
}
