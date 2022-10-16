using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newGameDialogueHandlerScript : MonoBehaviour
{
    public dialogHandlerScript dialog1;
    public dialogHandlerScript dialog2;
    public sceneTransitionScript transitionScript;
    bool dialog2Started = false;
    // Start is called before the first frame update
    void Start()
    {
        dialog1.Restart();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dialog2Started && dialog1.IsFinished() && !dialog2.IsFinished())
        {
            dialog2Started = true;
            dialog2.Restart();
        }

        if (dialog2Started && dialog2.IsFinished())
            transitionScript.DoTransition();
    }


}
