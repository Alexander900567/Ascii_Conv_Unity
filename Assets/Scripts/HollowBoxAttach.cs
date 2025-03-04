using UnityEngine;

public class HollowBoxAttach : MonoBehaviour
{

    [SerializeField] private GridManager gridManager;
    private bool isHidden = true;

    void Start(){
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        setHidden(false);
    }

    public void renderBox((int row, int col) topLeft, (int row, int col) botRight){
        if (!isHidden){
            (int row, int col) size = (Mathf.Abs(topLeft.row - botRight.row) + 1, Mathf.Abs(topLeft.col - botRight.col) + 1);
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(
                gridManager.getColSize() * size.col,
                gridManager.getRowSize() * size.row 
            );
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                gridManager.getUiBarWidth() + topLeft.col * gridManager.getColSize(),
                gridManager.invertRowPos(botRight.row) * gridManager.getRowSize()
            );
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
    }

    public void setHidden(bool updatedStatus){
        if (updatedStatus){
            isHidden = updatedStatus;
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        }
        else{
            isHidden = updatedStatus;
        }
    }

}
