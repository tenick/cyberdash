using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class comicsStripScript : MonoBehaviour
{
    sceneTransitionScript _sceneTransitionScript;

    public GameObject comicStripContainer;
    int noOfStrips;
    int currentStrip = 0;

    // Start is called before the first frame update
    void Start()
    {
        _sceneTransitionScript = GameObject.Find("SceneTransitionHandler").GetComponent<sceneTransitionScript>();
        noOfStrips = comicStripContainer.transform.childCount;

        //comicStripAnimator.SetTrigger("startNext");
    }


    public void NewGame()
    {
        _sceneTransitionScript.DoTransition();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentStrip <= noOfStrips)
        {
            // change scene if exceeded no. of strips
            if (currentStrip == noOfStrips)
            {
                _sceneTransitionScript.DoTransition();
                return;
            }

            GameObject comicStripObj = comicStripContainer.transform.GetChild(currentStrip).gameObject;
            Debug.Log("reaching here???? " + currentStrip + " | " + noOfStrips + " | " + comicStripObj.name);
            Animator comicStripAnimator = comicStripObj.GetComponent<Animator>();

            comicStripAnimator.SetTrigger("startNext");

            currentStrip++;

            
        }
    }

    IEnumerator Wait(int secs)
    {
        yield return new WaitForSeconds(secs);
    }
}
