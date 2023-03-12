using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ConfirmationPanelScript : MonoBehaviour
{
    public TextMeshProUGUI DialogTxt;
    string DialogMsg
    {
        get { return $"Are you sure you want to {DialogAction}?"; }
    }
    string DialogAction = "quit";
    bool confirmed = false;
    public bool response = false;


    public void ConfirmBtnClick(int res)
    {
        confirmed = true;
        response = res != 0;
    }

    public IEnumerator Show(string dialogAction, int buildIndex, Action<int> runAfter)
    {
        DialogAction = dialogAction;
        DialogTxt.text = DialogMsg;
        gameObject.SetActive(true);
        yield return new WaitUntil(() => confirmed);
        if (response) runAfter(buildIndex);
        confirmed = false;
        gameObject.SetActive(false);
    }
}
