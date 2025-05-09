using UnityEngine;

public class Eraser : StrokeTool
{
    private char lastActiveLetter;
    [SerializeField] private Pencil pencil;

    public override void draw(){
        pencil.draw();
    }

    public override void onEnter(){
        base.onEnter();
        lastActiveLetter = globalOperations.activeLetter;
        globalOperations.activeLetter = ' ';
    }

    public override void onExit(){
        base.onExit();
        globalOperations.activeLetter = lastActiveLetter;
    }
}
