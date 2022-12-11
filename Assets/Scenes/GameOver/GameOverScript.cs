using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScript : MonoBehaviour
{
    public sceneTransitionScript transitionScript;

    // Start is called before the first frame update
    void Start()
    {
        transitionScript.TransitionToScene = 1;
        // wait for 3 seconds (because animation takes 3 secs)
        StartCoroutine(Wait(2));
    }

    IEnumerator Wait(int secs)
    {
        yield return new WaitForSeconds(secs);

        // transition
        transitionScript.DoTransition();
    }
}
