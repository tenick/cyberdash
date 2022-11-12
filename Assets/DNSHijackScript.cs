using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DNSHijackScript : MonoBehaviour
{
    System.Guid g = System.Guid.NewGuid();
    public static int MinCooldown = 5;
    public static float CDAddVariance = .5f;
    static System.Random rand = new System.Random();
    public static float RandomCD
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

    public int TimeToReachServerInSecs = 8;

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

    private void Start()
    {
        DomainNameText.text = wrongToCorrect.ElementAt(rand.Next(0, 4)).Key;

        cyberAttackScript = GameObject.Find("CyberAttackGame").GetComponent<CyberAttackScript>();
        started = true;
        ServerPosition = GameObject.Find("ServerPosition").GetComponent<RectTransform>().anchoredPosition;
        startTime = Time.time;
        endTime = startTime + TimeToReachServerInSecs;
        dnsHijackRectTransform = transform.GetComponent<RectTransform>();
        dnsHijackInitPosition = dnsHijackRectTransform.anchoredPosition;

        // events
        GetComponent<Button>().onClick.AddListener(OnClick_DNSHijack);
        InputField.onEndEdit.AddListener(delegate { OnEndEdit_DNSHijack(); });

    }
    public void OnClick_DNSHijack()
    {
        if (!correctedDomainName && cyberAttackScript.currToolState == ToolState.MagnifyingGlass)
        {
            DomainNameText.text = wrongToCorrect[DomainNameText.text];
            correctedDomainName = true;
        }

    }

    public void OnEndEdit_DNSHijack()
    {
        lastEnteredText = InputField.text;
        Destroy(InputField);
    }

    void Update()
    {
        if (started)
        {
            if (Time.time > endTime)
            {
                Destroy(gameObject);
                cyberAttackScript.ReceiveMessage(!correctedDomainName || DomainNameText.text != lastEnteredText);
            }
            else
                dnsHijackRectTransform.anchoredPosition = new Vector2(Mathf.Lerp(ServerPosition.x, dnsHijackInitPosition.x, (endTime - Time.time) / TimeToReachServerInSecs), dnsHijackInitPosition.y);
            
        }

    }
}
