using Assets.Prefab.LevelGameHandlerScript;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4Script : MonoBehaviour
{
    public Animator level4TitlePanelAnimator;
    public characterAnimScript CharacterScript;

    // dialogs
    public GameObject AmeliaDialog1;
    public GameObject AmeliaDialog2;

    // scene
    public List<Action> SceneFlow;
    public int currentFlowIndex;

    // cyber attacks related
    public instructionsScript DDOSInstructions;
    public Transform CyberAttackParentCanvas;
    public CyberAttackScript CyberAttackPrefab;
    CyberAttackScript MITMACyberAttack;
    CyberAttackScript DNSHijackCyberAttack;
    CyberAttackScript DDOSCyberAttack;
    int correctMessages = 0;
    public AttackBase MITMAAtt;
    public AttackBase DNSHijackAtt;
    public AttackBase DDOSAtt;

    public GameObject JumpingExclamation;


    public LevelGameHandlerScript levelGameHandlerScript;

    void Start()
    {
        // fadeout the level 1 tutorial panel
        StartCoroutine(WaitForSecs(2, () =>
        {
            level4TitlePanelAnimator.SetTrigger("playFadeout");
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
            DDOSInstructions.AllowSpace = false;
            DDOSInstructions.gameObject.SetActive(true);
            DDOSInstructions.Close += DDOSInstructions_Close;

            StartCoroutine(WaitForSecs(2, () =>
            {
                AmeliaDialog2.SetActive(true);
                AmeliaDialog2.GetComponent<dialogHandlerScript>().Restart();
                AmeliaDialog2.GetComponent<dialogHandlerScript>().DialogFinish += Level3Script_DialogFinish;
            }));
        });

        SceneFlow.Add(() => // 1: start game
        {
            // MITMA set cooldown
            MITMAAtt.MinCooldown = 130;
            MITMAAtt.TimeToReachServerInSecs = 120;
            MITMAAtt.CDAddVariance = .33f;

            MITMACyberAttack = Instantiate(CyberAttackPrefab, CyberAttackParentCanvas);
            MITMACyberAttack.CloseBtn();
            MITMACyberAttack.Attack = MITMAAtt;

            Destroy(MITMACyberAttack.transform.GetChild(0).Find("ToolsPanel").Find("MagnifyingGlass").gameObject);
            MITMACyberAttack.MessageReceive += MITMACyberAttack_MessageReceive;

            // DNS hijack
            DNSHijackAtt.MinCooldown = 130;
            DNSHijackAtt.TimeToReachServerInSecs = 120;
            DNSHijackAtt.CDAddVariance = .66f;

            DNSHijackCyberAttack = Instantiate(CyberAttackPrefab, CyberAttackParentCanvas);
            DNSHijackCyberAttack.CloseBtn();
            DNSHijackCyberAttack.Attack = DNSHijackAtt;

            Destroy(DNSHijackCyberAttack.transform.GetChild(0).Find("ToolsPanel").Find("Padlock").gameObject);
            Destroy(DNSHijackCyberAttack.transform.GetChild(0).Find("Hackerdood").gameObject);
            DNSHijackCyberAttack.MessageReceive += DNSHijackCyberAttack_MessageReceive;

            // DDOS
            DDOSAtt.MinCooldown = 130;
            DDOSAtt.TimeToReachServerInSecs = 110;
            DNSHijackAtt.CDAddVariance = .99f;

            DDOSCyberAttack = Instantiate(CyberAttackPrefab, CyberAttackParentCanvas);
            DDOSCyberAttack.CloseBtn();
            DDOSCyberAttack.Attack = DDOSAtt;

            Destroy(DDOSCyberAttack.transform.GetChild(0).Find("ToolsPanel").Find("MagnifyingGlass").gameObject);
            Destroy(DDOSCyberAttack.transform.GetChild(0).Find("ToolsPanel").Find("Padlock").gameObject);
            Destroy(DDOSCyberAttack.transform.GetChild(0).Find("Hackerdood").gameObject);
            DDOSCyberAttack.MessageReceive += DDOSCyberAttack_MessageReceive;

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

    private void DDOSCyberAttack_MessageReceive(object sender, CyberAttackMessageReceiveArgs e)
    {
        if (e.IsMaliciousMessage)
            levelGameHandlerScript.AddHealth(-1);
        else levelGameHandlerScript.AddHealth(1);
    }

    private void DDOSInstructions_Close(object sender, EventArgs e)
    {
        Proceed();
    }

    private void Level3Script_DialogFinish(object sender, EventArgs e)
    {
        if (currentFlowIndex == 0) Proceed();
        else if (currentFlowIndex == 1)
            DDOSInstructions.AllowSpace = true;
    }

    private void CharacterScript_InteractStart(object sender, InteractArgs e)
    {
        if (currentFlowIndex == 2 && e.CollidedGameObj.name == "DNSHijackPC")
            DNSHijackCyberAttack.ShowBtn();
        else if (currentFlowIndex == 2 && e.CollidedGameObj.name == "MITMAPC")
            MITMACyberAttack.ShowBtn();
        else if (currentFlowIndex == 2 && e.CollidedGameObj.name == "DDOSPC")
        {
            JumpingExclamation.SetActive(false);
            DDOSCyberAttack.ShowBtn();
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
