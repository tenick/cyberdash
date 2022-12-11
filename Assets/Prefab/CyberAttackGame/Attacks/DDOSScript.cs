using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DDOSScript : AttackBase
{
    System.Guid g = System.Guid.NewGuid();
    public static int MinCooldown = 45;
    public static float CDAddVariance = .4f;
    static System.Random rand = new System.Random();
    public override float RandomCD
    {
        get { return MinCooldown * (float)(1 + CDAddVariance * rand.NextDouble()); }
    }

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

    public TextMeshProUGUI MessageText1;
    public TextMeshProUGUI MessageText2;
    public TextMeshProUGUI MessageText3;
    public TMP_InputField InputField;

    public Vector2 ServerPosition;


    public int TimeToReachServerInSecs = 65;

    CyberAttackScript cyberAttackScript;
    float startTime;
    float endTime;
    int speed;
    bool started;
    Vector2 dnsHijackInitPosition;
    RectTransform dnsHijackRectTransform;

    // win conditions
    string lastEnteredText = "";
    bool entered = false;


    // Start is called before the first frame update
    void Start()
    {
        MessageText1.text = wordList.ElementAt(rand.Next(0, wordList.Count));
        MessageText2.text = wordList.ElementAt(rand.Next(0, wordList.Count));
        MessageText3.text = wordList.ElementAt(rand.Next(0, wordList.Count));

        cyberAttackScript = GameObject.Find("CyberAttackGame").GetComponent<CyberAttackScript>();
        started = true;
        ServerPosition = GameObject.Find("ServerPosition").GetComponent<RectTransform>().anchoredPosition;
        startTime = Time.time;
        endTime = startTime + TimeToReachServerInSecs;
        dnsHijackRectTransform = transform.GetComponent<RectTransform>();
        dnsHijackInitPosition = dnsHijackRectTransform.anchoredPosition;

        // events
        InputField.onSubmit.AddListener(delegate { OnSubmit_DDOS(); });
    }

    private void OnSubmit_DDOS()
    {
        if (!entered)
        {
            lastEnteredText = InputField.text;
            Destroy(InputField);
            entered = true;
        }
    }

    void Update()
    {
        if (started)
        {

            if (Time.time > endTime)
            {
                List<string> original = new List<string>() { MessageText1.text.Trim(), MessageText2.text.Trim(), MessageText3.text.Trim() };
                List<string> input = lastEnteredText.Split(",").Select(x => x.Trim()).ToList();
                bool matching = original.Count == input.Count;
                input.ForEach(x => {
                    matching = matching && original.Contains(x);
                });
                cyberAttackScript.ReceiveMessage(!entered || !matching);
                started = false;
                Destroy(gameObject);
            }
            else
                dnsHijackRectTransform.anchoredPosition = new Vector2(Mathf.Lerp(ServerPosition.x, dnsHijackInitPosition.x, (endTime - Time.time) / TimeToReachServerInSecs), dnsHijackInitPosition.y);
        }
    }
}