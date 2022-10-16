using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logoSceneScript : MonoBehaviour
{
    public Animator animator;
    public sceneTransitionScript _sceneTransitionScript;

    // Start is called before the first frame update
    void Start()
    {
        animator.SetTrigger("playLogoAnimation");

        // wait for 3 seconds (because animation takes 3 secs)
        StartCoroutine(Wait(3));

        
    }

    IEnumerator Wait(int secs)
    {
        yield return new WaitForSeconds(secs);

        // transition
        _sceneTransitionScript.DoTransition();
    }
}
