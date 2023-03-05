using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Prefab.LevelGameHandlerScript
{
    public class LevelGameHandlerScript : MonoBehaviour
    {
        // Game needs these stuff
        public GameObject ExclamationPoint;
        public GameObject ExclamationPointsObj;
        public sceneTransitionScript transitionScript;
        public Animator LevelCompleteAnimator;
        public characterAnimScript CharacterScript;

        // Employees
        public GameObject Employees;

        // minigames
        public GameObject MiniGamesObj;
        public List<MiniGameBase> MiniGames;

        // level settings
        const int shiftHours = 6;
        public float fullLevelTimeInSecs = 180;

        // health
        public List<Sprite> HealthSprites;
        [HideInInspector]
        public int currHealth = 6;
        public Image Satisfaction;

        // states
        [HideInInspector]
        public bool isGameStarted = false;
        public float incrementAmount => fullLevelTimeInSecs / shiftHours;
        public TextMeshProUGUI ClearTasksText;
        public TextMeshProUGUI PreventThreatsText;

        public int MinigameSpawnCooldown = 30;
        public int MinigamesClearAmountToWin = 4;
        public int ThreatsClearAmountToWin = 4;
        public int CurrentMinigameClearAmount = 0; // how many minigames the player has cleared
        public int CurrentThreatsClearAmount = 0; // how many threats the player has cleared

        // game states
        float miniGameCD;
        List<bool> employeeHasMinigame = new List<bool>() { false, false, false, false };
        List<MiniGameBase> employeeMinigames = new List<MiniGameBase> { null, null, null, null };
        List<GameObject> ExclamationPoints = new List<GameObject> { null, null, null, null };

        int currentMiniGameCount = 0;
        int minigameSpawned = 0;

        private void Update()
        {
            //Debug.Log("Current time: " + Time.time + " cooldown time: " + miniGameCD);

            if (isGameStarted && Time.time > miniGameCD)
            {
                //Debug.Log("Current mini game count: " + currentMiniGameCount + " employeeMinigames.Count: " + employeeMinigames.Count + " CurrentMinigameClearAmount: " + CurrentMinigameClearAmount + " MinigamesClearAmountToWin: " + MinigamesClearAmountToWin);

                if (currentMiniGameCount < employeeMinigames.Count && (currentMiniGameCount + CurrentMinigameClearAmount) < MinigamesClearAmountToWin)
                {
                    int miniGameIndex = UnityEngine.Random.Range(0, MiniGames.Count);
                    int employeeIndex = UnityEngine.Random.Range(0, Employees.transform.childCount);
                    if (minigameSpawned == 0)
                    {
                        miniGameIndex = MiniGames.Count - 1; // assume that QuizGame is inserted last in the list of minigames
                    }

                    Debug.Log(employeeIndex + " | " + employeeHasMinigame.Count);

                    while (employeeHasMinigame[employeeIndex])
                        employeeIndex = UnityEngine.Random.Range(0, Employees.transform.childCount);

                    // spawn exclamation point
                    ExclamationPoints[employeeIndex] = Instantiate(ExclamationPoint, Employees.transform.GetChild(employeeIndex).position + new Vector3(0, 1), Quaternion.identity, ExclamationPointsObj.transform);
                    ExclamationPoints[employeeIndex].SetActive(true);

                    // spawn minigame
                    employeeMinigames[employeeIndex] = Instantiate(MiniGames[miniGameIndex], MiniGamesObj.transform);
                    if (employeeMinigames[employeeIndex] is QuizGameScript)
                    {
                        (employeeMinigames[employeeIndex] as QuizGameScript).selectedEmployeeIndex = employeeIndex;
                    }
                    employeeMinigames[employeeIndex].CloseBtn();
                    employeeMinigames[employeeIndex].Close += MiniGame_OnClose;
                    employeeMinigames[employeeIndex].Check += MiniGame_Check;
                    employeeMinigames[employeeIndex].TimeTicked += LevelGameHandlerScript_TimeTicked;
                    employeeHasMinigame[employeeIndex] = true;
                    currentMiniGameCount++;

                    minigameSpawned++;

                    AddHealth(-1);
                }
                miniGameCD = Time.time + MinigameSpawnCooldown;
            }
        }

        private void LevelGameHandlerScript_TimeTicked(object sender, EventArgs e)
        {
            for (int i = 0; i < employeeMinigames.Count; i++)
            {
                if (object.ReferenceEquals(employeeMinigames[i], sender))
                {
                    Debug.Log("ticked!");
                    MiniGameBase minigame = employeeMinigames[i];
                    GameObject exclam = ExclamationPoints[i];
                    float interpolVal = Math.Abs(Time.time - minigame.endTime) / minigame.MinigameDuration;
                    Debug.Log(interpolVal);
                    exclam.GetComponent<Image>().color = new Color(1, interpolVal, interpolVal);
                    break;
                }
            }
        }

        private void MiniGame_Check(object sender, EventArgs e)
        {
            // if wrong, must decrease health
            MiniGameBase miniGame = (MiniGameBase)sender;
            if (miniGame.isCorrect)
            {
                AddHealth(1);
                CurrentMinigameClearAmount++;
                ClearTasksText.text = $"Clear {MinigamesClearAmountToWin - CurrentMinigameClearAmount} Tasks";
                Debug.Log("current clears: " + CurrentMinigameClearAmount);
            }
            else AddHealth(-1);
        }

        public void ReceiveMessage(bool isMaliciousMessage)
        {
            if (isMaliciousMessage)
                AddHealth(-1);
            else
            {
                AddHealth(1);
                CurrentThreatsClearAmount++;
                if (CurrentThreatsClearAmount > ThreatsClearAmountToWin) CurrentThreatsClearAmount = ThreatsClearAmountToWin;
                PreventThreatsText.text = $"Prevent {ThreatsClearAmountToWin - CurrentThreatsClearAmount} Threats";
                CheckIfGameOver();
            }
        }


        public void AddHealth(int healthToAdd)
        {
            currHealth += healthToAdd;
            currHealth = Math.Clamp(currHealth, 0, 6);
            Satisfaction.sprite = HealthSprites[currHealth];
            CheckIfGameOver();
        }

        void CheckIfGameOver()
        {
            if (currHealth == 0) // game over
            {
                isGameStarted = false;
                StartCoroutine(WaitForSecs(1, () => {
                    transitionScript.TransitionToScene = 6;
                    transitionScript.PlayFadeInOnChangeScene = true;
                    transitionScript.DoTransition();
                }));
            }
            else if (CurrentMinigameClearAmount >= MinigamesClearAmountToWin && CurrentThreatsClearAmount >= ThreatsClearAmountToWin) // level complete
            {
                isGameStarted = false;
                LevelCompleteAnimator.SetTrigger("play");
                StartCoroutine(WaitForSecs(1, () => {
                    transitionScript.PlayFadeInOnChangeScene = true;
                    transitionScript.DoTransition();
                }));
            }
        }

        private void MiniGame_OnClose(object sender, EventArgs e)
        {
            // if correct, must delete
            MiniGameBase miniGame = (MiniGameBase)sender;
            if (miniGame.isCorrect || miniGame.TimeExceededFlag)
            {
                for (int i = 0; i < employeeMinigames.Count; i++)
                {
                    if (object.ReferenceEquals(employeeMinigames[i], sender))
                    {
                        Destroy(employeeMinigames[i]);
                        Destroy(ExclamationPoints[i]);
                        employeeHasMinigame[i] = false;
                        currentMiniGameCount--;
                        break;
                    }
                }

                if (!miniGame.isCorrect && miniGame.TimeExceededFlag)
                    AddHealth(-1);

                if (isGameStarted && CurrentMinigameClearAmount >= MinigamesClearAmountToWin)
                    CheckIfGameOver();
            }
        }

        public void StartGame()
        {
            Debug.Log("dito naman ang tunay na start");
            CharacterScript.canControl = true;
            CharacterScript.InteractStart += CharacterScript_InteractStart;
            isGameStarted = true;
            miniGameCD = Time.time + MinigameSpawnCooldown;
            employeeHasMinigame = Enumerable.Repeat(false, Employees.transform.childCount).ToList();
            employeeMinigames = Enumerable.Repeat<MiniGameBase>(null, Employees.transform.childCount).ToList();
            ExclamationPoints = Enumerable.Repeat<GameObject>(null, Employees.transform.childCount).ToList();
        }

        private void CharacterScript_InteractStart(object sender, InteractArgs e)
        {
            // get employee index first
            int employeeIndex = 0;
            bool found = false;
            for (int i = 0; i < Employees.transform.childCount; i++)
            {
                if (e.CollidedGameObj.name == Employees.transform.GetChild(i).name)
                {
                    employeeIndex = i;
                    Debug.Log("found!!!! " + e.CollidedGameObj.name + " is at index " + employeeIndex);
                    found = true;
                    break;
                }
            }

            // check if it has minigame
            if (found && employeeHasMinigame[employeeIndex])
                employeeMinigames[employeeIndex].ShowBtn();
        }

        IEnumerator WaitForSecs(int secs, Action runAfter)
        {
            yield return new WaitForSeconds(secs);

            runAfter();
        }
    }
}
