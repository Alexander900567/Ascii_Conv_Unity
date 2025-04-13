using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ImageConvertor : Tool
{
    //normal vars
    [SerializeField] private GameObject optionsContainer;
    [SerializeField] private GameObject outline;
    private GameObject outlineInstance;
    [SerializeField] private Texture2D image;
    private (int row, int col) topLeft;
    private (int row, int col) botRight;
    private List<int> asciiMap;
    
    private bool imageActive = false;
    private bool outlineActive = false;
    private bool equalizerActive = false;


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
        optionsContainer.SetActive(true);
    }
    public override void onExit(){
        optionsContainer.SetActive(false);
        if (outlineActive){
            Destroy(outlineInstance);
        }
        outlineActive = false;
    }

    public void filePathToTexture(string filePath){
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
    public void setCornersManual((int row, int col) topL, (int row, int col) botR){
        topLeft = topL;
        botRight = botR;
    }

    public List<List<char>> performConversion(){
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
        
        //equalize luminancePerChar
        float lowLumi = 0;
        if(equalizerActive){
            Dictionary<float, int> lumiCounts = new Dictionary<float, int>();
            foreach(Color pixel in pixels){
                float lumi = pixel.grayscale;
                if(lumiCounts.ContainsKey(lumi)){
                    lumiCounts[lumi] += 1;
                }
                else{
                    lumiCounts[lumi] = 0;
                }
            }
            List<float> luminaceList = new List<float>(lumiCounts.Keys);
            luminaceList.Sort();
            int fivePercent = (int)(downscaledTexture.width * downscaledTexture.height * 0.05);

            int lumiAdded = 0;
            lowLumi = 0;
            for (
                int lumiInd = 0; 
                lumiInd < luminaceList.Count && lumiAdded < fivePercent; 
                lumiInd+=1
            ){
                float currentLuminace = luminaceList[lumiInd];
                int occurences = lumiCounts[currentLuminace];
                int multiplier = Mathf.Min(fivePercent - lumiAdded, occurences);
                lowLumi += currentLuminace * multiplier;
                lumiAdded += multiplier;
            }
            lowLumi = lowLumi / (float) lumiAdded; 

            lumiAdded = 0;
            float highLumi = 0;
            for (
                int lumiInd = luminaceList.Count - 1; 
                lumiInd >= 0 && lumiAdded < fivePercent; 
                lumiInd-=1
            ){
                float currentLuminace = luminaceList[lumiInd];
                int occurences = lumiCounts[currentLuminace];
                int multiplier = Mathf.Min(fivePercent - lumiAdded, occurences);
                highLumi += currentLuminace * multiplier;
                lumiAdded += multiplier;
            }
            highLumi = highLumi / (float) lumiAdded; 

            luminacePerChar = (float)(highLumi - lowLumi) / (maxMapIndex + 1);
        }

        int col = topLeft.col;
        foreach(Color pixel in pixels){
            int index = (int)((pixel.grayscale - lowLumi) / luminacePerChar);
            index = Mathf.Min(maxMapIndex, index); 
            index = Mathf.Max(0, index); 

            outputList[0].Add((char)asciiMap[index]);
            col += 1;
            if (col > botRight.col){
                outputList.Insert(0, new List<char>());
                col = topLeft.col;
            }
        }
        RenderTexture.ReleaseTemporary(imager);

        if(outputList[0].Count < 1){
            outputList.RemoveAt(0);
        }

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

    public void toggleEqualizer(bool toggleState){
        equalizerActive = toggleState;
    }
}
