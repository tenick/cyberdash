using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WireGameScript : MonoBehaviour
{
    public GameObject Sources;
    public GameObject Destinations;
    public Image Wires;

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
                    { 2, 1 },
                }
            },
        };

        Reset();
    }

    public bool isTutorial = false;

    public void Reset()
    {
        // randomizing the template
        int templateIndex = Random.Range(0, Templates.Count);
        templateIndex = isTutorial ? 0 : templateIndex;
        Wires.sprite = Templates[templateIndex];
        currentTemplate = Wires.sprite;

        // randomizing the destinations
        int routerWSignalIndex = Random.Range(0, 3);
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
    public void SelectSource(int sourceNo)
    {
        Wires.sprite = TemplateByTemplateDestSprites[currentTemplate][TemplateBySourceByDest[currentTemplate][sourceNo]];
    }

    public void CloseBtn()
    {
        gameObject.SetActive(false);
    }

    public void CheckBtn()
    {
        Reset();
    }
}
