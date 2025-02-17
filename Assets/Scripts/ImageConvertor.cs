using UnityEngine;
using UnityEditor;
using System.IO;

public class ImageConvertor : Tool
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private GameObject chooseImageButton;
    [SerializeField] private GameObject outline;
    private GameObject outlineInstance;
    private Texture2D image;
    private (int row, int col) topLeft;
    private (int row, int col) botRight;
    //private bool imageUploaded;
    private bool activeOutline = false;
    private bool conversionUpdate = false;

    public override void onUpdate(){
        base.onUpdate();
        if (activeOutline){
            outlineInstance.GetComponent<HollowBoxAttach>().renderBox(topLeft, botRight);
        }
        if (conversionUpdate){
            performConversion();
        }
    }

    public override void handleInput(){
        if (isMouseOnGrid() && globalOperations.controls.Grid.MainClick.triggered){
            clickedGrid = true;
            startGpos = gridManager.getGridPos();

            if (!activeOutline){
                outlineInstance = Instantiate(
                    outline,
                    new Vector3(0, 0, 0),
                    transform.rotation
                );
                outlineInstance.transform.SetParent(gridManager.canvasTransform);
                activeOutline = true;
                setCorners(gridManager.getGridPos());
            }
        }
        else if (clickedGrid && globalOperations.controls.Grid.MainClick.IsPressed()){
           setCorners(gridManager.getGridPos());
        }

        if (clickedGrid && globalOperations.controls.Grid.MainClick.WasReleasedThisFrame()){
            clickedGrid = false;
            startGpos = (-1, -1);
        }
    }

    public override void onEnter(){
        chooseImageButton.SetActive(true);
    }
    public override void onExit(){
        chooseImageButton.SetActive(false);
        if (activeOutline){
            Destroy(outlineInstance);
        }
        activeOutline = false;
    }

    public void uploadImage(){
        string filePath = EditorUtility.OpenFilePanel("Choose an image", "~", "png");
        if (filePath == ""){
            return;
        }
        image = new Texture2D(1, 1);
        byte[] imageByteArray = File.ReadAllBytes(filePath);
        ImageConversion.LoadImage(image, imageByteArray, false);
    }

    public void setCorners((int row, int col) newGpos){
        topLeft = (
            Mathf.Min(startGpos.row, newGpos.row),
            Mathf.Min(startGpos.col, newGpos.col)
        );
        botRight = (
            Mathf.Max(startGpos.row, newGpos.row),
            Mathf.Max(startGpos.col, newGpos.col)
        );
    }

    public void performConversion(){

    }    
}
