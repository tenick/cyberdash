using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levle1TutorialScript : MonoBehaviour
{

    public Animator level1PanelFadeoutAnimator;

    public characterAnimScript CharacterScript;

    public GameObject Amelia;
    public GameObject Bob;
    public GameObject JumpingExclamation;
    public GameObject ArrowNotif;
    public GameObject NotesBtn;
    public notesUIScript NotesScript;

    public GameObject AmeliaDialog1;
    public GameObject AmeliaDialog2;
    public GameObject AmeliaDialog3;
    public GameObject PlayerDialog1;
    public GameObject AmeliaDialog4;
    public GameObject AmeliaDialog5;

    public GameObject OSIGameObj;

    public List<Action> SceneFlow;

    public int currentFlowIndex;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForSecs(2));

        // setup
        currentFlowIndex = 0;
        CharacterScript.InteractStart += CharacterScript_InteractStart;
        CharacterScript.CurrentInteractableObjects = new() { Amelia };

        // define the scene flow
        SceneFlow = new List<Action>();
        SceneFlow.Add(() => // 0 show amelia 1st dialog
        {
            AmeliaDialog1.SetActive(true);
            CharacterScript.canControl = false;
            AmeliaDialog1.GetComponent<dialogHandlerScript>().Restart();
            AmeliaDialog1.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
        });
        SceneFlow.Add(() => // 1 move exclamation point to bob
        {
            JumpingExclamation.transform.position = new Vector2(-1.73f, 1.38f);
            CharacterScript.canControl = true;
        });
        SceneFlow.Add(() => // 2 show amelia 2nd dialog
        {
            AmeliaDialog2.SetActive(true);
            CharacterScript.canControl = false;
            AmeliaDialog2.GetComponent<dialogHandlerScript>().Restart();
            AmeliaDialog2.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
        });
        SceneFlow.Add(() => // 3 move exclamation point back to Amelia
        {
            JumpingExclamation.transform.position = new Vector2(1.27f, 3.77f);
            CharacterScript.canControl = true;
        });
        SceneFlow.Add(() => // 4 show amelia 3rd dialog
        {
            AmeliaDialog3.SetActive(true);
            CharacterScript.canControl = false;
            AmeliaDialog3.GetComponent<dialogHandlerScript>().Restart();
            AmeliaDialog3.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
        });
        SceneFlow.Add(() => // 5 hide exclamation and show arrow pointing to notes (show notes)
        {
            CharacterScript.canControl = true;
            JumpingExclamation.SetActive(false);
            ArrowNotif.SetActive(true);
            NotesBtn.SetActive(true);
            NotesScript.OnOpen += NotesScript_OnOpen;
            NotesScript.OnClose += NotesScript_OnClose;
        });
        SceneFlow.Add(() => // 6 move arrow to close button
        {
            ArrowNotif.transform.position = new Vector2(6.57f, 2.52f);
            ArrowNotif.transform.Rotate(new Vector3(0, 0, -180));
        });
        SceneFlow.Add(() => // 7
        {
            ArrowNotif.SetActive(false);
            PlayerDialog1.GetComponent<dialogHandlerScript>().Restart();
            PlayerDialog1.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
        });
        SceneFlow.Add(() => // 8
        {
            CharacterScript.canControl = true;
            JumpingExclamation.SetActive(true);
        });
        SceneFlow.Add(() => // 9
        {
            JumpingExclamation.SetActive(true);
            CharacterScript.canControl = false;
            AmeliaDialog4.GetComponent<dialogHandlerScript>().Restart();
            AmeliaDialog4.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
        });
        SceneFlow.Add(() => // 10 move exclamation point to PC2
        {
            CharacterScript.canControl = true;
            JumpingExclamation.transform.position = new Vector2(4.22f, 3.58f);
        });
        SceneFlow.Add(() => // 11 : talk to PC 2
        {
            AmeliaDialog5.GetComponent<dialogHandlerScript>().Restart();
            AmeliaDialog5.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
            OSIGameObj.SetActive(true);

        });
    }

    private void NotesScript_OnOpen(object sender, EventArgs e)
    {
        if (currentFlowIndex == 6)
            Proceed();
    }
    private void NotesScript_OnClose(object sender, EventArgs e)
    {
        if (currentFlowIndex == 7)
            Proceed();
    }

    

    private void CharacterScript_InteractStart(object sender, InteractArgs e)
    {
        if (currentFlowIndex == 0 && e.CollidedGameObj.name == "Amelia")
        {
            Proceed();
        }
        if (currentFlowIndex == 2 && e.CollidedGameObj.name == "Bob")
        {
            Proceed();
        }
        if (currentFlowIndex == 4 && e.CollidedGameObj.name == "Amelia")
        {
            Proceed();
        }
        if (currentFlowIndex == 8)
            Proceed();
        if (currentFlowIndex == 9 && e.CollidedGameObj.name == "Amelia")
            Proceed();
        if (currentFlowIndex == 11 && e.CollidedGameObj.name == "PC2")
            Proceed();
    }

    private void Dialog_DialogFinish(object sender, EventArgs e)
    {
        Proceed();
    }

    void Proceed()
    {
        SceneFlow[currentFlowIndex].Invoke();
        currentFlowIndex++;
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator WaitForSecs(int secs)
    {
        yield return new WaitForSeconds(secs);

        level1PanelFadeoutAnimator.SetTrigger("playFadeout");
    }
}
