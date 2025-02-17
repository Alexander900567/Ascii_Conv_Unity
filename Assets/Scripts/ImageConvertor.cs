using UnityEngine;
using UnityEditor;
using System.IO;

public class ImageConvertor : Tool
{
    [SerializeField] ComputeShader computeShader;
    [SerializeField] GameObject chooseImageButton;
    Texture2D image;




    public override void onEnter(){
        chooseImageButton.SetActive(true);
    }
    public override void onExit(){
        chooseImageButton.SetActive(false);
    }


    public void uploadImage(){
        string filePath = EditorUtility.OpenFilePanel("Choose an image", "~", "png");
        if (filePath == ""){
            return;
        }
        byte[] imageByteArray = File.ReadAllBytes(filePath);
        ImageConversion.LoadImage(image, imageByteArray, false);

    

    }



}
