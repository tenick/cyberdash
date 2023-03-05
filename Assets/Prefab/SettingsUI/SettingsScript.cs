using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsScript : MonoBehaviour
{
    public sceneTransitionScript transitionScript;

    public void BackToMainMenu()
    {
        transitionScript.TransitionToScene = 1;
        transitionScript.PlayFadeInOnChangeScene = true;
        transitionScript.DoTransition();
    }
    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }
    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}
