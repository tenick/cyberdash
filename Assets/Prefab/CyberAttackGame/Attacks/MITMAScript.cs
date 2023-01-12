using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MITMAScript : AttackBase
{
    List<string> wordList = new()
    {
        "google.com",
        "yahoo.com",
        "facebook.com",
        "twitter.com",
        "youtube.com",
        "router",
        "server",
        "network",
        "cybersecurity",
        "traceroute",
        "ping",
        "ip address",
        "topology",
        "switch"
    };

    public TextMeshProUGUI MessageText;
    public TMP_InputField InputField;
    public GameObject Padlock;
    public GameObject Virus;


    // win conditions
    string lastEnteredText = "";
    bool isVirused = false;
    bool isEncrypted = false;
    bool entered = false;

    void Start()
    {
        MessageText.text = wordList.ElementAt(UnityEngine.Random.Range(0, wordList.Count));

        cyberAttackScript = gameObject.transform.parent.parent.parent.GetComponent<CyberAttackScript>();
        started = true;
        ServerPosition = gameObject.transform.parent.parent.Find("ServerPosition").GetComponent<RectTransform>().anchoredPosition;
        startTime = Time.time;
        endTime = startTime + TimeToReachServerInSecs;
        dnsHijackRectTransform = transform.GetComponent<RectTransform>();
        dnsHijackInitPosition = dnsHijackRectTransform.anchoredPosition;

        // events
        GetComponent<Button>().onClick.AddListener(OnClick_MITMA);
        InputField.onSubmit.AddListener(delegate { OnSubmit_MITMA(); });
    }

    void OnClick_MITMA()
    {
        if (!isEncrypted && !isVirused && cyberAttackScript.currToolState == ToolState.Padlock)
        {
            Padlock.SetActive(true);
            isEncrypted = true;
        }
    }

    void OnSubmit_MITMA()
    {
        if (!entered)
        {
            lastEnteredText = InputField.text;
            Destroy(InputField);
            entered = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            if (!isVirused && Time.time > startTime + TimeToReachServerInSecs * .75 && (!isEncrypted || !entered || MessageText.text.Trim() != lastEnteredText.Trim()))
            {
                isVirused = true;
                Virus.SetActive(true);
            }

            if (Time.time > endTime)
            {
                cyberAttackScript.ReceiveMessage(isVirused || !isEncrypted || !entered || MessageText.text.Trim() != lastEnteredText.Trim());
                started = false;
                Destroy(gameObject);
            }
            else
                dnsHijackRectTransform.anchoredPosition = new Vector2(Mathf.Lerp(ServerPosition.x, dnsHijackInitPosition.x, (endTime - Time.time) / TimeToReachServerInSecs), dnsHijackInitPosition.y);
        }
    }
}
