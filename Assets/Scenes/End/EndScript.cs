using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScript : MonoBehaviour
{
    public sceneTransitionScript transitionScript;

    // dialogs
    public GameObject AmeliaDialog1;

    void Start()
    {
        // fadeout the level 1 tutorial panel
        StartCoroutine(WaitForSecs(3, () =>
        {
            AmeliaDialog1.SetActive(true);
            AmeliaDialog1.GetComponent<dialogHandlerScript>().Restart();
            AmeliaDialog1.GetComponent<dialogHandlerScript>().DialogFinish += EndScript_DialogFinish; ;
            
        }));
    }

    private void EndScript_DialogFinish(object sender, EventArgs e)
    {
        transitionScript.DoTransition();
    }

    IEnumerator WaitForSecs(int secs, Action runAfter)
    {
        yield return new WaitForSeconds(secs);

        runAfter();
    }
}
