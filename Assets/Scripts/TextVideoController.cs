using UnityEngine;
using UnityEngine.UI;

public class TextVideoController : MonoBehaviour
{

    [SerializeField] private Button playButton; 
    [SerializeField] private Toggle loopToggle;
    [SerializeField] private Button stopButton;
    private TextVideoPlayer textVidPlay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textVidPlay = GameObject.Find("TextVideoPlayer").GetComponent<TextVideoPlayer>();

        playButton.onClick.AddListener(textVidPlay.togglePlaying);
        stopButton.onClick.AddListener(textVidPlay.ejectFromVideo);
    }

    public void loopWraper(bool toggle){
        textVidPlay.toggleLoop(toggle);
    }
}
