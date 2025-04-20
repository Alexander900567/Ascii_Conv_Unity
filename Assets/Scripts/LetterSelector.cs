using UnityEngine;
using UnityEngine.UI;

public class LetterSelector : MonoBehaviour
{
    [SerializeField] private GlobalOperations global;
    [SerializeField] private RawImage image;
    [SerializeField] private RectTransform imageTransform;
    [SerializeField] private GameObject fontSource;
    private FontSource fontScript;
    private float selCharHeight;
    private float selCharWidth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fontScript = fontSource.GetComponent<FontSource>();
        updateSelctorTexture();
    }

    public void updateSelctorTexture(){
        image.texture = fontScript.fontTexture;
        selCharHeight = imageTransform.rect.height / fontScript.getNumCharHeight();
        selCharWidth = imageTransform.rect.width / fontScript.getNumCharWidth();
        Debug.Log($"{imageTransform.rect.width} / {fontScript.getNumCharWidth()} = {selCharWidth})");
    }

    private (float x, float y) getMousePosOnImage(){
        Vector3 mousePos = Input.mousePosition;
        float xPos = mousePos.x - imageTransform.position.x; 
        float yPos = mousePos.y - imageTransform.position.y; 
        return (x: xPos, y: yPos);
    }

    public void moveBoxToActiveLetter(){

    }

    public void letterClicked(){   
        (float x, float y) mpos = getMousePosOnImage();
        int xCord = Mathf.Min((int)(mpos.x / selCharWidth), fontScript.getNumCharWidth() - 1);
        int yCord = Mathf.Min((int)(mpos.y / selCharHeight), fontScript.getNumCharHeight() - 1);
        Debug.Log($"({xCord}, {yCord})");

    }
}
