using UnityEngine;

public class Eraser : Tool
{
    private char lastActiveLetter;
    [SerializeField] private Pencil pencil;

    public override void draw(){
        pencil.draw();
    }

    public override void onEnter(){
        lastActiveLetter = globalOperations.activeLetter;
        globalOperations.activeLetter = ' ';
    }

    public override void onExit(){
        globalOperations.activeLetter = lastActiveLetter;
    }
}
