using UnityEngine;
using UnityEditor;
using System.IO;

public class ImageConvertor : Tool
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private GameObject chooseImageButton;
    [SerializeField] private GameObject outline;
    private Texture2D image;




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
