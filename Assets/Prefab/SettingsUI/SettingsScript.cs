using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsScript : MonoBehaviour
{
    public sceneTransitionScript transitionScript;
    public ConfirmationPanelScript confirmationPanelScript;
    public void RetryLevel()
    {
        ShowModal("Retry", SceneManager.GetActiveScene().buildIndex, ChangeScene);
    }
    public void BackToMainMenu()
    {
        ShowModal("Quit", 1, ChangeScene);
    }
    void ChangeScene(int buildIndex)
    {
        transitionScript.TransitionToScene = buildIndex;
        transitionScript.PlayFadeInOnChangeScene = true;
        transitionScript.DoTransition();
    }

    void ShowModal(string modalAction, int buildIndex, Action<int> runAfter)
    {
        StartCoroutine(confirmationPanelScript.Show(modalAction, buildIndex, runAfter));
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
