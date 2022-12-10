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

    public dialogHandlerScript AmeliaDialog1;
    public dialogHandlerScript AmeliaDialog2;
    public dialogHandlerScript AmeliaDialog3;
    public dialogHandlerScript PlayerDialog1;
    public dialogHandlerScript AmeliaDialog4;
    public dialogHandlerScript AmeliaDialog5;
    public dialogHandlerScript AmeliaDialog6;

    public GameObject OSIGameObj;
    public GameObject WireGameObj;
    public GameObject CyberAttackGameObj;

    public GameObject OSIInstructions;
    public GameObject WireGameInstructions;

    public List<Action> SceneFlow;

    public int currentFlowIndex;

    // win conditions:
    bool osiGameIsCorrect = false;
    bool wireGameIsCorrect = false;

    public Animator LevelCompleteAnimator;

    public sceneTransitionScript sceneTransition;

    // Start is called before the first frame update
    void Start()
    {
        // fadeout the level 1 tutorial panel
        StartCoroutine(WaitForSecs(2, () => { level1PanelFadeoutAnimator.SetTrigger("playFadeout"); }));

        // setup
        currentFlowIndex = 0;
        CharacterScript.InteractStart += CharacterScript_InteractStart;
        CharacterScript.CurrentInteractableObjects = new() { Amelia };
        OSIGameObj.GetComponent<PCScript>().Close += OSIGame_OnClose;
        WireGameObj.GetComponent<WireGameScript>().Close += WireGame_OnClose;

        // define the scene flow
        SceneFlow = new List<Action>();
        SceneFlow.Add(() => // 0 show amelia 1st dialog
        {
            AmeliaDialog1.gameObject.SetActive(true);
            CharacterScript.canControl = false;
            AmeliaDialog1.Restart();
            AmeliaDialog1.DialogFinish += Dialog_DialogFinish;
        });
        SceneFlow.Add(() => // 1 move exclamation point to bob
        {
            JumpingExclamation.transform.position = new Vector2(-1.73f, 1.38f);
            CharacterScript.canControl = true;
        });
        SceneFlow.Add(() => // 2 show amelia 2nd dialog
        {
            AmeliaDialog2.gameObject.SetActive(true);
            CharacterScript.canControl = false;
            AmeliaDialog2.Restart();
            AmeliaDialog2.DialogFinish += Dialog_DialogFinish;
        });
        SceneFlow.Add(() => // 3 move exclamation point back to Amelia
        {
            JumpingExclamation.transform.position = new Vector2(1.27f, 3.77f);
            CharacterScript.canControl = true;
        });
        SceneFlow.Add(() => // 4 show amelia 3rd dialog
        {
            AmeliaDialog3.gameObject.SetActive(true);
            CharacterScript.canControl = false;
            AmeliaDialog3.Restart();
            AmeliaDialog3.DialogFinish += Dialog_DialogFinish;
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
            PlayerDialog1.Restart();
            PlayerDialog1.DialogFinish += Dialog_DialogFinish;
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
            AmeliaDialog4.Restart();
            AmeliaDialog4.DialogFinish += Dialog_DialogFinish;
        });
        SceneFlow.Add(() => // 10 move exclamation point to PC2
        {
            CharacterScript.canControl = true;
            JumpingExclamation.transform.position = new Vector2(-5.74f, 1.36f);
            Instantiate(JumpingExclamation, new Vector3(-1.72f, 1.36f), Quaternion.identity);
        });

    }

    private void WireGame_OnClose(object sender, EventArgs e)
    {
        wireGameIsCorrect = ((WireGameScript)sender).isCorrect;
        CheckIfGameOver();
    }

    private void OSIGame_OnClose(object sender, EventArgs e)
    {
        osiGameIsCorrect = ((PCScript)sender).isCorrect;
        CheckIfGameOver();
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

    void CheckIfGameOver()
    {
        if (osiGameIsCorrect && wireGameIsCorrect)
        {
            LevelCompleteAnimator.SetTrigger("play");
            StartCoroutine(WaitForSecs(1, () => {
                sceneTransition.TransitionToScene = 5;
                sceneTransition.PlayFadeInOnChangeScene = true;
                sceneTransition.DoTransition();
            }));
        }
    }

    bool explainedOSIGame = false;
    bool onOSIDialog = false;
    bool explainedWireGame = false;
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
        if (currentFlowIndex == 11 && e.CollidedGameObj.name == "Alex")
        {
            Debug.Log(e.CollidedGameObj.name);
            if (!explainedOSIGame)
            {
                AmeliaDialog5.Restart();
                AmeliaDialog5.DialogFinish += Dialog_DialogFinish;
            }
            OSIGameObj.SetActive(true);
        }
        if (currentFlowIndex == 11 && e.CollidedGameObj.name == "Bob")
        {
            Debug.Log(e.CollidedGameObj.name);
            if (!explainedWireGame)
            {
                AmeliaDialog6.Restart();
                AmeliaDialog6.DialogFinish += Dialog_DialogFinish;
            }
            WireGameObj.SetActive(true);
            //WireGameObj.GetComponent<WireGameScript>().Reset();
        }
    }

    private void Dialog_DialogFinish(object sender, EventArgs e)
    {
        Debug.Log("explained OSI? : " + explainedOSIGame + " explained Wire Game? : " + explainedWireGame);
        Debug.Log("CurrentFlowIndex: " + currentFlowIndex + " sender? " + sender + " amelia5: " + AmeliaDialog5 + " amelia6: " + AmeliaDialog6);
        Debug.Log((object.ReferenceEquals(sender, AmeliaDialog5)) + " | " + (object.ReferenceEquals(sender, AmeliaDialog6)));
        if (object.ReferenceEquals(sender, AmeliaDialog5) && currentFlowIndex == 11 && !explainedOSIGame)
        {
            Debug.Log("ameliaDialog5 finished");
            OSIInstructions.SetActive(true);
            explainedOSIGame = true;
        }
        if (object.ReferenceEquals(sender, AmeliaDialog6) && currentFlowIndex == 11 && !explainedWireGame)
        {
            Debug.Log("ameliaDialog6 finished");
            WireGameInstructions.SetActive(true);
            explainedWireGame = true;
        }

        if (currentFlowIndex !=  11)
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

    IEnumerator WaitForSecs(int secs, Action runAfter)
    {
        yield return new WaitForSeconds(secs);

        runAfter();
    }
}
