using UnityEngine;
using UnityEngine.UI;

public class TextVideoController : MonoBehaviour
{

    [SerializeField] private Button playButton; 
    [SerializeField] private Toggle loopToggle;
    [SerializeField] private Button stopButton;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private RectTransform sliderBack;
    private TextVideoPlayer textVidPlay;
    private bool wentDown = false;
    private bool wasVideoPlaying;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textVidPlay = GameObject.Find("TextVideoPlayer").GetComponent<TextVideoPlayer>();

        playButton.onClick.AddListener(textVidPlay.togglePlaying);
        stopButton.onClick.AddListener(textVidPlay.ejectFromVideo);
        progressSlider.onValueChanged.AddListener((float num) => {textVidPlay.changeFrame((int)num);});
        progressSlider.maxValue = textVidPlay.getTotalFrames() - 1;
    }

    void Update(){
        if (!wentDown && Input.GetMouseButtonDown(0)){
            if(RectTransformUtility.RectangleContainsScreenPoint(sliderBack, Input.mousePosition, null)){
                wasVideoPlaying = textVidPlay.isVideoPlaying();
                textVidPlay.hiddenPauseVideo();
                wentDown = true;
            }
        }
        else if (wentDown && Input.GetMouseButtonUp(0)){
            if(wasVideoPlaying){
                textVidPlay.hiddenStartVideo();
            }
            wentDown = false;
        }
    }

    public void loopWraper(bool toggle){
        textVidPlay.toggleLoop(toggle);
    }
}
