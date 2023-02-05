using Assets.Prefab.LevelGameHandlerScript;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Script : MonoBehaviour
{
    public Animator level3TitlePanelAnimator;
    public characterAnimScript CharacterScript;

    // dialogs
    public GameObject AmeliaDialog1;
    public GameObject AmeliaDialog2;

    // scene
    public List<Action> SceneFlow;
    public int currentFlowIndex;

    // cyber attacks related
    public instructionsScript MITMAInstructions;
    public Transform CyberAttackParentCanvas;
    public CyberAttackScript CyberAttackPrefab;
    CyberAttackScript MITMACyberAttack;
    CyberAttackScript DNSHijackCyberAttack;
    int correctMessages = 0;
    public AttackBase MITMAAtt;
    public AttackBase DNSHijackAtt;

    public GameObject JumpingExclamation;


    public LevelGameHandlerScript levelGameHandlerScript;

    void Start()
    {
        notesUIScript.end = 12;
        // fadeout the level 1 tutorial panel
        StartCoroutine(WaitForSecs(2, () =>
        {
            level3TitlePanelAnimator.SetTrigger("playFadeout");
            StartCoroutine(WaitForSecs(1, () =>
            {
                AmeliaDialog1.SetActive(true);
                CharacterScript.canControl = false;
                CharacterScript.InteractStart += CharacterScript_InteractStart;
                AmeliaDialog1.GetComponent<dialogHandlerScript>().Restart();
                AmeliaDialog1.GetComponent<dialogHandlerScript>().DialogFinish += Level3Script_DialogFinish;
            }));
        }));

        // define scene flow
        SceneFlow = new List<Action>();
        SceneFlow.Add(() => // 0: show instructions
        {
            MITMAInstructions.gameObject.SetActive(true);
            MITMAInstructions.Close += MITMAInstructions_Close;
        });

        SceneFlow.Add(() => // 1: show demo game
        {
            // set cooldown
            MITMAAtt.MinCooldown = 30;
            MITMAAtt.TimeToReachServerInSecs = 20;

            MITMACyberAttack = Instantiate(CyberAttackPrefab, CyberAttackParentCanvas);
            MITMACyberAttack.Attack = MITMAAtt;

            Destroy(MITMACyberAttack.transform.GetChild(0).Find("XBtn").gameObject);
            Destroy(MITMACyberAttack.transform.GetChild(0).Find("ToolsPanel").Find("MagnifyingGlass").gameObject);

            MITMACyberAttack.MessageReceive += TutorialMITMACyberAttack_MessageReceive;
            notesUIScript.end = 14;
        });

        SceneFlow.Add(() => // 2: Delete MITMA cyberattackgame and show the final dialog
        {
            Destroy(MITMACyberAttack.gameObject);

            AmeliaDialog2.SetActive(true);
            AmeliaDialog2.GetComponent<dialogHandlerScript>().Restart();
            AmeliaDialog2.GetComponent<dialogHandlerScript>().DialogFinish += Level3Script_DialogFinish;
        });

        SceneFlow.Add(() => // 3: start game
        {
            // MITMA set cooldown
            MITMAAtt.MinCooldown = 110;
            MITMAAtt.TimeToReachServerInSecs = 100;

            MITMACyberAttack = Instantiate(CyberAttackPrefab, CyberAttackParentCanvas);
            MITMACyberAttack.CloseBtn();
            MITMACyberAttack.Attack = MITMAAtt;

            Destroy(MITMACyberAttack.transform.GetChild(0).Find("ToolsPanel").Find("MagnifyingGlass").gameObject);
            MITMACyberAttack.MessageReceive += MITMACyberAttack_MessageReceive;

            // DNS hijack
            DNSHijackAtt.MinCooldown = 100;
            DNSHijackAtt.TimeToReachServerInSecs = 80;

            DNSHijackCyberAttack = Instantiate(CyberAttackPrefab, CyberAttackParentCanvas);
            DNSHijackCyberAttack.CloseBtn();
            DNSHijackCyberAttack.Attack = DNSHijackAtt;

            Destroy(DNSHijackCyberAttack.transform.GetChild(0).Find("ToolsPanel").Find("Padlock").gameObject);
            Destroy(DNSHijackCyberAttack.transform.GetChild(0).Find("Hackerdood").gameObject);
            DNSHijackCyberAttack.MessageReceive += DNSHijackCyberAttack_MessageReceive;

            CharacterScript.canControl = true;

            levelGameHandlerScript.fullLevelTimeInSecs = 180;
            levelGameHandlerScript.gameObject.SetActive(true);
            levelGameHandlerScript.StartGame();
        });
    }

    private void MITMACyberAttack_MessageReceive(object sender, CyberAttackMessageReceiveArgs e)
    {
        if (e.IsMaliciousMessage)
            levelGameHandlerScript.AddHealth(-1);
        else levelGameHandlerScript.AddHealth(1);
    }

    private void DNSHijackCyberAttack_MessageReceive(object sender, CyberAttackMessageReceiveArgs e)
    {
        if (e.IsMaliciousMessage)
            levelGameHandlerScript.AddHealth(-1);
        else levelGameHandlerScript.AddHealth(1);
    }

    private void TutorialMITMACyberAttack_MessageReceive(object sender, CyberAttackMessageReceiveArgs e)
    {
        if (!e.IsMaliciousMessage)
        {
            correctMessages++;
            if (correctMessages == 3)
                Proceed();
        }
    }

    private void MITMAInstructions_Close(object sender, EventArgs e)
    {
        Proceed();
    }

    private void Level3Script_DialogFinish(object sender, EventArgs e)
    {
        Proceed();
    }

    private void CharacterScript_InteractStart(object sender, InteractArgs e)
    {
        if (currentFlowIndex == 4 && e.CollidedGameObj.name == "DNSHijackPC")
            DNSHijackCyberAttack.ShowBtn();
        else if (currentFlowIndex == 4 && e.CollidedGameObj.name == "MITMAPC")
        {
            JumpingExclamation.SetActive(false);
            MITMACyberAttack.ShowBtn();
        }
    }

    void Proceed()
    {
        SceneFlow[currentFlowIndex].Invoke();
        currentFlowIndex++;
    }

    IEnumerator WaitForSecs(int secs, Action runAfter)
    {
        yield return new WaitForSeconds(secs);

        runAfter();
    }
}
