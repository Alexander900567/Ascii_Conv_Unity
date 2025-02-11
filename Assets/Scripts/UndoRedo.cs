using UnityEngine;
using System.Collections.Generic;

public class UndoRedo : MonoBehaviour
{
    [SerializeField] private GlobalOperations global;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject undoButton;
    [SerializeField] private GameObject redoButton;
    private List<List<(int, int, char)>> undoBuffer;
    private List<List<(int, int, char)>> redoBuffer;
    private int maxUndos = 50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        undoBuffer = new List<List<(int, int, char)>>();
        redoBuffer = new List<List<(int, int, char)>>();
    }

    // Update is called once per frame
    void Update()
    {
        if (global.controls.Grid.PerformUndo.triggered){
            performUndo();
        }
        else if(global.controls.Grid.PerformRedo.triggered){
            performRedo();
        }
    }

    public void performUndo(){
        if (undoBuffer.Count > 0){
            //stash redo
            List<(int, int, char)> redoElement = new List<(int, int, char)>();
            foreach((int, int, char) item in undoBuffer[^1]){
                redoElement.Add((item.Item1, item.Item2, gridManager.getGarrSpace(item.Item1, item.Item2)));
            }
            redoBuffer.Add(redoElement);

            //perform undo
            foreach((int, int, char) item in undoBuffer[^1]){
                gridManager.addToGridArray(item.Item1, item.Item2, item.Item3);
            }
            undoBuffer.RemoveAt(undoBuffer.Count - 1);
            global.renderUpdate = true;
        }
    }

    public void performRedo(){
        if (redoBuffer.Count > 0){
            //stash undo
            List<(int, int, char)> undoElement = new List<(int, int, char)>();
            foreach((int, int, char) item in redoBuffer[^1]){
                undoElement.Add((item.Item1, item.Item2, gridManager.getGarrSpace(item.Item1, item.Item2)));
            }
            undoBuffer.Add(undoElement);

            //perform redo
            foreach((int, int, char) item in redoBuffer[^1]){
                gridManager.addToGridArray(item.Item1, item.Item2, item.Item3);
            }
            redoBuffer.RemoveAt(redoBuffer.Count - 1);
            global.renderUpdate = true;
        }
    }

    public void addUndoFromPbuffer(){
        List<(int, int, char)> undoElement = new List<(int, int, char)>();
        foreach((int, int, char) item in gridManager.getPbuffer()){
            undoElement.Add((item.Item1, item.Item2, gridManager.getGarrSpace(item.Item1, item.Item2)));
        }
        addUndoElement(undoElement);
    }

    public void addUndoElement(List<(int, int, char)> undoElement){
        if (undoBuffer.Count > maxUndos){
            undoBuffer.RemoveAt(0);
        }
        undoBuffer.Add(undoElement);
        redoBuffer = new List<List<(int, int, char)>>();
    }

    public void disableUndoRedo(){
        undoButton.SetActive(false);
        redoButton.SetActive(false);
        global.controls.Grid.PerformUndo.Disable();
        global.controls.Grid.PerformRedo.Disable();
    }
    
    public void enableUndoRedo(){
        undoButton.SetActive(true);
        redoButton.SetActive(true);
        global.controls.Grid.PerformUndo.Enable();
        global.controls.Grid.PerformRedo.Enable();
    }
}
