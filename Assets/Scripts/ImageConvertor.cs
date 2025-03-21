using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ImageConvertor : Tool
{
    //normal vars
    [SerializeField] private GameObject chooseImageButton;
    [SerializeField] private GameObject convertButton;
    [SerializeField] private GameObject outline;
    [SerializeField] private SaveLoad saveLoad;
    [SerializeField] private GameObject videoPopUp;
    private GameObject outlineInstance;
    [SerializeField] private Texture2D image;
    private (int row, int col) topLeft;
    private (int row, int col) botRight;
    private List<int> asciiMap;
    
    //state vars
    private bool imageActive = false;
    private bool outlineActive = false;

    public override void onUpdate(){
        base.onUpdate();
        if (outlineActive){
            outlineInstance.GetComponent<HollowBoxAttach>().renderBox(topLeft, botRight);
        }
    }

    public override void handleInput(){
        if (isMouseOnGrid() && globalOperations.controls.Grid.MainClick.triggered){
            clickedGrid = true;
            startGpos = gridManager.getGridPos();

            if (!outlineActive){
                outlineInstance = Instantiate(
                    outline,
                    new Vector3(0, 0, 0),
                    transform.rotation
                );
                outlineInstance.transform.SetParent(gridManager.canvasTransform);
                outlineActive = true;
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
        convertButton.SetActive(true);
    }
    public override void onExit(){
        chooseImageButton.SetActive(false);
        convertButton.SetActive(false);
        if (outlineActive){
            Destroy(outlineInstance);
        }
        outlineActive = false;
    }

    private void filePathToTexture(string filePath){
        image = new Texture2D(1, 1);
        byte[] imageByteArray = File.ReadAllBytes(filePath);
        ImageConversion.LoadImage(image, imageByteArray, false);
        imageActive = true;
    }

    public void uploadImage(){
        string filePath = EditorUtility.OpenFilePanel("Choose an image", "~", "png,jpg");
        if (filePath == ""){
            return;
        }
        filePathToTexture(filePath);
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

    private List<List<char>> performConversion(){
        asciiMap = new List<int> {' ', '.', ':', '-', '=', '+' , '*', '#', '%', '@'};
        //asciiMap = new List<int> {' ', '+', '@'};
        //asciiMap = new List<int> {' ', '.', ':', 'c', 'o', 'P', 'O', '?', '@'};
        int maxMapIndex = asciiMap.Count - 1;
        int heightCount = (botRight.row - topLeft.row) + 1;
        int widthCount = (botRight.col - topLeft.col) + 1;
        float luminacePerChar = (float)1.0 / (maxMapIndex + 1);

        RenderTexture imager = RenderTexture.GetTemporary(widthCount, heightCount);
        Graphics.Blit(image, imager); 
        Graphics.SetRenderTarget(imager);
        Texture2D downscaledTexture = new Texture2D(widthCount, heightCount);
        downscaledTexture.ReadPixels(
            new Rect(0, 0, widthCount, heightCount),
            0, 0, false 
        );
        Color[] pixels = downscaledTexture.GetPixels();

        List<List<char>> outputList = new List<List<char>>();
        outputList.Add(new List<char>());

        int col = topLeft.col;
        foreach(Color pixel in pixels){
            int index = Mathf.Min(maxMapIndex, (int)(pixel.grayscale / luminacePerChar)); 
            outputList[0].Add((char)asciiMap[index]);
            col += 1;
            if (col > botRight.col){
                outputList.Insert(0, new List<char>());
                col = topLeft.col;
            }
        }
        RenderTexture.ReleaseTemporary(imager);

        return outputList;
    }    

    public void imageToGridArray(){
        if (!outlineActive || !imageActive){
            return;
        }

        List<List<char>> outputList = performConversion();

        int row = topLeft.row;
        int col = topLeft.col;
        foreach(List<char> charRow in outputList){
            foreach(char character in charRow){
                gridManager.addToPreviewBuffer(row, col, character);
                col += 1;
                if (col > botRight.col){
                    row += 1;
                    col = topLeft.col;
                }
            }
        }
        gridManager.writePbufferToArray();
        globalOperations.renderUpdate = true;
    }

    public void displayVideoPopUp(){
        globalOperations.openPopUp(videoPopUp);
    }

    public void convertVideo(int frameRate){
        string filePath = EditorUtility.OpenFolderPanel("Choose a directory of images", "~", "");
        if (filePath == ""){
            return;
        }

        DirectoryInfo dirObj = new DirectoryInfo(filePath);
        FileInfo[] fileList = dirObj.GetFiles();

        string[] saveFileNameArray = {filePath, "video.txt"};
        StreamWriter saveFile = File.CreateText(Path.Combine(saveFileNameArray));
        saveFile.WriteLine($"rowCount:{gridManager.getRowCount()}");
        saveFile.WriteLine($"colCount:{gridManager.getColCount()}");
        saveFile.WriteLine($"frameRate:{frameRate}");

        topLeft = (row: 0, col: 0);
        botRight = (row: gridManager.getRowCount() - 1, col: gridManager.getColCount() - 1);
        

        foreach(FileInfo file in fileList){
            string fileExtension = file.Name.Split('.')[^1];

            if(fileExtension != "png" && fileExtension != "jpg" && fileExtension != "jpeg"){
                continue;
            }

            filePathToTexture(file.FullName);

            

        }


        //filePathToTexture(filePath);

    }

    public void playVideo(){

    }
}
