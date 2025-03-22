using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class SaveLoad : MonoBehaviour
{
    [SerializeField] GridManager gridManager;
    [SerializeField] GlobalOperations globalOp;

    // Update is called once per frame
    void Update()
    {
        
    }
    public string compressGrid(List<List<char>> grid){
        string compString = "";

        foreach(List<char> row in grid){
            int sameCount = 0;
            char currChar = row[0];
            foreach(char space in row){
                if(currChar == space){
                    sameCount += 1;
                }
                else{
                    compString += $"{sameCount}{currChar}\t";
                    sameCount = 1;
                    currChar = space;
                }
            }
            compString += $"{sameCount}{currChar}\n";
        }

        compString = compString.Remove(compString.Length - 1, 1);
        return compString;
    }

    public void saveGridArray(){
        string filePath = EditorUtility.SaveFilePanel("Save grid as txt", "~", "grid", "txt");
        if (filePath == ""){
            return;
        }
        StreamWriter file = File.CreateText(filePath);

        file.WriteLine($"rowCount:{gridManager.getRowCount()}");
        file.WriteLine($"colCount:{gridManager.getColCount()}");
        string compString = compressGrid(gridManager.getGridArray());
        foreach(string row in compString.Split("\n")){
            file.WriteLine(row);
        }
        file.Close();
    }

    public List<List<char>> decompressSaveStringToGrid(string saveString){
        List<List<char>> newGrid = new List<List<char>>();
        char letter = ' ';
        int numLetters = 0;
        foreach(string row in saveString.Split("\n")){
            if (row == "") { continue; }
            newGrid.Add(new List<char>());
            foreach(string node in row.Split("\t")){
                letter = node[^1];
                numLetters = System.Int32.Parse(node.Substring(0, node.Length - 1));
                for(int col = 0; col < numLetters; col++){
                    newGrid[^1].Add(letter);
                }
            }
        }
        return newGrid;
    }

    public void loadGridArray(){
        string filePath = EditorUtility.OpenFilePanel("Load a grid save file", "~", "txt");
        if (filePath == ""){
            return;
        }
        StreamReader file = new StreamReader(filePath);

        int rowNum = System.Int32.Parse(file.ReadLine().Split(":")[1].Trim());
        int colNum = System.Int32.Parse(file.ReadLine().Split(":")[1].Trim());

        gridManager.resizeGrid(rowNum, colNum);

        string saveString = file.ReadToEnd();
        file.Close();

        List<List<char>> newGrid = decompressSaveStringToGrid(saveString);
        for (int row = 0; row < rowNum; row++){
            for (int col = 0; col < colNum; col++){
                gridManager.addToGridArray(row, col, newGrid[row][col]);
            }
        }
        globalOp.renderUpdate = true;
        gridManager.constructCachedArray();
    }
}
