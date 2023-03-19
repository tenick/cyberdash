using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueScript : MonoBehaviour
{
    public sceneTransitionScript _sceneTransitionScript;

    public List<Image> LevelImgs;
    public List<Sprite> LevelsDoneSprites;
    public List<Sprite> LevelsLockedSprites;
    public void OpenBtn()
    {
        for (int i = 0; i < 4; i++)
        {
            bool currLvlIsDone = PlayerPrefs.GetInt("level"+(i+1), 0) != 0;
            if (currLvlIsDone)
            {
                LevelImgs[i].sprite = LevelsDoneSprites[i];
                LevelImgs[i].gameObject.GetComponent<Selectable>().enabled = true;
            }
            else
            {
                LevelImgs[i].sprite = LevelsLockedSprites[i];
                LevelImgs[i].gameObject.GetComponent<Selectable>().enabled = false;
            }
        }
        gameObject.SetActive(true);
    }
    public void CloseBtn()
    {
        gameObject.SetActive(false);
    }

    public void LevelSelect(int buildIndex)
    {
        _sceneTransitionScript.TransitionToScene = 5 + buildIndex;
        _sceneTransitionScript.DoTransition();
    }
}
