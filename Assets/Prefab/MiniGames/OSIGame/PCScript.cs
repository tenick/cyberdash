using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public static class ListExtension
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
} 

public class PCScript : MiniGameBase
{
    public static int MinCooldown = 30;
    public static float CDAddVariance = .5f;
    static System.Random rand = new System.Random();

    public override float RandomCD
    {
        get { return MinCooldown * (float)(1 + CDAddVariance * rand.NextDouble()); }
    }

    public GameObject DefaultSlots;
    public GameObject OSISlots;
    public GameObject OSILayers;


    public int GivenNoOfLayers;

    public TextMeshProUGUI CheckResultText;

    void Start()
    {
        // randomize osi layers in default slot
        List<int> osiLayers = Enumerable.Range(0, 7).ToList();
        osiLayers.Shuffle();

        foreach (int layerIndex in osiLayers)
        {
            OSILayerScript oSILayerScript = OSILayers.transform.GetChild(layerIndex).GetComponent<OSILayerScript>();
            oSILayerScript.PCScript = this;
            oSILayerScript.PutToDefaultSlot();
        }

        // add random given layers to OSI slot
        osiLayers.Shuffle();
        osiLayers = osiLayers.Skip(7 - Mathf.Clamp(GivenNoOfLayers, 0, 7)).ToList();
        foreach (int osiLayerIndex in osiLayers)
        {
            // fix osiLayerIndex order based on gameobject heirarchy
            GameObject osiLayer = OSILayers.transform.GetChild(osiLayerIndex).gameObject;
            OSILayerScript oSILayerScript = osiLayer.GetComponent<OSILayerScript>();
            RectTransform oSISlot = OSISlots.transform.GetChild(osiLayerIndex).GetComponent<RectTransform>();

            // set as given
            oSILayerScript.SetAsGiven();

            oSILayerScript.PutToOSISlot(oSISlot);
        }
    }

    void Update()
    {

    }
    public void CloseBtn()
    {
        gameObject.SetActive(false);
        OnClose();
    }
    public void CheckBtn()
    {
        bool result = true;
        for (int i = 0; i < 7; i++)
        {
            OSILayerScript oSILayerScript = OSILayers.transform.GetChild(i).GetComponent<OSILayerScript>();

            if (oSILayerScript.isInOSISlot && oSILayerScript.slotIndex != -1)
            {
                int slotIndex = Convert.ToInt32(oSILayerScript.name[oSILayerScript.name.Length - 1] + "") - 1;
                if (oSILayerScript.slotIndex != slotIndex)
                {
                    result = false;
                    break;
                }
            }
            else
            {
                result = false;
                break;
            }
        }

        isCorrect = result;
        if (result)
            CheckResultText.text = "Correct";
        else
            CheckResultText.text = "Wrong";

        OnCheck();
    }
}
