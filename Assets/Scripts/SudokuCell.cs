using System;
using TMPro;
using UnityEngine;

public class SudokuCell : MonoBehaviour
{
    private TextMeshProUGUI label;
    private int x;
    private int y;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetMaterial(Material mat)
    {
        MeshRenderer rend = GetComponent<MeshRenderer>();
        rend.material = mat;
    }

    public void SetLabel(String text)
    {
        label.text = text;
    }

    public String GetLabel()
    {
        return label.text;
    }

    public void SetX(int x) { this.x = x; }
    public void SetY(int y) { this.y = y; }
    public int GetX() { return x; }
    public int GetY() { return y; }
}
