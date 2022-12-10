using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Level1_1Script : MonoBehaviour
{
    public Animator level1_1PanelFadeoutAnimator;

    public characterAnimScript CharacterScript;

    public List<Action> SceneFlow;
    public int currentFlowIndex;

    public GameObject AmeliaDialog1;
    public GameObject AmeliaDialog2;
    public GameObject AmeliaDialog3;

    public GameObject SatisfactionExample;
    public GameObject ArrowNotif;
    public Animator ClockAnimator;
    public GameObject ExclamationPoint;
    public GameObject ExclamationPointsObj;
    public sceneTransitionScript transitionScript;
    public Animator LevelCompleteAnimator;


    // Employees
    public GameObject Employees;

    // minigames
    public GameObject MiniGamesObj;
    public List<MiniGameBase> MiniGames;

    // level settings
    const int shiftHours = 6;
    const float fullLevelTimeInSecs = 180;

    // health
    public List<Sprite> HealthSprites;
    [HideInInspector]
    public int currHealth = 6;
    public Image Satisfaction;

    // states
    [HideInInspector]
    public int currHour = 0;
    [HideInInspector]
    public bool isGameStarted = false;
    public float incrementAmount => fullLevelTimeInSecs / shiftHours;

    // Start is called before the first frame update
    void Start()
    {
        // fadeout the level 1 tutorial panel
        StartCoroutine(WaitForSecs(2, () =>
        {
            level1_1PanelFadeoutAnimator.SetTrigger("playFadeout");
            StartCoroutine(WaitForSecs(1, () =>
            {
                AmeliaDialog1.SetActive(true);
                CharacterScript.canControl = false;
                AmeliaDialog1.GetComponent<dialogHandlerScript>().Restart();
                AmeliaDialog1.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
            }));
        }));


        // define the scene flow
        SceneFlow = new List<Action>();
        SceneFlow.Add(() => // 0: show arrow and dialog2
        {
            ArrowNotif.SetActive(true);
            StartCoroutine(WaitForSecs(2, () =>
            {
                ArrowNotif.SetActive(false);
                ArrowNotif.transform.position = new Vector2(-0.5f, 3.73f);
                AmeliaDialog2.SetActive(true);
                AmeliaDialog2.GetComponent<dialogHandlerScript>().Restart();
                AmeliaDialog2.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
            }));
        });
        SceneFlow.Add(() => // 1: show arrow and dialog3
        {
            ArrowNotif.SetActive(true);
            StartCoroutine(WaitForSecs(2, () =>
            {
                ArrowNotif.SetActive(false);
                AmeliaDialog3.SetActive(true);
                AmeliaDialog3.GetComponent<dialogHandlerScript>().Restart();
                AmeliaDialog3.GetComponent<dialogHandlerScript>().DialogFinish += Dialog_DialogFinish;
            }));
        });
        SceneFlow.Add(() => // 2: game start
        {
            //SatisfactionExample.SetActive(false);
            CharacterScript.canControl = true;
            CharacterScript.InteractStart += CharacterScript_InteractStart;
            isGameStarted = true;
            miniGameCD = Time.time + MiniGameSpawnRandomCD;
            gameTime = Time.time + incrementAmount;
            employeeHasMinigame = Enumerable.Repeat(false, Employees.transform.childCount).ToList();
            employeeMinigames = Enumerable.Repeat<MiniGameBase>(null, Employees.transform.childCount).ToList();
            ExclamationPoints = Enumerable.Repeat<GameObject>(null, Employees.transform.childCount).ToList();
            ClockAnimator.SetTrigger("incr");
        });
    }

    private void CharacterScript_InteractStart(object sender, InteractArgs e)
    {
        // get employee index first
        int employeeIndex = 0;
        for (int i = 0; i < Employees.transform.childCount; i++)
        {
            if (e.CollidedGameObj.name == Employees.transform.GetChild(i).name)
            {
                employeeIndex = i;
                Debug.Log("found!!!! " + e.CollidedGameObj.name + " is at index " + employeeIndex);
                break;
            }
        }

        // check if it has minigame
        if (employeeHasMinigame[employeeIndex])
            employeeMinigames[employeeIndex].gameObject.SetActive(true);
    }

    void Proceed()
    {
        SceneFlow[currentFlowIndex].Invoke();
        currentFlowIndex++;
    }

    public static int MinCooldown = 15;
    public static float CDAddVariance = .5f;
    static System.Random rand = new System.Random();

    public float MiniGameSpawnRandomCD
    {
        get { return MinCooldown * (float)(1 + CDAddVariance * rand.NextDouble()); }
    }

    // game states
    float miniGameCD;
    List<bool> employeeHasMinigame = new List<bool>() { false, false, false, false };
    List<MiniGameBase> employeeMinigames = new List<MiniGameBase> { null, null, null, null };
    List<GameObject> ExclamationPoints = new List<GameObject> { null, null, null, null };

    int currentMiniGameCount = 0;

    float gameTime;
    private void Update()
    {
        if (isGameStarted && Time.time > gameTime)
        {
            ClockAnimator.SetTrigger("incr");
            gameTime = Time.time + incrementAmount;
            currHour++;
            CheckIfGameOver();
        }

        if (isGameStarted && Time.time > miniGameCD)
        {
            if (currentMiniGameCount < employeeMinigames.Count)
            {
                int miniGameIndex = UnityEngine.Random.Range(0, MiniGames.Count);
                int employeeIndex = UnityEngine.Random.Range(0, Employees.transform.childCount);

                Debug.Log(employeeIndex + " | " + employeeHasMinigame.Count);

                while (employeeHasMinigame[employeeIndex])
                    employeeIndex = UnityEngine.Random.Range(0, Employees.transform.childCount);

                // spawn exclamation point
                ExclamationPoints[employeeIndex] = Instantiate(ExclamationPoint, Employees.transform.GetChild(employeeIndex).position + new Vector3(0, 1), Quaternion.identity, ExclamationPointsObj.transform);
                ExclamationPoints[employeeIndex].SetActive(true);

                // spawn minigame
                employeeMinigames[employeeIndex] = Instantiate(MiniGames[miniGameIndex], MiniGamesObj.transform);
                employeeMinigames[employeeIndex].gameObject.SetActive(false);
                employeeMinigames[employeeIndex].Close += MiniGame_OnClose;
                employeeMinigames[employeeIndex].Check += MiniGame_Check;
                employeeHasMinigame[employeeIndex] = true;
                currentMiniGameCount++;
            }
            miniGameCD = Time.time + MiniGameSpawnRandomCD;
        }
    }

    private void MiniGame_Check(object sender, EventArgs e)
    {
        // if wrong, must decrease health
        MiniGameBase miniGame = (MiniGameBase)sender;
        if (!miniGame.isCorrect)
        {
            currHealth--;
            Satisfaction.sprite = HealthSprites[currHealth];
        }
        CheckIfGameOver();
    }

    void CheckIfGameOver()
    {
        if (currHealth == 0) // game over
        {
            StartCoroutine(WaitForSecs(1, () => {
                transitionScript.TransitionToScene = 6;
                transitionScript.PlayFadeInOnChangeScene = true;
                transitionScript.DoTransition();
            }));
        }
        else if (currHour == 6) // level complete
        {
            LevelCompleteAnimator.SetTrigger("play");
            StartCoroutine(WaitForSecs(1, () => {
                transitionScript.TransitionToScene = 7;
                transitionScript.PlayFadeInOnChangeScene = true;
                transitionScript.DoTransition();
            }));
        }
    }

    private void MiniGame_OnClose(object sender, EventArgs e)
    {
        // if correct, must delete
        MiniGameBase miniGame = (MiniGameBase)sender;
        if (miniGame.isCorrect)
        {
            for (int i = 0; i < employeeMinigames.Count; i++)
            {
                if (object.ReferenceEquals(employeeMinigames[i], sender))
                {
                    Debug.Log("founddddd this SHITTTT");
                    Destroy(employeeMinigames[i]);
                    Destroy(ExclamationPoints[i]);
                    employeeHasMinigame[i] = false;
                    currentMiniGameCount--;
                    return;
                }
            }
            Debug.Log("not found mannnnn :(((");
        }
        else
        {
            Debug.Log("minus healthhhh!!");
        }
    }

    private void Dialog_DialogFinish(object sender, EventArgs e)
    {
        Proceed();
    }

    IEnumerator WaitForSecs(int secs, Action runAfter)
    {
        yield return new WaitForSeconds(secs);

        runAfter();
    }
}
