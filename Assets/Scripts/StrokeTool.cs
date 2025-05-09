using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StrokeTool : Tool
{
    [SerializeField] private GameObject strokeWidthSliderObject;
    private Slider strokeWidthSlider = null;
    protected int strokeMin = 1;
    protected int strokeMax = 10;


    public void showStrokeWidthSlider(){
        strokeWidthSliderObject.SetActive(true);
    }
    public void hideStrokeWidthSlider(){
        strokeWidthSliderObject.SetActive(false);
    }
    private Slider getStrokeWidthSlider(){
        if (strokeWidthSlider == null){
            strokeWidthSlider = strokeWidthSliderObject.transform.Find("StrokeWidthSlider").gameObject.GetComponent<Slider>();
        }
        return strokeWidthSlider;
    }

    public int getStrokeWidth() {
        return (int) getStrokeWidthSlider().value;
        
    }
    public void increaseStrokeWidth(int increase){ //These are called by keybinds and buttons and soon by input fields
        setStrokeWidth(Mathf.Min(getStrokeWidth() + increase, strokeMax));
    }
    public void decreaseStrokeWidth(int decrease){
        setStrokeWidth(Mathf.Max(getStrokeWidth() - decrease, strokeMin));
    }
    public void setStrokeWidth(int target){
        if (target >= strokeMin && target <= strokeMax){
            getStrokeWidthSlider().value = target;
        }
    }

    public override void onEnter()
    {
        showStrokeWidthSlider();
    }
    public override void onExit()
    {
        hideStrokeWidthSlider();
    }
}
