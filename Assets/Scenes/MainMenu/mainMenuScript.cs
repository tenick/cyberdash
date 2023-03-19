using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenuScript : MonoBehaviour
{
    sceneTransitionScript _sceneTransitionScript;
    public ContinueScript ContinueScript;
    void Start()
    {
        _sceneTransitionScript = GameObject.Find("SceneTransitionHandler").GetComponent<sceneTransitionScript>();
    }

    public void Continue()
    {
        ContinueScript.OpenBtn();
    }
    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        _sceneTransitionScript.DoTransition();
    }

    public void Quit()
    {
        Debug.Log("quit!!");
        Application.Quit();
    }
}
