using System.Collections;
using UnityEngine;

public class SudokuBacktrackSolver
{
    public int nSolutions;
    public int[,] solution;
    public int[,] workspace;
    private SudokuRules rules;

    public SudokuBacktrackSolver(int[,] values)
    {
        nSolutions = 0;
        solution = new int[9, 9];
        workspace = new int[9, 9];
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                workspace[x, y] = values[x, y];
            }
        }
        rules = new SudokuRules(workspace);
    }

    void SaveWorkspaceAsSolution()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                solution[x, y] = workspace[x, y];
            }
        }

    }


    public IEnumerator Solve()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (workspace[x, y] != 0) { continue; }
                for (int val = 1; val < 10; val++)
                {
                    if (!rules.PlacementIsValid(x, y, val)) { continue; }

                    // Try value in this position
                    workspace[x, y] = val;

                    yield return Solve();

                    // Check if the maze was solved by placing this value
                    // if it was increment our solution count by 1
                    if (rules.IsSolved())
                    {
                        nSolutions += 1;
                        if (nSolutions > 1)
                        {
                            Debug.Log("2 solutions found, quitting early");
                            yield break;
                        }
                        SaveWorkspaceAsSolution();
                    }
                    workspace[x, y] = 0;
                }
                yield break;
            }
        }
    }

}
