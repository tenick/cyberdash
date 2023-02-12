using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizGameScript : MiniGameBase
{
    public static int MinCooldown = 30;
    public override float MinigameDuration
    {
        get { return MinCooldown; }
    }

    public GameObject ChoicesObj;
    public List<Sprite> Sprites;
    public int selectedEmployeeIndex;

    public Image EmployeeImg;
    public TextMeshProUGUI DialogTxt;
    public TextMeshProUGUI Choice1Txt;
    public TextMeshProUGUI Choice2Txt;
    public TextMeshProUGUI Choice3Txt;


    public static int qStartIndex = 0;
    public static int qEndIndex = 5;
    List<(string question, List<string> options, int correctIndex, string CorrectDialog)> QnA = new()
    {
        (
            "One of the employees who uses outdated software got attacked by malware. What will you do to mitigate this phenomenon?", 
            new List<string> {
                "Re-install the computer’s OS",
                "Use a stronger password",
                "Look for software updates"
            },
            2,
            "Software updates are necessary to keep computers, mobile devices and tablets running smoothly -- and they may lower security vulnerabilities."
        ),
        (
            "While browsing the net, I’ve noticed that my computer system was infiltrated by malware. What will you do?",
            new List<string> {
                "Disconnect your internet connection",
                "Turn on firewall",
                "Use an adblock"
            },
            1,
            "Firewalls provide protection against outside cyber attackers by shielding your computer or network from malicious or unnecessary network traffic. Firewalls can also prevent malicious software from accessing a computer or network via the internet."
        ),
        (
            "You’ve heard that one of your colleagues got hacked from their social media by ransomware. What can you advise them?",
            new List<string> {
                "Use a different name next time",
                "Stop using social media",
                "Use a stronger password"
            },
            2,
            "A strong password is the main barrier keeping most of your online accounts from being hacked. Without up to date practices, you might be using passwords that cyber-frauds can easily guess within hours."
        ),
        (
            "I’ve read an article on how identity theft is prevalent nowadays. What can we do to improve our network security?",
            new List<string> {
                "Turn on firewall",
                "Use a dual factor authentication",
                "Back up data"
            },
            1,
            "Multi-Factor Authentication increases security because even if one credential becomes compromised, unauthorized users will be unable to meet the second authentication requirement, negating access to a targeted device."
        ),
        (
            "I received an email from an anonymous company along with an attachment which they claimed to contain their business proposal. How should I respond?",
            new List<string> {
                "Ignore the email",
                "View their business proposal",
                "Download the attachment"
            },
            0,
            "Malicious email attachments can be classified into three categories: those that try to steal your personal information, those that try to take over your computer, and those that try to hold you hostage."
        ),
        (
            "I received a call from one of our customers claiming that whenever they use our domain name, it will bring them to a suspicious website instead. How do we classify this threat?",
            new List<string> {
                "Man-in-the-middle",
                "DNS Hijacking",
                "DDOS"
            },
            1,
            "Domain Name System hijacking is an attack that redirects you to websites that are different from the ones you're intending to visit. This is usually done to steal your personal data, display unwanted ads, or impose internet censorship."
        ),
        (
            "I gathered a report that one of our customers was a victim of identity theft. It appears as if someone had been stealing our customer’s personal information by intercepting and eavesdropping on our network. What threat is at play here?",
            new List<string> {
                "DNS Hijacking",
                "DDOS",
                "Man-in-the-middle"
            },
            2,
            "Domain Name System hijacking is an attack that redirects you to websites that are different from the ones you're intending to visit. This is usually done to steal your personal data, display unwanted ads, or impose internet censorship."
        ),
        (
            "While surfing the net, I’ve noticed that there is quite some traffic on our servers – rendering our service unavailable to our users. What kind of threat is doing this?",
            new List<string> {
                "DDOS",
                "Man-in-the-middle",
                "DNS Hijacking"
            },
            0,
            "A DDoS attack is when someone tries to make a website or computer unavailable to other."
        ),
    };

    string WrongDialog = "That feels kinda wrong. Let’s think this through.";
    (string question, List<string> options, int correctIndex, string CorrectDialog) selectedQna;

    int selectedQuestionIndex;
    public override void Start()
    {
        base.Start();

        selectedQuestionIndex = UnityEngine.Random.Range(qStartIndex, qEndIndex);
        selectedQna = QnA[selectedQuestionIndex];

        // set UI
        DialogTxt.text = selectedQna.question;
        Choice1Txt.text = selectedQna.options[0];
        Choice2Txt.text = selectedQna.options[1];
        Choice3Txt.text = selectedQna.options[2];
    }

    public override void ShowBtn()
    {
        EmployeeImg.sprite = Sprites[selectedEmployeeIndex];
        base.ShowBtn();
    }

    public void CheckBtn(int choice)
    {
        if (isCorrect) return;

        isCorrect = choice == selectedQna.correctIndex;
        if (isCorrect)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i != choice)
                    ChoicesObj.transform.GetChild(i).gameObject.SetActive(false);
                else
                {
                    GameObject correctChoice = ChoicesObj.transform.GetChild(i).gameObject;
                    correctChoice.GetComponent<Image>().color = new(0, 1, 0);
                }
            }
            DialogTxt.text = selectedQna.CorrectDialog;
        }
        else
        {
            ChoicesObj.transform.GetChild(choice).gameObject.SetActive(false);
            DialogTxt.text = WrongDialog;
            StartCoroutine(WaitForSecs(2, () =>
            {
                if (!isCorrect) DialogTxt.text = selectedQna.question;
            }));
        }
        OnCheck();
    }

    IEnumerator WaitForSecs(int secs, Action runAfter)
    {
        yield return new WaitForSeconds(secs);

        runAfter();
    }
}
