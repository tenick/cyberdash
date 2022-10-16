using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainMenuScript : MonoBehaviour
{
    sceneTransitionScript _sceneTransitionScript;
    void Start()
    {
        _sceneTransitionScript = GameObject.Find("SceneTransitionHandler").GetComponent<sceneTransitionScript>();
    }

    // Start is called before the first frame update
    public void NewGame()
    {
        _sceneTransitionScript.DoTransition();
    }
}
