using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levle1TutorialScript : MonoBehaviour
{

    public Animator level1PanelFadeoutAnimator;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForSecs(2));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForSecs(int secs)
    {
        yield return new WaitForSeconds(secs);

        level1PanelFadeoutAnimator.SetTrigger("playFadeout");
    }
}
