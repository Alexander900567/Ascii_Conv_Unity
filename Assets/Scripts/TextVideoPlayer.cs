using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TextVideoPlayer : MonoBehaviour
{
    [SerializeField] private GlobalOperations globalOperations;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private ImageConvertor imageConvertor;
    [SerializeField] private SaveLoad saveLoad;
    [SerializeField] private GameObject videoPopUp;
    [SerializeField] private GameObject loadingMenuObj;
    [SerializeField] private GameObject videoControlPrefab;

    private GameObject vidContInst = null;
    private bool videoConverting = false;
    private bool videoPlaying = false;
    private bool videoLooping = false;
    private string[] frameArray;
    private int frameNum;
    private int totalFrames;
    private float secBetweenFrames;
    private float frameTimer;

    void Update(){
        if (videoPlaying){
            handlePlayingVideo();
        }
    }

    public void displayVideoPopUp(){
        globalOperations.openPopUp(videoPopUp);
    }

    public void convertVideo(int frameRate){
        if(videoConverting){
            return;
        }
        string filePath = EditorUtility.OpenFolderPanel("Choose a directory of images", "~", "");
        if (filePath == ""){
            return;
        }
        StartCoroutine(processImagesIntoText(frameRate, filePath));
    }

    //ffmpeg -i bad_apple.mp4 -vf fps=30 %04d.png
    public IEnumerator processImagesIntoText(int frameRate, string filePath){
        videoConverting = true;
        globalOperations.controls.Grid.Disable();

        //show loading menu
        GameObject loadingMenu = Instantiate(
            loadingMenuObj,
            new Vector3(-2, -2, 0),
            transform.rotation
        );
        Slider loadingSlider = loadingMenu.transform.Find("LoadingSlider").GetComponent<Slider>();
        RectTransform canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        loadingMenu.transform.SetParent(canvas);


        DirectoryInfo dirObj = new DirectoryInfo(filePath);
        FileInfo[] fileList = dirObj.GetFiles();

        string[] saveFileNameArray = {filePath, "video.txt"};
        StreamWriter saveFile = File.CreateText(Path.Combine(saveFileNameArray));
        saveFile.WriteLine($"rowCount:{gridManager.getRowCount()}");
        saveFile.WriteLine($"colCount:{gridManager.getColCount()}");
        saveFile.WriteLine($"frameRate:{frameRate}");

        //mark the entire canvas as the size of the converted images
        imageConvertor.setCornersManual(
            (row:0,col:0),
            (row: gridManager.getRowCount() - 1, col: gridManager.getColCount() - 1)
        );

        //calculate how much the loading slider should change per image
        float loadingProgressInc = 1f / (float)fileList.Length;

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
            loadingSlider.value += loadingProgressInc;
            yield return null;
        }
        saveFile.Close();

        videoConverting = false;
        globalOperations.controls.Grid.Enable();
        Destroy(loadingMenu);
        yield return null;
    }

    public void playVideo(){
        string filePath = EditorUtility.OpenFilePanel("Choose an converted video text file", "~", "txt");
        if (filePath == ""){
            return;
        }

        globalOperations.closePopUp();
        vidContInst = Instantiate(
            videoControlPrefab,
            new Vector3(0, Screen.height - 1, 0),
            transform.rotation
        );
        RectTransform canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        vidContInst.transform.SetParent(canvas);

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
        totalFrames = frameArray.Length;
        videoPlaying = true;
        resetCurrentVideoToStart();
    }

    private void resetCurrentVideoToStart(){
        frameNum = 0;
        frameTimer = 0;
    }

    private void handlePlayingVideo(){
        frameTimer += Time.deltaTime;
        if (frameTimer < secBetweenFrames){
            return;
        }
        frameTimer = frameTimer - secBetweenFrames;

        string frame = frameArray[frameNum];

        List<List<char>> newGrid = saveLoad.decompressSaveStringToGrid(frame);
        for (int row = 0; row < gridManager.getRowCount(); row++){
            for (int col = 0; col < gridManager.getColCount(); col++){
                gridManager.addToGridArray(row, col, newGrid[row][col]);
            }
        }

        globalOperations.renderUpdate = true;
        frameNum += 1;
        if(frameNum >= totalFrames){
            if(!videoLooping){
                togglePlaying();
                gridManager.clearGrid();
            }
            resetCurrentVideoToStart();
        }
    }

    public void togglePlaying(){
        videoPlaying = !videoPlaying;
        if(vidContInst != null){
            vidContInst.transform.Find("GameObject").Find("PlayButton").Find("PlayImage").gameObject.SetActive(!videoPlaying);
            vidContInst.transform.Find("GameObject").Find("PlayButton").Find("PauseImage").gameObject.SetActive(videoPlaying);
        }
    }

    public void toggleLoop(bool toggle){
        videoLooping = toggle;
    }

    public void ejectFromVideo(){
        if(vidContInst != null){
            Destroy(vidContInst);
            vidContInst = null;
        }
        videoPlaying = false;
        videoLooping = false;
        gridManager.clearGrid();
    }

}
