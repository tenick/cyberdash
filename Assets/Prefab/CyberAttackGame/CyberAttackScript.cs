using Assets.Prefab.CyberAttackGame.Attacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ToolState
{
    Default,
    MagnifyingGlass,
    Padlock
}

public class CyberAttackMessageReceiveArgs : EventArgs
{
    public bool IsMaliciousMessage { get; set; }
    public CyberAttackMessageReceiveArgs(bool isMaliciousMessage)
    {
        IsMaliciousMessage = isMaliciousMessage;
    }
}

public class CyberAttackScript : MonoBehaviour
{
    public event EventHandler<CyberAttackMessageReceiveArgs> MessageReceive;

    public Texture2D magnifyingGlassCursor;
    public Texture2D padlockCursor;

    public AttackBase Attack;
    public Transform ParentObject;

    public GameObject AttackLocations;

    public ToolState currToolState;

    // Start is called before the first frame update
    void Start()
    {
        lastAttack1 = Time.time + Attack.RandomCD / 4;
        lastAttack2 = Time.time + Attack.RandomCD / 4;
        lastAttack3 = Time.time + Attack.RandomCD / 4;
        currToolState = ToolState.Default;
    }

    public void ShowBtn()
    {
        Vector3 camPos = Camera.main.transform.position;
        camPos.z = 0;
        transform.position = camPos;
    }
    public void CloseBtn()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        currToolState = ToolState.Default;
        transform.position += new Vector3(0, -1000, 0);
    }

    float lastAttack1;
    float lastAttack2;
    float lastAttack3;
    
    void Update()
    {
        if (Time.time > lastAttack1)
        {
            GameObject newAttack1 = Instantiate(Attack.gameObject, AttackLocations.transform.GetChild(0).position, Quaternion.identity, ParentObject);
            newAttack1.SetActive(true);
            lastAttack1 = Time.time + Attack.RandomCD;
        }

        if (Time.time > lastAttack2)
        {
            GameObject newAttack2 = Instantiate(Attack.gameObject, AttackLocations.transform.GetChild(1).position, Quaternion.identity, ParentObject);
            newAttack2.SetActive(true);
            lastAttack2 = Time.time + Attack.RandomCD;
        }

        if (Time.time > lastAttack3)
        {
            GameObject newAttack3 = Instantiate(Attack.gameObject, AttackLocations.transform.GetChild(2).position, Quaternion.identity, ParentObject);
            newAttack3.SetActive(true);
            lastAttack3 = Time.time + Attack.RandomCD;
        }
    }

    public void ReceiveMessage(bool isMaliciousMessage)
    {
        MessageReceive?.Invoke(this, new CyberAttackMessageReceiveArgs(isMaliciousMessage));
    }

    public void OnClick_MagnifyingGlassTool()
    {
        Cursor.SetCursor(magnifyingGlassCursor, Vector2.zero, CursorMode.Auto);
        currToolState = ToolState.MagnifyingGlass;
    }

    public void OnClick_PadlockTool()
    {
        Cursor.SetCursor(padlockCursor, Vector2.zero, CursorMode.Auto);
        currToolState = ToolState.Padlock;
    }

    public void OnClick_KeyboardTool()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        currToolState = ToolState.Default;
    }
}
