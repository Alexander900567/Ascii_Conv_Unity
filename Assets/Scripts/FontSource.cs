using UnityEngine;
using System.Collections.Generic;

public class FontSource : MonoBehaviour
{
    public Texture2D fontTexture;

    private Dictionary<char, (int, int)> charLocations; 
    private Dictionary<(int, int), char> simplifiedCordsToChar;
    private int charHeight = 16;
    private int charWidth = 16;
    private (int, int) noneCords;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        noneCords = (0, 0);
        charLocations = new Dictionary<char, (int, int)>();
        simplifiedCordsToChar = new Dictionary<(int, int), char>();
        translateStringToLocations(
            "! \" # $ % & ' ( ) * " +
            "+ , - . / 0 1 2 3 4 " +
            "5 6 7 8 9 : ; < = > " +
            "? @ A B C D E F G H " +
            "I J K L M N O P Q R " +
            "S T U V W X Y Z [ ] " + 
            "^ _ ` a b c d e f g " +
            "h i j k l m n o p q " +
            "r s t u v w x y z { " +
            "} ~ space \u2588 none \\ |"
        );
    }

    public void translateStringToLocations(string inputString){

        string[] splitString = inputString.Split(' ');
        int textureRows = fontTexture.height / charHeight;
        int textureCols = fontTexture.width / charWidth;

        int col = 0;
        int row = textureRows - 1;
        foreach(string character in splitString){
            //add charater to dict
            if (character == "space"){
                charLocations.Add(' ', getLocation(col, row));
                simplifiedCordsToChar.Add((col, row), ' ');
            }
            else if (character == "none"){
                noneCords = getLocation(col, row);
                simplifiedCordsToChar.Add((col, row), '\uFFF0');
            }
            else{
                charLocations.Add(character.ToCharArray()[0], getLocation(col, row));
                simplifiedCordsToChar.Add((col, row), character.ToCharArray()[0]);
            }
            
            //handle location iteration
            col += 1;
            if (col >= textureCols){
                row -= 1;
                col = 0;
            }
        }

        (int, int) getLocation(int col, int row){
            return (
                col * charWidth,
                row * charHeight
            );
        }
    }    

    public (int, int) getLocationOfChar(char key){
        (int, int) location;
        if (charLocations.TryGetValue(key, out location)) {
            return location;
        }
        else {
            return noneCords;
        }
    }

    public char getCharFromSimpLocation((int, int) location){
        char character;
        if (simplifiedCordsToChar.TryGetValue(location, out character)) {
            return character;
        }
        else {
            return ' ';
        }
    }

    public (int, int) getSimpLocationFromChar(char character){
        foreach((int, int) location in simplifiedCordsToChar.Keys){
            if (simplifiedCordsToChar[location] == character){
                return location;
            }
        }
        return (-1, -1);
    }

    public int getCharHeight(){
        return charHeight;
    }
    public int getCharWidth(){
        return charWidth;
    }
    public int getNumCharHeight(){
        return fontTexture.height / charHeight;
    }
    public int getNumCharWidth(){
        return fontTexture.width / charWidth;
    }
}
