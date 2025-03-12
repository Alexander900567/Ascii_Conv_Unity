using UnityEngine;

public abstract class Tool : MonoBehaviour
{

    [SerializeField] protected GlobalOperations globalOperations;
    [SerializeField] protected GridManager gridManager;
    protected (int row, int col) startGpos = (-1, -1);
    protected bool clickedGrid = false; 

    // Update is called once per frame
    public virtual void onUpdate(){
        handleInput();
    }

    public virtual void handleInput(){
        if (isMouseOnGrid() && globalOperations.controls.Grid.MainClick.triggered){
            clickedGrid = true;
            startGpos = gridManager.getGridPos();
            draw();
        }
        else if (clickedGrid && globalOperations.controls.Grid.MainClick.IsPressed()){
           draw(); 
        }

        if (clickedGrid && globalOperations.controls.Grid.MainClick.WasReleasedThisFrame()){
            gridManager.writePbufferToArray();
            clickedGrid = false;
            startGpos = (-1, -1);
        }
    }

    public virtual void draw(){}

    public virtual void onEnter(){return;}

    public virtual void onExit(){return;}

    public bool isMouseOnGrid(){
        Vector3 mousePos = Input.mousePosition;
        return mousePos.x > gridManager.getUiBarWidth();
    }
}
