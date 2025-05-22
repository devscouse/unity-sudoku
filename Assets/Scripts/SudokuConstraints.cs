using System.Collections.Generic;
using UnityEngine;

class SudokuConstraints
{
    private uint[,] domains;
    private SudokuValues values;

    public SudokuConstraints(SudokuValues values)
    {
        this.values = values;
        domains = new uint[9, 9];
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                for (int v = 1; v < 10; v++)
                    AddValueToDomain(x, y, v);
            }
        }
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (values.GetValue(x, y) != 0)
                {
                    SetValue(x, y, values.GetValue(x, y));
                }
            }
        }
    }

    void AddValueToDomain(int x, int y, int value)
    {
        domains[x, y] |= 1u << (value - 1);
    }

    void RemoveValueFromDomain(int x, int y, int value)
    {
        domains[x, y] &= ~(1u << (value - 1));
    }

    bool DomainHasValue(int x, int y, int value)
    {
        return (domains[x, y] >> (value - 1) & 1u) == 1;
    }

    int DomainValueCount(int x, int y)
    {
        int count = 0;
        for (int val = 1; val < 10; val++)
        {
            if (DomainHasValue(x, y, val)) { count++; }
        }
        return count;
    }

    void ReCalculateDomain(int x, int y)
    {
        domains[x, y] = 0;
        foreach (int value in values.GetCol(x)) { RemoveValueFromDomain(x, y, value); }
        foreach (int value in values.GetRow(y)) { RemoveValueFromDomain(x, y, value); }
        foreach (int value in values.GetSquare(x, y)) { RemoveValueFromDomain(x, y, value); }
    }

    public void SetValue(int x, int y, int value)
    {
        // Enable constraint for this value in affected positions
        foreach (Pos pos in values.GetColPositions(x)) { RemoveValueFromDomain(pos.X, pos.Y, value); }
        foreach (Pos pos in values.GetRowPositions(y)) { RemoveValueFromDomain(pos.X, pos.Y, value); }
        foreach (Pos pos in values.GetSquarePositions(x, y)) { RemoveValueFromDomain(pos.X, pos.Y, value); }
    }

    public void WipeValue(int x, int y)
    {
        int currValue = values.GetValue(x, y);
        foreach (Pos pos in values.GetColPositions(x)) { AddValueToDomain(pos.X, pos.Y, currValue); }
        foreach (Pos pos in values.GetRowPositions(y)) { AddValueToDomain(pos.X, pos.Y, currValue); }
        foreach (Pos pos in values.GetSquarePositions(x, y)) { AddValueToDomain(pos.X, pos.Y, currValue); }
        ReCalculateDomain(x, y);
    }


    public List<int> GetDomainValues(int x, int y)
    {
        List<int> values = new();
        for (int value = 1; value < 10; value++)
        {
            if (DomainHasValue(x, y, value)) { values.Add(value); }
        }
        return values;
    }

    public Pos GetEmptyPosWithLeastConstraints()
    {
        int leastValues = 10;
        Pos pos = null;
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (values.GetValue(x, y) != 0) { continue; }
                int nValues = DomainValueCount(x, y);
                if (nValues < leastValues)
                {
                    leastValues = nValues;
                    pos = new Pos(x, y);
                }
            }
        }
        Debug.Log($"Position {pos.X}, {pos.Y} has smallest domain: {leastValues}");
        return pos;
    }

    public bool AllEmptyPositionsHavePlaceableValues()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (values.GetValue(x, y) != 0) { continue; }
                if (DomainValueCount(x, y) == 0)
                {
                    Debug.Log($"Empty cell at {x}, {y} has an empty domain");
                    return false;
                }
            }
        }
        return true;
    }
}
