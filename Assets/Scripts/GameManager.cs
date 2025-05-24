using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera mainCam;
    public SudokuBoard board;
    public GameObject titleScreen;
    public GameObject spinner;

    public int easyExtraClues;
    public int mediumExtraClues;
    public int hardExtraClues;

    private SudokuCell selectedCell;
    private bool acceptingInput = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        titleScreen.SetActive(true);
    }

    IEnumerator Generate(int extraClues)
    {
        titleScreen.SetActive(false);
        spinner.SetActive(true);
        yield return board.Generate(extraClues);
        spinner.SetActive(false);
        board.Show();
        acceptingInput = true;
    }

    public void GenerateEasyPuzzle()
    {
        StartCoroutine(Generate(easyExtraClues));
    }

    public void GenerateMediumPuzzle()
    {
        StartCoroutine(Generate(mediumExtraClues));
    }

    public void GenerateHardPuzzle()
    {
        StartCoroutine(Generate(hardExtraClues));
    }


    bool GetInputNumber(out int number)
    {
        number = -1;
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Backspace))
        {
            number = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1)) { number = 1; }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) { number = 2; }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { number = 3; }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) { number = 4; }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) { number = 5; }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) { number = 6; }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) { number = 7; }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) { number = 8; }
        else if (Input.GetKeyDown(KeyCode.Alpha9)) { number = 9; }
        else { return false; }
        return true;

    }

    void ProcessUserInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 20, Color.white);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Cell"))
                {
                    selectedCell = hit.collider.gameObject.GetComponent<SudokuCell>();
                    board.SetSelectedCell(selectedCell.GetX(), selectedCell.GetY());
                    Debug.Log($"Cell ({selectedCell.GetX()}, {selectedCell.GetY()}) selected");
                }
            }
        }
        if (selectedCell != null && GetInputNumber(out int number))
        {
            board.SetValue(selectedCell.GetX(), selectedCell.GetY(), number);
        }
    }

    void Update()
    {
        if (acceptingInput) { ProcessUserInput(); }
    }

}
