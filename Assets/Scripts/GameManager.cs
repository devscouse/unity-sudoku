using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera mainCam;
    public SudokuBoard board;
    public GameObject titleScreen;

    public GameObject gameOverScreen;
    public GameObject gameOverEffect;
    public GameObject spinner;
    public GameObject timer;

    public int easyExtraClues;
    public int mediumExtraClues;
    public int hardExtraClues;

    public float directionPollDelay;

    private SudokuCell selectedCell;
    private bool acceptingInput = false;
    private float startTime;
    private float directionInputTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        titleScreen.SetActive(true);
        spinner.SetActive(false);
        gameOverScreen.SetActive(false);
        gameOverEffect.SetActive(false);
        timer.SetActive(false);
    }

    IEnumerator Generate(int extraClues)
    {
        titleScreen.SetActive(false);
        spinner.SetActive(true);
        yield return board.Generate(extraClues);
        spinner.SetActive(false);
        board.Show();
        acceptingInput = true;
        startTime = Time.time;
        timer.SetActive(true);
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

    void SelectCell(SudokuCell cell)
    {
        selectedCell = cell;
        board.SetSelectedCell(cell.GetX(), cell.GetY());
        Debug.Log($"Cell ({selectedCell.GetX()}, {selectedCell.GetY()}) selected");
    }

    void SelectCell(int x, int y)
    {
        SelectCell(board.GetSudokuCell(x, y));
    }

    void ProcessUserInput()
    {
        float timeSinceDirectionGiven = Time.time - directionInputTime;
        if (selectedCell == null) { SelectCell(0, 0); }


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 20, Color.white);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!hit.collider.CompareTag("Cell")) { return; }
                SelectCell(hit.collider.gameObject.GetComponent<SudokuCell>());
            }
        }
        else if (timeSinceDirectionGiven > directionPollDelay)
        {
            float vInput = Input.GetAxisRaw("Vertical");
            float hInput = Input.GetAxisRaw("Horizontal");

            if (vInput != 0)
            {
                if (vInput > 0 && selectedCell.GetY() < 9)
                {
                    SelectCell(selectedCell.GetX(), selectedCell.GetY() + 1);
                }
                else if (vInput < 0 && selectedCell.GetY() > 0)
                {
                    SelectCell(selectedCell.GetX(), selectedCell.GetY() - 1);
                }
                directionInputTime = Time.time;
            }
            else if (hInput != 0)
            {
                if (hInput > 0 && selectedCell.GetX() < 9)
                {
                    SelectCell(selectedCell.GetX() + 1, selectedCell.GetY());
                }
                else if (hInput < 0 && selectedCell.GetX() > 0)
                {
                    SelectCell(selectedCell.GetX() - 1, selectedCell.GetY());
                }
                directionInputTime = Time.time;
            }
        }
        if (GetInputNumber(out int number))
        {
            board.SetValue(selectedCell.GetX(), selectedCell.GetY(), number);
            if (board.IsSolved())
            {
                board.Hide();
                timer.SetActive(false);

                TextMeshProUGUI timeTakenText = gameOverScreen.transform.Find("TimeTakenText").GetComponent<TextMeshProUGUI>();
                float timeSinceStart = Time.time - startTime;
                int minutes = (int)(timeSinceStart / 60);
                int seconds = (int)(timeSinceStart % 60);
                timeTakenText.text = $"You solved the puzzle in {minutes} minutes and {seconds} seconds";
                gameOverScreen.SetActive(true);
                gameOverEffect.SetActive(true);
            }
        }
    }

    void SetTimer()
    {
        float timeSinceStart = Time.time - startTime;
        int minutes = (int)(timeSinceStart / 60);
        int seconds = (int)(timeSinceStart % 60);
        string secondsString = seconds.ToString();
        if (seconds < 10) { secondsString = "0" + secondsString; }
        string timeString = $"{minutes}:{secondsString}";

        timer.GetComponent<TextMeshProUGUI>().text = timeString;
    }

    void Update()
    {
        if (acceptingInput) { ProcessUserInput(); }
        SetTimer();
    }

    public void Restart()
    {
        gameOverScreen.SetActive(false);
        gameOverEffect.SetActive(false);
        titleScreen.SetActive(true);
    }

}
