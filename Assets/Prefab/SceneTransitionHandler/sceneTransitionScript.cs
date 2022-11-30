using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneTransitionScript : MonoBehaviour
{
    Animator animator;

    public bool PlayFadeOutOnSceneLoad;
    public bool PlayFadeInOnChangeScene;
    public int TransitionToScene;
    bool isTransitioning = false;


    // Start is called before the first frame update
    void Start()
    {
        GameObject fadeImgObj = gameObject.transform.GetChild(0).transform.GetChild(0).gameObject;
        animator = fadeImgObj.GetComponent<Animator>();

        if (PlayFadeOutOnSceneLoad)
            animator.SetTrigger("startFadeOut");
    }

    public void DoTransition()
    {
        if (!isTransitioning)
        {
            StartCoroutine(DoTransitionCoroutine());
            isTransitioning = true;
        }
    }

    IEnumerator DoTransitionCoroutine()
    {
        if (PlayFadeInOnChangeScene)
        {
            Debug.Log("IS IT FUCKING FADING IN??? TO SCENE " + TransitionToScene);
            // play transition (if user wants to) and wait for transition to finish
            animator.SetTrigger("startFadeIn");

            yield return new WaitForSeconds(1);
        }
        

        // load scene
        SceneManager.LoadScene(TransitionToScene);
    }
}
