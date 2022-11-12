using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dialogHandlerScript : MonoBehaviour
{
    public event EventHandler DialogFinish;

    // not null, will be set everytime Restart() is called
    List<Image> sprites;
    TextMeshProUGUI nameTxt;
    TextMeshProUGUI dialogTxt;
    DialogBase dialog;

    // states
    bool isShowing = false;
    bool isFinished = false;
    int currentDialogIndex = 0;

    
    private void Start()
    {
    }


    public void Restart()
    {
        // show dialog handler
        gameObject.SetActive(true);
        isShowing = true;
        isFinished = false;


        // get sprites
        sprites = new List<Image>();
        GameObject spritesObj = gameObject.transform.GetChild(0).gameObject;
        for (int i = 0; i < spritesObj.transform.childCount; i++)
        {
            sprites.Add(spritesObj.transform.GetChild(i).gameObject.GetComponent<Image>());
            sprites[i].color = new Color(0, 0, 0, 0);
        }

        // get name text and dialog text
        GameObject dialogBoxObj = gameObject.transform.GetChild(2).gameObject;
        nameTxt = dialogBoxObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        dialogTxt = dialogBoxObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        // hide guide text
        GameObject guideTextObj = gameObject.transform.GetChild(3).gameObject;
        guideTextObj.SetActive(false);

        // get script
        GameObject dialogScriptObj = gameObject.transform.GetChild(4).gameObject;
        dialog = dialogScriptObj.GetComponent<DialogBase>();

        currentDialogIndex = 0;

        Next();
    }

    void Update()
    {
        if (isShowing && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))) // left clicked while dialog is showing
        {
            Next();
        }
    }

    void Next()
    {
        if (currentDialogIndex >= dialog.speakers.Count)
        {
            isShowing = false;
            isFinished = true;
            gameObject.SetActive(false);
            DialogFinish?.Invoke(this, EventArgs.Empty);
            return;
        }

        int currentSpeaker = dialog.speakers[currentDialogIndex];
        string currentDialog = dialog.dialogs[currentDialogIndex];
        List<int> currentPresentSpeakers = dialog.presentSpeakers[currentDialogIndex] != "" ? dialog.presentSpeakers[currentDialogIndex].Split(',').Select(x => Convert.ToInt32(x)).ToList() : new List<int>();

        // handle speakers
        for (int i = 0; i < sprites.Count; i++)
        {
            Image sprite = sprites[i];


            // is it a present speaker?
            Color resultingColor = new Color(0, 0, 0, 0);
            if (currentPresentSpeakers.Contains(i))
            {
                resultingColor.a = 1;

                // is it the current speaker?
                if (i == currentSpeaker)
                {
                    resultingColor.r = 1; resultingColor.g = 1; resultingColor.b = 1;
                }
                else
                {
                    resultingColor.r = .5f; resultingColor.g = .5f; resultingColor.b = .5f;
                }
            }

            sprite.color = resultingColor;
        }

        // handle current dialog name and text
        nameTxt.text = sprites[currentSpeaker].gameObject.name;
        dialogTxt.text = currentDialog;


        currentDialogIndex++;
    }

    public bool IsFinished()
    {
        return isFinished;
    }
}
