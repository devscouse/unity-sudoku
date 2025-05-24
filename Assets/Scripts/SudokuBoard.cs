using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Material clueMat;

    [Header("Sudoku Generator Controls")]
    public int mediumExtraClues;
    public int easyExtraClues;

    private GameObject[,] cells;
    private int[,] values;
    private SudokuRules rules;
    private SudokuCspSolver solver;
    private Vector2Int selectedCell;

    void InstantiateCells()
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
                if ((y + 1) % 3 == 0) { yPos += majorPadding; }
                else { yPos += minorPadding; }
                values[x, y] = 0;
            }
            xPos += cellSize;
            if ((x + 1) % 3 == 0) { xPos += majorPadding; }
            else { xPos += minorPadding; }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InstantiateCells();
        rules = new SudokuRules(values);
    }

    public IEnumerator Solver()
    {
        float t1 = Time.time;
        yield return solver.Solve();
        float runtime = Time.time - t1;
        Debug.Log($"{solver.nSolutions} distinct solutions found in {runtime} seconds");
    }
    public void Solve()
    {
        solver = new SudokuCspSolver(values);
        StartCoroutine(Solver());
    }

    public IEnumerator Generate(int extraClues)
    {
        ResetBoard();
        SudokuGenerator generator = new();
        yield return generator.Generate();
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (generator.problem[x, y] == 0) { continue; }
                SetClue(x, y, generator.problem[x, y]);
            }
        }
        List<int> cellIds = Enumerable.Range(0, 80).ToList();
        cellIds = cellIds.OrderBy(x => Guid.NewGuid()).ToList();
        int i = 0;
        while (extraClues > 0)
        {
            int cellId = cellIds[i++];
            int x = cellId / 9;
            int y = cellId % 9;
            if (GetValue(x, y) == 0)
            {
                SetClue(x, y, generator.solution[x, y]);
                extraClues--;
                Debug.Log($"Clue added at {x}, {y}");
            }
        }
    }

    public void ResetBoard()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                GetSudokuCell(x, y).isClue = false;
                SetValue(x, y, 0);
            }
        }
    }

    SudokuCell[] GetSudokuCellRow(int y)
    {
        SudokuCell[] row = new SudokuCell[9];
        for (int x = 0; x < 9; x++)
        {
            row[x] = GetSudokuCell(x, y);
        }
        return row;
    }

    SudokuCell[] GetSudokuCellCol(int x)
    {
        SudokuCell[] col = new SudokuCell[9];
        for (int y = 0; y < 9; y++)
        {
            col[y] = GetSudokuCell(x, y);
        }
        return col;
    }

    SudokuCell[] GetSudokuCellSquare(int x, int y)
    {
        SudokuCell[] square = new SudokuCell[9];
        int gridX = (x / 3) * 3;
        int gridY = (y / 3) * 3;
        int i = 0;
        for (int dx = 0; dx < 3; dx++)
        {
            for (int dy = 0; dy < 3; dy++)
            {
                int x2 = gridX + dx;
                int y2 = gridY + dy;
                square[i++] = GetSudokuCell(x2, y2);
            }
        }
        return square;
    }

    public SudokuCell GetSudokuCell(int x, int y)
    {
        return cells[x, y].GetComponent<SudokuCell>();
    }

    SudokuCell[,] GetSudokuCells()
    {
        SudokuCell[,] allCells = new SudokuCell[9, 9];
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                allCells[x, y] = GetSudokuCell(x, y);
            }
        }
        return allCells;
    }

    public void SetSelectedCell(int x, int y)
    {
        selectedCell = new Vector2Int(x, y);
    }

    public void SetClue(int x, int y, int value)
    {
        values[x, y] = value;
        SetValue(x, y, value);
        GetSudokuCell(x, y).isClue = true;
    }

    public void SetValue(int x, int y, int value)
    {
        SudokuCell cell = GetSudokuCell(x, y);
        if (cell.isClue)
        {
            Debug.LogWarning("Invalid attempt to set the value of a clue cell");
            return;
        }
        values[x, y] = value;
        if (value == 0) { cell.SetLabel(""); }
        else { cell.SetLabel(value.ToString()); }
    }

    public int GetValue(int x, int y) { return values[x, y]; }
    public int[,] GetValues() { return values; }


    public bool CheckBoardForErrors()
    {
        bool result = false;
        foreach (SudokuCell cell in GetSudokuCells())
        {
            if (!cell.isClue) { cell.SetMaterial(normalMat); }
            else { cell.SetMaterial(clueMat); }
        }
        for (int i = 0; i < 9; i++)
        {
            if (!rules.RowIsValid(i))
            {
                result = true;
                foreach (SudokuCell cell in GetSudokuCellRow(i)) { cell.SetMaterial(errMat); }
            }
            if (!rules.ColIsValid(i))
            {
                result = true;
                foreach (SudokuCell cell in GetSudokuCellCol(i)) { cell.SetMaterial(errMat); }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (!rules.SquareIsValid(i * 3, j * 3))
                {
                    result = true;
                    foreach (SudokuCell cell in GetSudokuCellSquare(i * 3, j * 3)) { cell.SetMaterial(errMat); }
                }
            }
        }
        return result;
    }
    void FixedUpdate()
    {
        CheckBoardForErrors();
        GetSudokuCell(selectedCell.x, selectedCell.y).SetMaterial(selectedMat);
    }

    public void Hide()
    {
        foreach (SudokuCell cell in GetSudokuCells())
            cell.GetComponent<MeshRenderer>().enabled = false;
    }

    public void Show()
    {
        foreach (SudokuCell cell in GetSudokuCells())
            cell.GetComponent<MeshRenderer>().enabled = true;
    }
}
