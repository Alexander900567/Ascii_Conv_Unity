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

    public void decompressGrid(){

    }

    public void loadGridArray(){

    }
}
