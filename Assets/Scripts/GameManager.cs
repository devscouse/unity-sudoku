using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera mainCam;
    public SudokuBoard board;


    private SudokuCell selectedCell;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (selectedCell != null)
            {
                board.MarkDeselectedCell(selectedCell.GetX(), selectedCell.GetY());
                selectedCell = null;
            }
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 20, Color.white);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Cell"))
                {
                    selectedCell = hit.collider.gameObject.GetComponent<SudokuCell>();
                    board.MarkSelectedCell(selectedCell.GetX(), selectedCell.GetY());
                    Debug.Log($"Cell ({selectedCell.GetX()}, {selectedCell.GetY()}) selected");
                }
            }
        }
        if (selectedCell != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Backspace))
            {
                board.SetValue(selectedCell.GetX(), selectedCell.GetY(), 0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                board.SetValue(selectedCell.GetX(), selectedCell.GetY(), 1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                board.SetValue(selectedCell.GetX(), selectedCell.GetY(), 2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                board.SetValue(selectedCell.GetX(), selectedCell.GetY(), 3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                board.SetValue(selectedCell.GetX(), selectedCell.GetY(), 4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                board.SetValue(selectedCell.GetX(), selectedCell.GetY(), 5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                board.SetValue(selectedCell.GetX(), selectedCell.GetY(), 6);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                board.SetValue(selectedCell.GetX(), selectedCell.GetY(), 7);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                board.SetValue(selectedCell.GetX(), selectedCell.GetY(), 8);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                board.SetValue(selectedCell.GetX(), selectedCell.GetY(), 9);
            }
        }
    }

}
