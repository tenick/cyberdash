using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScript : MonoBehaviour
{
    public sceneTransitionScript transitionScript;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait());
        transitionScript.DoTransition();
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(4);
    }

}
