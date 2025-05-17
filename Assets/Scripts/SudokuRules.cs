class SudokuRules
{
    private readonly int[,] board;

    public SudokuRules(int[,] board)
    {
        this.board = board;
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
        int gridY = (x / 3) * 3;
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
