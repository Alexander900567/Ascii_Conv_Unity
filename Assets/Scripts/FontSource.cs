using UnityEngine;
using System.Collections.Generic;

public class FontSource : MonoBehaviour
{
    public Texture2D fontTexture;

    private Dictionary<char, (int, int)> charLocations; 
    private int charHeight;
    private int charWidth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        charHeight = 16;
        charWidth = 16;
        charLocations = new Dictionary<char, (int, int)>();
        charLocations.Add('a', (0, 16));
    }


    public (int, int) getLocationOfChar(char key){
        (int, int) location;
        if (charLocations.TryGetValue(key, out location)) {
            return location;
        }
        else {
            return (0, 0);
        }
    }

    public int getCharHeight(){
        return charHeight;
    }
    public int getCharWidth(){
        return charWidth;
    }
}
