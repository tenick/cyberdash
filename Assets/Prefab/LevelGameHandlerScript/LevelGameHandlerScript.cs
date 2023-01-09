using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Prefab.LevelGameHandlerScript
{
    public class LevelGameHandlerScript : MonoBehaviour
    {
        // Game needs these stuff
        public Animator ClockAnimator;
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
        public int currHour = 0;
        [HideInInspector]
        public bool isGameStarted = false;
        public float incrementAmount => fullLevelTimeInSecs / shiftHours;

        public static int MinCooldown = 10;
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
                Debug.Log("why the fuck is it ticking even though it hasn't fucking started yet??");
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
            if (miniGame.isCorrect)
                AddHealth(1);
            else AddHealth(-1);
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

        public void StartGame()
        {
            CharacterScript.canControl = true;
            CharacterScript.InteractStart += CharacterScript_InteractStart;
            isGameStarted = true;
            miniGameCD = Time.time + MiniGameSpawnRandomCD;
            gameTime = Time.time + incrementAmount;
            employeeHasMinigame = Enumerable.Repeat(false, Employees.transform.childCount).ToList();
            employeeMinigames = Enumerable.Repeat<MiniGameBase>(null, Employees.transform.childCount).ToList();
            ExclamationPoints = Enumerable.Repeat<GameObject>(null, Employees.transform.childCount).ToList();
            ClockAnimator.SetTrigger("incr");
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
                employeeMinigames[employeeIndex].gameObject.SetActive(true);
        }

        IEnumerator WaitForSecs(int secs, Action runAfter)
        {
            yield return new WaitForSeconds(secs);

            runAfter();
        }
    }
}
