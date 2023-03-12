using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenuScript : MonoBehaviour
{
    sceneTransitionScript _sceneTransitionScript;
    public GameObject ContinuePanel;
    void Start()
    {
        _sceneTransitionScript = GameObject.Find("SceneTransitionHandler").GetComponent<sceneTransitionScript>();
    }

    public void Continue()
    {
        ContinuePanel.SetActive(true);
    }
    public void NewGame()
    {
        _sceneTransitionScript.DoTransition();
    }

    public void Quit()
    {
        Debug.Log("quit!!");
        Application.Quit();
    }
}
