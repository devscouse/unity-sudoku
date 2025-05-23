using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SudokuCspSolver
{
    public int[,] workspace;
    private SudokuValues workspaceValues;
    private SudokuRules workspaceRules;
    private bool finished;
    public SudokuConstraints constraints;
    public int nSolutions;

    public SudokuCspSolver(int[,] values)
    {
        nSolutions = 0;
        workspace = new int[9, 9];
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                workspace[x, y] = values[x, y];
            }
        }
        workspaceValues = new SudokuValues(workspace);
        workspaceRules = new SudokuRules(workspace);
        constraints = new SudokuConstraints(workspaceValues);
    }


    public IEnumerator Solve()
    {
        if (workspaceRules.IsSolved())
        {
            Debug.Log("Solution Found!");
            nSolutions++;
            if (nSolutions > 1)
            {
                Debug.Log("2 solutions found, quitting early");
                finished = true;
                yield break;
            }
        }
        finished = false;
        if (!constraints.AllEmptyPositionsHavePlaceableValues() || workspaceRules.IsFilled()) { yield break; }

        Pos pos = constraints.GetEmptyPosWithLeastConstraints();
        List<int> placeableValues = constraints.GetDomainValues(pos.X, pos.Y);
        Debug.Log($"{pos.X}, {pos.Y} has the smallest domain: {String.Join(" ", placeableValues)}");

        foreach (int value in placeableValues)
        {
            if (!workspaceRules.PlacementIsValid(pos.X, pos.Y, value))
            {
                Debug.Log($"Somehow an invalid value is trying to be placed {value} at {pos.X}, {pos.Y}");
                continue;
            }
            workspace[pos.X, pos.Y] = value;
            constraints.SetValue(pos.X, pos.Y, value);
            yield return Solve();
            if (finished) { yield break; }
            constraints.WipeValue(pos.X, pos.Y);
            workspace[pos.X, pos.Y] = 0;
        }
    }
}
