using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class SudokuGenerator
{
    public int[,] solution;
    public int[,] problem;
    private bool finished;
    private List<int> requiredClueCache;
    private SudokuRules solutionRules;
    private SudokuCspSolver solver;

    public SudokuGenerator()
    {
        solution = new int[9, 9];
        problem = new int[9, 9];
        solutionRules = new SudokuRules(solution);
        requiredClueCache = new List<int>();
    }

    void CopySolutionToProblem()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                problem[x, y] = solution[x, y];
            }
        }
    }

    public IEnumerator GenerateSolution()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (solution[x, y] != 0) { continue; }
                List<int> numbers = Enumerable.Range(1, 9).ToList();
                numbers = numbers.OrderBy(x => Guid.NewGuid()).ToList();
                foreach (int val in numbers)
                {
                    if (!solutionRules.PlacementIsValid(x, y, val)) { continue; }

                    // Try value in this position
                    solution[x, y] = val;

                    // Try to solve the result of the values
                    yield return GenerateSolution();

                    // If a solution was found then we can break
                    if (solutionRules.IsSolved()) { yield break; }

                    // Otherwise we can revert our choice
                    solution[x, y] = 0;
                }
                yield break;
            }
        }
    }

    int hashXY(int x, int y)
    {
        return x * 9 + y;
    }

    IEnumerator RemoveClue()
    {
        List<int> xValues = Enumerable.Range(0, 9).ToList();
        xValues = xValues.OrderBy(x => Guid.NewGuid()).ToList();

        List<int> yValues = Enumerable.Range(0, 9).ToList();
        yValues = yValues.OrderBy(x => Guid.NewGuid()).ToList();


        foreach (int x in xValues)
        {
            foreach (int y in yValues)
            {
                if (problem[x, y] == 0) { continue; }
                int posHash = hashXY(x, y);

                if (requiredClueCache.Contains(posHash)) { continue; }
                int value = problem[x, y];
                problem[x, y] = 0;
                solver = new SudokuCspSolver(problem);
                yield return solver.Solve();

                if (solver.nSolutions > 1)
                {
                    Debug.Log($"Cannot remove {x}, {y}");
                    requiredClueCache.Add(posHash);
                    problem[x, y] = value;
                }
                else { yield break; }
            }
        }
        Debug.Log("Cannot remove any more clues without producing multiple solutions");
        finished = true;
    }


    public IEnumerator Generate()
    {
        float t1 = Time.time;
        requiredClueCache = new List<int>();
        finished = false;
        yield return GenerateSolution();
        CopySolutionToProblem();

        while (!finished)
        {
            yield return RemoveClue();
        }

        Debug.Log($"Generated hardest possible puzzle in {Time.time - t1} seconds");
    }
}
