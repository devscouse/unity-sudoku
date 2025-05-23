using System;
using TMPro;
using UnityEngine;

public class SudokuCell : MonoBehaviour
{
    private TextMeshProUGUI label;
    private int x;
    private int y;
    public bool isClue = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        label = transform.Find("Canvas/Label").GetComponent<TextMeshProUGUI>();
    }

    public void SetMaterial(Material mat)
    {
        MeshRenderer rend = GetComponent<MeshRenderer>();
        rend.material = mat;
    }

    public void SetLabel(string text)
    {
        if (text != "")
        {
            for (int value = 1; value < 10; value++)
                SetNoteInactive(value);
        }
        label.text = text;
    }

    public string GetLabel()
    {
        return label.text;
    }

    public void SetNoteInactive(int value)
    {
        if (value == 0)
            return;
        transform.Find($"Canvas/Notes{value}").gameObject.SetActive(false);
    }

    public void SetNoteActive(int value)
    {
        if (value == 0)
            return;
        transform.Find($"Canvas/Notes{value}").gameObject.SetActive(true);
    }

    public void SetX(int x) { this.x = x; }
    public void SetY(int y) { this.y = y; }
    public int GetX() { return x; }
    public int GetY() { return y; }
}
