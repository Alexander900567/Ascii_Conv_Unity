using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridSizePopUp : MonoBehaviour
{
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_InputField rowInput;
    [SerializeField] private TMP_InputField colInput;
    private GridManager gridManager;
    private GlobalOperations global;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        global = GameObject.Find("GlobalOperations").GetComponent<GlobalOperations>();

        cancelButton.onClick.AddListener(global.closePopUp);
        submitButton.onClick.AddListener(onSubmit);

        rowInput.text = gridManager.getRowCount().ToString();
        colInput.text = gridManager.getColCount().ToString();
    }

    public void onSubmit(){
        int newRow = System.Int32.Parse(rowInput.text);
        int newCol = System.Int32.Parse(colInput.text);

        gridManager.resizeGrid(newRow, newCol);
        global.closePopUp();
    }


}
