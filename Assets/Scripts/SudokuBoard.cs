using System;
using Unity.VisualScripting;
using UnityEngine;

public class SudokuBoard : MonoBehaviour
{
    public float minorPadding;
    public float majorPadding;
    public GameObject cellPrefab;

    [Header("Sudoku Cell Materials")]
    public Material normalMat;
    public Material selectedMat;
    public Material errMat;

    private GameObject[,] cells;
    private int[,] values;
    private SudokuSolver solver;
    private SudokuRules rules;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cells = new GameObject[9, 9];
        values = new int[9, 9];
        float cellSize = cellPrefab.GetComponent<MeshRenderer>().bounds.size.x;

        float gridSize = cellSize * 8 + minorPadding * 6 + majorPadding * 2;
        float xPos = -gridSize / 2;
        for (int x = 0; x < 9; x++)
        {
            float yPos = -gridSize / 2;
            for (int y = 0; y < 9; y++)
            {
                GameObject cell = Instantiate(cellPrefab, new Vector3(xPos, 0.1f, yPos), cellPrefab.transform.rotation);
                SudokuCell sudokuCell = cell.GetComponent<SudokuCell>();
                sudokuCell.SetX(x);
                sudokuCell.SetY(y);
                cells[x, y] = cell;

                yPos += cellSize;
                if ((y + 1) % 3 == 0)
                {
                    yPos += majorPadding;
                }
                else
                {
                    yPos += minorPadding;
                }
                values[x, y] = 0;
            }
            xPos += cellSize;
            if ((x + 1) % 3 == 0)
            {
                xPos += majorPadding;
            }
            else
            {
                xPos += minorPadding;
            }
        }

        solver = GetComponent<SudokuSolver>();
        rules = new SudokuRules(values);
    }

    public void Solve()
    {
        solver.CopyBoardToWorkspace(values);
        if (solver.Solve())
        {
            Debug.Log("Board can be solved!");
            int[,] solution = solver.GetSolution();
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    SetValue(x, y, solution[x, y]);
                }
            }
        }
    }

    SudokuCell GetSudokuCell(int x, int y)
    {
        return cells[x, y].GetComponent<SudokuCell>();
    }

    public void MarkSelectedCell(int x, int y)
    {
        GetSudokuCell(x, y).SetMaterial(selectedMat);
    }

    public void MarkDeselectedCell(int x, int y)
    {
        GetSudokuCell(x, y).SetMaterial(normalMat);
    }

    public void SetValue(int x, int y, int value)
    {
        if (!rules.PlacementIsValid(x, y, value))
        {
            GetSudokuCell(x, y).SetMaterial(errMat);
        }
        values[x, y] = value;
        GetSudokuCell(x, y).SetLabel(value.ToString());
    }

    public void WipeValue(int x, int y)
    {
        values[x, y] = 0;
        GetSudokuCell(x, y).SetLabel("");
    }

    public int GetValue(int x, int y) { return values[x, y]; }
    public int[,] GetValues() { return values; }
}
