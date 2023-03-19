using Assets.Prefab.LevelGameHandlerScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Level1_1Script : MonoBehaviour
{
    public Animator level1_1PanelFadeoutAnimator;

    public characterAnimScript CharacterScript;

    public List<Action> SceneFlow;
    public int currentFlowIndex = 1;

    public GameObject AmeliaDialog1;
    public GameObject AmeliaDialog2;
    public GameObject AmeliaDialog3;

    public GameObject ArrowNotif;

    public LevelGameHandlerScript levelGameHandlerScript;

    // Start is called before the first frame update
    void Start()
    {
        levelGameHandlerScript.ClearTasksText.text = $"Clear {levelGameHandlerScript.MinigamesClearAmountToWin - levelGameHandlerScript.CurrentMinigameClearAmount} Tasks";
        Destroy(levelGameHandlerScript.PreventThreatsText.gameObject);
        notesUIScript.end = 9;
        PlayerPrefs.SetInt("level1", 1);

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
                ArrowNotif.transform.position = new Vector2(-0.5f, 3.73f);
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
            levelGameHandlerScript.fullLevelTimeInSecs = 90;
            Debug.Log("umabot din ba dito?");
            levelGameHandlerScript.StartGame();
        });
    }

    void Proceed()
    {
        SceneFlow[currentFlowIndex].Invoke();
        currentFlowIndex++;
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
