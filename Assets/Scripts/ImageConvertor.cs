using UnityEngine;

public class ImageConvertor : Tool
{
    [SerializeField] ComputeShader computeShader;
    [SerializeField] GameObject chooseImageButton;





    public override void onEnter(){
        chooseImageButton.SetActive(true);
    }
    public override void onExit(){
        chooseImageButton.SetActive(false);
    }



}
