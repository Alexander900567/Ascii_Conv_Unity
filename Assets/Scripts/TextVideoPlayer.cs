using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TextVideoPlayer : MonoBehaviour
{
    [SerializeField] private GlobalOperations globalOperations;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ImageConvertor imageConvertor;
    [SerializeField] private SaveLoad saveLoad;
    [SerializeField] private GameObject videoPopUp;

    private string[] frameArray;
    private int frameNum;
    private int totalFrames;
    private float secBetweenFrames;

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

        imageConvertor.setCornersManual(
            (row:0,col:0),
            (row: gridManager.getRowCount() - 1, col: gridManager.getColCount() - 1)
        );

        foreach(FileInfo file in fileList){
            string fileExtension = file.Name.Split('.')[^1];
            if(fileExtension != "png" && fileExtension != "jpg" && fileExtension != "jpeg"){
                continue;
            }

            imageConvertor.filePathToTexture(file.FullName);
            List<List<char>> outputList = imageConvertor.performConversion();

            saveFile.WriteLine("-----");
            string compString = saveLoad.compressGrid(outputList);
            foreach(string row in compString.Split("\n")){
                saveFile.WriteLine(row);
            }
        }
        saveFile.Close();
    }

    public void playVideo(){
        string filePath = EditorUtility.OpenFilePanel("Choose an converted video text file", "~", "txt");
        if (filePath == ""){
            return;
        }

        globalOperations.closePopUp();

        StreamReader file = new StreamReader(filePath);

        int rowNum = System.Int32.Parse(file.ReadLine().Split(":")[1].Trim());
        int colNum = System.Int32.Parse(file.ReadLine().Split(":")[1].Trim());
        int frameRate = System.Int32.Parse(file.ReadLine().Split(":")[1].Trim());
        secBetweenFrames = 1.0f / frameRate;
        string t = file.ReadLine();

        gridManager.resizeGrid(rowNum, colNum);
        gridManager.constructCachedArray();

        string saveString = file.ReadToEnd();
        file.Close();
        frameArray = saveString.Split("-----\n");
        frameNum = 0;
        totalFrames = frameArray.Length;
        StartCoroutine("handlePlayingVideo");
    }

    private IEnumerator handlePlayingVideo(){
        Debug.Log(secBetweenFrames);
        while (frameNum < totalFrames){
            string frame = frameArray[frameNum];

            List<List<char>> newGrid = saveLoad.decompressSaveStringToGrid(frame);
            for (int row = 0; row < gridManager.getRowCount(); row++){
                for (int col = 0; col < gridManager.getColCount(); col++){
                    gridManager.addToGridArray(row, col, newGrid[row][col]);
                }
            }

            globalOperations.renderUpdate = true;
            frameNum += 1;
            yield return new WaitForSeconds(secBetweenFrames);
        }

    }
}
