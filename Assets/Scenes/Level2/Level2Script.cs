using Assets.Prefab.LevelGameHandlerScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Level2Script : MonoBehaviour
{
    public Animator level2TitlePanelAnimator;
    public characterAnimScript CharacterScript;

    // dialogs
    public GameObject AmeliaDialog1;
    public GameObject AmeliaDialog2;
    public GameObject AmeliaDialog3;
    public GameObject AmeliaDialog4;
    public GameObject AmeliaDialog5;

    // scene
    public List<Action> SceneFlow;
    public int currentFlowIndex;

    // cyber attacks related
    public instructionsScript DNSThreatInstructions1;
    public instructionsScript DNSThreatInstructions2;
    public Transform CyberAttackParentCanvas;
    public CyberAttackScript CyberAttack;
    CyberAttackScript CyberAttackInstance;
    int correctMessages = 0;
    public AttackBase Attack1;

    public GameObject JumpingExclamation;


    public LevelGameHandlerScript levelGameHandlerScript;

    // Start is called before the first frame update
    void Start()
    {
        // fadeout the level 1 tutorial panel
        StartCoroutine(WaitForSecs(2, () =>
        {
            level2TitlePanelAnimator.SetTrigger("playFadeout");
            StartCoroutine(WaitForSecs(1, () =>
            {
                AmeliaDialog1.SetActive(true);
                CharacterScript.canControl = false;
                CharacterScript.InteractStart += CharacterScript_InteractStart;
                AmeliaDialog1.GetComponent<dialogHandlerScript>().Restart();
                AmeliaDialog1.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
            }));
        }));

        // define scene flow
        SceneFlow = new List<Action>();
        SceneFlow.Add(() => // 0: show exclam point to server
        {
            CharacterScript.canControl = true;
            JumpingExclamation.SetActive(true);
        });

        SceneFlow.Add(() => // 1: dialog 2
        {
            AmeliaDialog2.SetActive(true);
            CharacterScript.canControl = false;
            AmeliaDialog2.GetComponent<dialogHandlerScript>().Restart();
            AmeliaDialog2.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
        });

        SceneFlow.Add(() => // 2: show DNS threat instructions and show dialog 3
        {
            DNSThreatInstructions1.AllowSpace = false;
            DNSThreatInstructions1.gameObject.SetActive(true);
            DNSThreatInstructions1.Close += DNSThreatInstructions_Close;

            StartCoroutine(WaitForSecs(2, () =>
            {
                AmeliaDialog3.SetActive(true);
                AmeliaDialog3.GetComponent<dialogHandlerScript>().Restart();
                AmeliaDialog3.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
            }));
        });

        SceneFlow.Add(() => // 3: show instruction 2
        {
            DNSThreatInstructions2.AllowSpace = false;
            DNSThreatInstructions2.gameObject.SetActive(true);
            DNSThreatInstructions2.Close += DNSThreatInstructions_Close;

            StartCoroutine(WaitForSecs(2, () =>
            {
                AmeliaDialog4.SetActive(true);
                AmeliaDialog4.GetComponent<dialogHandlerScript>().Restart();
                AmeliaDialog4.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
            }));
        });

        SceneFlow.Add(() => // 4: show cyber attack game
        {
            // set cooldown
            Attack1.MinCooldown = 20;
            Attack1.TimeToReachServerInSecs = 25;
            CyberAttack.Attack = Attack1;

            CyberAttackInstance = Instantiate(CyberAttack, CyberAttackParentCanvas);

            Destroy(CyberAttackInstance.transform.GetChild(0).Find("XBtn").gameObject);
            Destroy(CyberAttackInstance.transform.GetChild(0).Find("Hackerdood").gameObject);
            Destroy(CyberAttackInstance.transform.GetChild(0).Find("ToolsPanel").Find("Padlock").gameObject);

            CyberAttackInstance.MessageReceive += TutorialCyberAttackInstance_MessageReceive;
            CharacterScript.canControl = false;
            JumpingExclamation.SetActive(false);
        });

        SceneFlow.Add(() => // 5: Delete cyberattackgame and show the final dialog
        {
            Destroy(CyberAttackInstance.gameObject);

            AmeliaDialog5.SetActive(true);
            AmeliaDialog5.GetComponent<dialogHandlerScript>().Restart();
            AmeliaDialog5.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
        });

    }

    private void TutorialCyberAttackInstance_MessageReceive(object sender, CyberAttackMessageReceiveArgs e)
    {
        if (!e.IsMaliciousMessage)
        {
            correctMessages++;
            if (correctMessages == 3)
                Proceed();
        }
    }

    private void CyberAttackInstance_MessageReceive(object sender, CyberAttackMessageReceiveArgs e)
    {
        if (e.IsMaliciousMessage)
            levelGameHandlerScript.AddHealth(-1);
        else levelGameHandlerScript.AddHealth(1);
    }

    void Proceed()
    {
        SceneFlow[currentFlowIndex].Invoke();
        currentFlowIndex++;
    }

    private void DNSThreatInstructions_Close(object sender, EventArgs e)
    {
        if (currentFlowIndex == 3)
            Proceed();
        else if (currentFlowIndex == 4)
            Proceed();
    }

    private void CharacterScript_InteractStart(object sender, InteractArgs e)
    {
        if (currentFlowIndex == 1 && e.CollidedGameObj.name == "PC1")
            Proceed();
        else if (currentFlowIndex == 6 && e.CollidedGameObj.name == "PC1")
        {
            CyberAttackInstance.ShowBtn();
        }
    }

    private void Dialog_DialogFinish(object sender, EventArgs e)
    {
        if (currentFlowIndex == 0)
            Proceed();
        else if (currentFlowIndex == 2)
            Proceed();
        else if (currentFlowIndex == 3)
            DNSThreatInstructions1.AllowSpace = true;
        else if (currentFlowIndex == 4)
            DNSThreatInstructions2.AllowSpace = true;
        else if (currentFlowIndex == 6) // game start
        {
            // set cooldown
            Attack1.MinCooldown = 35;
            Attack1.TimeToReachServerInSecs = 50;
            CyberAttack.Attack = Attack1;

            CyberAttackInstance = Instantiate(CyberAttack, CyberAttackParentCanvas);
            Destroy(CyberAttackInstance.transform.GetChild(0).Find("Hackerdood").gameObject);
            Destroy(CyberAttackInstance.transform.GetChild(0).Find("ToolsPanel").Find("Padlock").gameObject);
            CyberAttackInstance.MessageReceive += CyberAttackInstance_MessageReceive;
            CyberAttackInstance.gameObject.SetActive(true);
            CyberAttackInstance.CloseBtn();

            levelGameHandlerScript.fullLevelTimeInSecs = 180;

            levelGameHandlerScript.gameObject.SetActive(true);
            levelGameHandlerScript.StartGame();
        }
    }

    IEnumerator WaitForSecs(int secs, Action runAfter)
    {
        yield return new WaitForSeconds(secs);

        runAfter();
    }
}
