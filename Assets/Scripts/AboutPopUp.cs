using UnityEngine;
using UnityEngine.UI;

public class AboutPopUp : MonoBehaviour
{
    private GlobalOperations global;
    [SerializeField] private Button closeButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        global = GameObject.Find("GlobalOperations").GetComponent<GlobalOperations>();
        closeButton.onClick.AddListener(global.closePopUp);
    }
}
