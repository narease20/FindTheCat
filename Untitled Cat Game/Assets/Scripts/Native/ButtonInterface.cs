﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonInterface : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI text;
    public BoxCollider inputBox;

    [TextArea(3, 5)]
    public string[] newText;

    private Queue<string> sentences;

    public int timesInteracted = 0;

    private void Awake()
    {
        inputBox = this.GetComponent<BoxCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartText()
    {
        sentences.Clear();

        for(int i = 0; i < newText.Length; ++i)
        {
            sentences.Enqueue(newText[i]);
        }
        //Next();
    }

    public void Next()
    {
        if(sentences.Count == 0)
        {
            return;
        }
        string sentence = sentences.Dequeue();
        // Isaiah, the disabler is here!
        //inputBox.enabled = false;
        StopAllCoroutines();
        StartCoroutine(DisplaySentence(sentence));
    }

    IEnumerator DisplaySentence(string sentence)
    {
        text.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            text.text += letter;
            yield return null;
        }
        // Isaiah, the inabler is here!
        //inputBox.enabled = true;
    }

    public void Clear()
    {
        text.text = "";
    }

    public void ActivateButton()
    {
        if (button.interactable)
        {
            Debug.Log("Text should appear?");
            if(timesInteracted == 0)
            {
                if (!text.enabled)
                {
                    text.enabled = true;
                }
                StartText();
                timesInteracted++;
            }
            
            if(timesInteracted > 0)
            {
                Clear();
                Next();
                timesInteracted++;
            }
            
        }
    }

}
