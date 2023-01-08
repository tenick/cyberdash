using Assets.Prefab.MiniGames.TracertGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class TracertScript : MiniGameBase
{
    public TextMeshProUGUI TerminalTexts;
    public TMP_InputField PingInput;
    public TMP_InputField SourceInput;
    public TMP_InputField DestInput;
    public TextMeshProUGUI GuideText;
    public TextMeshProUGUI UsesLeftText;
    public 
    int usesLeft = 4;

    List<string> nodes = new()
    {
        "B1", "B2", "B3", "B4", "A", "B", "C", "D", "E", "F"
    };
    List<(string N1, string N2)> edges = new()
    {
        ("B1", "B"), ("B", "A"), ("B3", "A"), ("C", "A"),
        ("C", "B4"), ("E", "B4"), ("E", "F"), ("B2", "F"),
        ("F", "D"), ("B1", "D"), ("D", "C"), ("B", "C"), ("E", "D")
    };
    string brokenRouter;

    // Start is called before the first frame update
    void Start()
    {
        
        PingInput.onSubmit.AddListener(SendPing);

        List<string> routers = nodes.Where(r => r.Length == 1).ToList();
        brokenRouter = routers[UnityEngine.Random.Range(0, routers.Count)];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendPing(string text)
    {
        Debug.Log(text);
        string pingInput = PingInput.text.Trim();
        TerminalTexts.text += ">>> ping router " + pingInput + '\n';
        if (brokenRouter == pingInput) // correct
        {
            isCorrect = true;
            PingInput.enabled = false;
            GuideText.text = "Correct!";
            TerminalTexts.text += "    " + "Pinging " + pingInput + " with 8 bytes of data:\n    Request timed out.\n\n    Ping Statistics for " + pingInput + ": \n        Packets: Sent = 1, Received = 0, Lost = 1 (100% loss)";
        }
        else
            TerminalTexts.text += "    " + "Pinging " + pingInput + " with 8 bytes of data:\n    Reply from " + pingInput + ": bytes=8 time=42ms TTL=109\n\n    Ping Statistics for " + pingInput + ": \n        Packets: Sent = 1, Received = 1, Lost = 0 (0% loss)";
        PingInput.text = "";
    }

    public void SendPacket()
    {
        if (usesLeft <= 0) return;

        string destNodeStr = DestInput.text;
        string srcNodeStr = SourceInput.text;

        List<List<string>> allPaths = DFS.allPaths(nodes, edges, srcNodeStr, destNodeStr);

        List<string> routers = nodes.Where(r => r.Length == 1).ToList();
        //brokenRouter = routers[UnityEngine.Random.Range(0, routers.Count)];


        // sort paths by length
        SortedDictionary<int, List<List<string>>> lenByPaths = new();
        foreach (var path in allPaths)
        {
            int pathLen = path.Count;
            if (!lenByPaths.ContainsKey(pathLen))
                lenByPaths.Add(pathLen, new());
            lenByPaths[pathLen].Add(path);
        }

        //Console.WriteLine($"Number of shortest path/s from {srcNodeStr} to {destNodeStr}: " + allPaths.Count);
        bool found = false;
        string result = "Request Timed Out";
        foreach (var kvp in lenByPaths)
        {
            //Console.WriteLine("Paths of length: " + kvp.Key);
            foreach (var path in kvp.Value)
            {
                //Console.WriteLine(string.Join('-', path));
                if (!path.Contains(brokenRouter))
                {
                    found = true;
                    result = string.Join('-', path);
                    break;
                }
            }
            if (found) break;
        }

        TerminalTexts.text += ">>> Tracert " + destNodeStr + '\n';
        TerminalTexts.text += "    " + result + '\n';
        Debug.Log(brokenRouter);


        usesLeft--;
        UsesLeftText.text = usesLeft + "";
    }

    public void CloseBtn()
    {
        gameObject.SetActive(false);
        OnClose();
    }
}
