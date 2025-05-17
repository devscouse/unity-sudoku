using System;
using UnityEngine;

public class SudokuSolver : MonoBehaviour
{
    private int[,] workspace;
    private SudokuRules rules;

    void Start()
    {
        workspace = new int[9, 9];
        rules = new SudokuRules(workspace);
    }

    public void CopyBoardToWorkspace(int[,] values)
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                workspace[x, y] = values[x, y];
            }
        }
    }


    public bool Solve()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (workspace[x, y] != 0) { continue; }
                for (int value = 1; value < 10; value++)
                {
                    if (rules.PlacementIsValid(x, y, value))
                    {
                        Debug.Log($"Trying {value} in position ({x}, {y})");
                        workspace[x, y] = value;
                        if (Solve())
                        {
                            return true;
                        }
                        workspace[x, y] = 0;

                    }
                }
                return false;
            }
        }
        return false;
    }

    public int[,] GetSolution()
    {
        return workspace;
    }
}
