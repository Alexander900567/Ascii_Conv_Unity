using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FindReplacePopUpScript : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button replaceButton;
    [SerializeField] private TMP_InputField findInput;
    [SerializeField] private TMP_InputField replaceInput;
    private FindAndReplace findAndReplace;
    private GridManager gridManager;
    private GlobalOperations global;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        global = GameObject.Find("GlobalOperations").GetComponent<GlobalOperations>();
        findAndReplace = GameObject.Find("FindAndReplace").GetComponent<FindAndReplace>();

        closeButton.onClick.AddListener(global.closePopUp);
        replaceButton.onClick.AddListener(onReplace);
    }

    private void onReplace(){
        char target;
        char replaceWith;

        target = findInput.text[0];
        replaceWith = replaceInput.text[0];

        findAndReplace.replaceCharacters(target, replaceWith);
    }

}
