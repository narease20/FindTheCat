using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInterface : MonoBehaviour
{
    public Button button;
    public Text text;

    [TextArea(3, 5)]
    public string[] newText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateButton()
    {
        if (button.interactable)
        {
            text.enabled = true;
            Debug.Log("Text should appear?");
        }
    }

}
