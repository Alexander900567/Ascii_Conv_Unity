using UnityEngine;

public class FindAndReplace : MonoBehaviour
{
    [SerializeField] private GlobalOperations global;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject findReplacePopUp;

    public void openFindReplacePopUp(){
        global.openPopUp(findReplacePopUp);
    }

    public void replaceCharacters(char targetChar, char replaceWith){
        for(int row = 0; row < gridManager.getRowCount(); row+=1){
            for(int col = 0; col < gridManager.getColCount(); col+=1){
                if(gridManager.getGarrSpace(row, col) == targetChar){
                    gridManager.addToPreviewBuffer(row, col, replaceWith);
                }
            }
        }
        gridManager.writePbufferToArray();
        global.renderUpdate = true;
    }

}
