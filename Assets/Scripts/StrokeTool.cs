using UnityEngine;

public class StrokeTool : Tool
{
    [SerializeField] private GameObject strokeWidthSlider;
    private static int strokeWidth = 1;
    private static int strokeMin = 1;
    private static int strokeMax = 100;

    public static int getStrokeWidth() {
        return strokeWidth;
    }
    public static void increaseStrokeWidth(int increase){ //These are called by keybinds and buttons and soon by input fields
        if (strokeWidth + increase <= strokeMax){ //If less than or equal to upper bound for strokeWidth
            strokeWidth += increase; //Increase as usual
        }
        else if (strokeWidth + increase > strokeMax){ //If goes over
            strokeWidth = strokeMax; //Set to max
        }
    }
    public static void decreaseStrokeWidth(int decrease){
        if (strokeWidth - decrease >= strokeMin){ //If less than or equal to lower bound
            strokeWidth -= decrease; //Decrease as usual
        }
        else if (strokeWidth - decrease < strokeMin){ //If goes under
            strokeWidth = strokeMin; //Set to min
        }
    }
    public static void setStrokeWidth(int target){
        if (target >= strokeMin && target <= strokeMax){
            strokeWidth = target;
        }
    }
}
