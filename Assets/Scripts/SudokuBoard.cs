using System;
using System.Collections;
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

    private GameObject[,] cells;
    private int[,] values;
    private SudokuRules rules;
    private SudokuBacktrackGenerator generator;
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


        generator = new SudokuBacktrackGenerator();
        solver = new SudokuCspSolver(values);
        StartCoroutine(generator.Generate());
    }

    public void LoadPuzzle()
    {
        SetValue(0, 0, 9);
        SetValue(1, 0, 2);
        SetValue(2, 0, 6);
        SetValue(7, 1, 8);
        SetValue(0, 2, 3);
        SetValue(3, 3, 1);
        SetValue(0, 4, 7);
        SetValue(1, 4, 5);
        SetValue(4, 4, 2);
        SetValue(6, 4, 9);
        SetValue(2, 5, 3);
        SetValue(3, 5, 5);
        SetValue(4, 5, 6);
        SetValue(7, 5, 1);
        SetValue(0, 6, 2);
        SetValue(3, 6, 4);
        SetValue(4, 6, 1);
        SetValue(6, 6, 5);
        SetValue(8, 6, 7);
        SetValue(8, 7, 4);
        SetValue(2, 8, 5);
        SetValue(3, 8, 6);
        SetValue(4, 8, 8);
        SetValue(8, 8, 3);
        solver = new SudokuCspSolver(values);
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
        StartCoroutine(Solver());
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

    SudokuCell GetSudokuCell(int x, int y)
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

    public void SetValue(int x, int y, int value)
    {
        values[x, y] = value;
        if (value == 0)
        {
            GetSudokuCell(x, y).SetLabel("");
        }
        else
        {
            GetSudokuCell(x, y).SetLabel(value.ToString());
        }
    }

    public int GetValue(int x, int y) { return values[x, y]; }
    public int[,] GetValues() { return values; }

    public bool CheckBoardForErrors()
    {
        bool result = false;
        foreach (SudokuCell cell in GetSudokuCells()) { cell.SetMaterial(normalMat); }
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

        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                SetValue(x, y, generator.problem[x, y]);
            }
        }

    }
}
