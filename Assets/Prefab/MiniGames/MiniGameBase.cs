using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameBase : MonoBehaviour
{
    [HideInInspector]
    public bool isCorrect;
    public event EventHandler Close;
    public event EventHandler Check;
    public event EventHandler TimeTicked;
    public Selectable CheckUI;
    public Animator ClockAnimator;
    public bool TimeExceededFlag;
    public bool isTutorial;

    public virtual float MinigameDuration { get; }

    public float endTime;
    float nextClockTick;
    public virtual void Start()
    {
        endTime = Time.time + MinigameDuration;
        nextClockTick = Time.time + MinigameDuration / 6;
    }
    public void Update()
    {
        if (isTutorial) return;

        if (Time.time >= nextClockTick) // clock will tick
        {
            Debug.Log("time ticked");
            ClockAnimator.SetTrigger("incr");
            nextClockTick = Time.time + MinigameDuration / 6;
            TimeTicked?.Invoke(this, EventArgs.Empty);
        }

        if (Time.time >= endTime) // time's up for solving this minigame
        {
            TimeExceededFlag = true;
            CloseBtn();
        }
    }

    public virtual void ShowBtn()
    {
        Vector3 camPos = Camera.main.transform.position;
        camPos.z = 0;
        transform.position = camPos;
    }
    public virtual void CloseBtn()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        transform.position += new Vector3(0, -1000, 0);
        OnClose();
    }

    public virtual void OnClose()
    {
        Close?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnCheck()
    {
        if (isCorrect)
        {
            if (CheckUI != null)
                CheckUI.interactable = false;
        }
        Check?.Invoke(this, EventArgs.Empty);
    }
}