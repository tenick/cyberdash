using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WireGameScript : MiniGameBase
{
    public static int MinCooldown = 30;
    public static float CDAddVariance = .5f;
    static System.Random rand = new System.Random();

    public override float RandomCD
    {
        get { return MinCooldown * (float)(1 + CDAddVariance * rand.NextDouble()); }
    }

    
    

    public GameObject Sources;
    public GameObject Destinations;
    public Image Wires;
    public TextMeshProUGUI CheckResultText;

    public Sprite RouterPic;
    public Sprite RouterWithSignalPic;

    // templates
    public List<Sprite> Templates;

    public List<Sprite> TutTemplateDests;
    public List<Sprite> Template1Dests;
    public List<Sprite> Template2Dests;

    Dictionary<Sprite, List<Sprite>> TemplateByTemplateDestSprites;
    Dictionary<Sprite, Dictionary<int, int>> TemplateBySourceByDest;

    // Start is called before the first frame update
    void Start()
    {
        TemplateByTemplateDestSprites = new Dictionary<Sprite, List<Sprite>>()
        {
            { Templates[0], TutTemplateDests },
            { Templates[1], Template1Dests },
            { Templates[2], Template2Dests },
        };

        TemplateBySourceByDest = new Dictionary<Sprite, Dictionary<int, int>>()
        {
            {
                Templates[0],
                new Dictionary<int, int>()
                {
                    { 0, 0 },
                    { 1, 2 },
                    { 2, 1 },
                }
            },
            {
                Templates[1], 
                new Dictionary<int, int>() 
                {
                    { 0, 1 }, 
                    { 1, 2 }, 
                    { 2, 0 },
                }
            },
            {
                Templates[2],
                new Dictionary<int, int>()
                {
                    { 0, 1 },
                    { 1, 2 },
                    { 2, 0 },
                }
            },
        };

        Reset();
    }

    public bool isTutorial = false;
    public int routerWSignalIndex = default;
    public void Reset()
    {
        // randomizing the template
        int templateIndex = UnityEngine.Random.Range(0, Templates.Count);
        templateIndex = isTutorial ? 0 : templateIndex;
        Wires.sprite = Templates[templateIndex];
        currentTemplate = Wires.sprite;

        // randomizing the destinations
        routerWSignalIndex = UnityEngine.Random.Range(0, 3);
        routerWSignalIndex = isTutorial ? 0 : routerWSignalIndex;
        for (int i = 0; i < Destinations.transform.childCount; i++)
        {
            Image img = Destinations.transform.GetChild(i).GetComponent<Image>();
            if (i == routerWSignalIndex)
                img.sprite = RouterWithSignalPic;
            else
                img.sprite = RouterPic;
        }
    }

    Sprite currentTemplate;
    int previousSourceSelected = -1;
    public void SelectSource(int sourceNo)
    {
        if (previousSourceSelected != -1)
            Sources.transform.GetChild(previousSourceSelected).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        GameObject selectedSource = Sources.transform.GetChild(sourceNo).gameObject;
        selectedSource.GetComponent<Image>().color = new Color(0, 200f / 255f, 0, 1);
        previousSourceSelected = sourceNo;
    }

    public void CloseBtn()
    {
        gameObject.SetActive(false);
        OnClose();
    }

    public void CheckBtn()
    {
        if (previousSourceSelected != -1)
        {
            Wires.sprite = TemplateByTemplateDestSprites[currentTemplate][TemplateBySourceByDest[currentTemplate][previousSourceSelected]];

            isCorrect = TemplateBySourceByDest[currentTemplate][previousSourceSelected] == routerWSignalIndex;
            if (isCorrect)
            {
                CheckResultText.text = "Correct";

                // TODO: gray out all the buttons (except exit) when correct
            }
            else
            {
                CheckResultText.text = "Wrong";
                Sources.transform.GetChild(previousSourceSelected).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                Reset();
            }

            OnCheck();
        }
    }
}