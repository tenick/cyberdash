using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DNSHijackScript : AttackBase
{
    System.Guid g = System.Guid.NewGuid();
    public static int MinCooldown = 35;
    public static float CDAddVariance = .33f; // [0,1] : random percentage of MinCooldown to be added
    static System.Random rand = new System.Random();
    public override float RandomCD
    {
        get { return MinCooldown * (float)(1 + CDAddVariance * rand.NextDouble()); }
    }

    Dictionary<string, string> wrongToCorrect = new()
    {
        { "goggol.com", "google.com" },
        { "yehoo.com", "yahoo.com" },
        { "fistbook.com", "facebook.com" },
        { "tweeter.com", "twitter.com" },
        { "yuutube.com", "youtube.com" }
    };

    public TextMeshProUGUI DomainNameText;
    public TMP_InputField InputField;


    public Vector2 ServerPosition;

    public int TimeToReachServerInSecs = 60;

    CyberAttackScript cyberAttackScript;
    float startTime;
    float endTime;
    int speed;
    bool started;
    Vector2 dnsHijackInitPosition;
    RectTransform dnsHijackRectTransform;

    // win conditions
    bool correctedDomainName = false;
    string lastEnteredText = "";
    bool entered = false;

    private void Start()
    {
        DomainNameText.text = wrongToCorrect.ElementAt(rand.Next(0, 4)).Key;

        cyberAttackScript = gameObject.transform.parent.parent.parent.GetComponent<CyberAttackScript>();
        started = true;
        ServerPosition = gameObject.transform.parent.parent.Find("ServerPosition").GetComponent<RectTransform>().anchoredPosition;
        startTime = Time.time;
        endTime = startTime + TimeToReachServerInSecs;
        dnsHijackRectTransform = transform.GetComponent<RectTransform>();
        dnsHijackInitPosition = dnsHijackRectTransform.anchoredPosition;

        // events
        GetComponent<Button>().onClick.AddListener(OnClick_DNSHijack);
        InputField.onSubmit.AddListener(delegate { OnSubmit_DNSHijack(); });

    }
    public void OnClick_DNSHijack()
    {
        if (!correctedDomainName && cyberAttackScript.currToolState == ToolState.MagnifyingGlass)
        {
            DomainNameText.text = wrongToCorrect[DomainNameText.text];
            correctedDomainName = true;
        }
    }

    public void OnSubmit_DNSHijack()
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
                cyberAttackScript.ReceiveMessage(!correctedDomainName || !entered || DomainNameText.text.Trim() != lastEnteredText.Trim());
                started = false;
                Destroy(gameObject);
            }
            else
                dnsHijackRectTransform.anchoredPosition = new Vector2(Mathf.Lerp(ServerPosition.x, dnsHijackInitPosition.x, (endTime - Time.time) / TimeToReachServerInSecs), dnsHijackInitPosition.y);
        }
    }
}
