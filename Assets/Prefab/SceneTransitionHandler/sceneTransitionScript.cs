using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneTransitionScript : MonoBehaviour
{
    Animator animator;

    public bool PlayFadeOutOnSceneLoad;
    public bool PlayFadeInOnChangeScene;
    public int TransitionToScene;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>();

        if (PlayFadeOutOnSceneLoad)
            animator.SetTrigger("startFadeOut");
    }

    public void DoTransition()
    {
        StartCoroutine(DoTransitionCoroutine());
    }

    IEnumerator DoTransitionCoroutine()
    {
        if (PlayFadeInOnChangeScene)
        {
            // play transition (if user wants to) and wait for transition to finish
            animator.SetTrigger("startFadeIn");

            yield return new WaitForSeconds(1);
        }
        

        // load scene
        SceneManager.LoadScene(TransitionToScene);
    }
}
