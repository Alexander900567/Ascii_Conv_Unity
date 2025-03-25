using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoPopUpScipt : MonoBehaviour
{
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button convertButton;
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_InputField frameRateInput;
    private TextVideoPlayer videoPlayer;
    private GlobalOperations global;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        videoPlayer = GameObject.Find("TextVideoPlayer").GetComponent<TextVideoPlayer>();
        global = GameObject.Find("GlobalOperations").GetComponent<GlobalOperations>();

        cancelButton.onClick.AddListener(global.closePopUp);
        convertButton.onClick.AddListener(onConvert);
        playButton.onClick.AddListener(onPlay);
    }

    public void onConvert(){
        int frameRate = System.Int32.Parse(frameRateInput.text);

        if (frameRate <= 0){
            return;
        }

        videoPlayer.convertVideo(frameRate);
    }

    public void onPlay(){
        videoPlayer.playVideo();
    }

}
