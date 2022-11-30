using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1_1Script : MonoBehaviour
{
    public Animator level1_1PanelFadeoutAnimator;

    public characterAnimScript CharacterScript;

    public List<Action> SceneFlow;
    public int currentFlowIndex;

    public GameObject AmeliaDialog1;
    public GameObject AmeliaDialog2;
    public GameObject AmeliaDialog3;

    public GameObject SatisfactionExample;
    public GameObject ArrowNotif;
    public Animator ClockAnimator;


    // level settings
    const int shiftHours = 6;
    const float fullLevelTimeInSecs = 180;

    // states
    public int currHour = 0;
    public bool isGameStarted = false;
    public float incrementAmount => fullLevelTimeInSecs / shiftHours;

    // Start is called before the first frame update
    void Start()
    {
        // fadeout the level 1 tutorial panel
        StartCoroutine(WaitForSecs(2, () => 
        { 
            level1_1PanelFadeoutAnimator.SetTrigger("playFadeout");
            StartCoroutine(WaitForSecs(1, () =>
            {
                AmeliaDialog1.SetActive(true);
                CharacterScript.canControl = false;
                AmeliaDialog1.GetComponent<dialogHandlerScript>().Restart();
                AmeliaDialog1.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
            }));
        }));


        // define the scene flow
        SceneFlow = new List<Action>();
        SceneFlow.Add(() => // 0: show arrow and dialog2
        {
            ArrowNotif.SetActive(true);
            StartCoroutine(WaitForSecs(2, () =>
            {
                ArrowNotif.SetActive(false);
                ArrowNotif.transform.position = new Vector2(0.3f, 1.07f);
                AmeliaDialog2.SetActive(true);
                AmeliaDialog2.GetComponent<dialogHandlerScript>().Restart();
                AmeliaDialog2.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
            }));
        });
        SceneFlow.Add(() => // 1: show arrow and dialog3
        {
            ArrowNotif.SetActive(true);
            StartCoroutine(WaitForSecs(2, () =>
            {
                ArrowNotif.SetActive(false);
                AmeliaDialog3.SetActive(true);
                AmeliaDialog3.GetComponent<dialogHandlerScript>().Restart();
                AmeliaDialog3.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
            }));
        });
        SceneFlow.Add(() => // 2: game start
        {
            SatisfactionExample.SetActive(false);
            CharacterScript.canControl = true;
            isGameStarted = true;
            gameTime = Time.time + incrementAmount;
            ClockAnimator.SetTrigger("incr");
        });
    }

    void Proceed()
    {
        SceneFlow[currentFlowIndex].Invoke();
        currentFlowIndex++;
    }

    float gameTime;
    private void Update()
    {
        if (isGameStarted && Time.time > gameTime)
        {
            ClockAnimator.SetTrigger("incr");
            gameTime = Time.time + incrementAmount;
            currHour++;
        }
    }

    private void Dialog_DialogFinish(object sender, EventArgs e)
    {
        Proceed();
    }

    IEnumerator WaitForSecs(int secs, Action runAfter)
    {
        yield return new WaitForSeconds(secs);

        runAfter();
    }
}
