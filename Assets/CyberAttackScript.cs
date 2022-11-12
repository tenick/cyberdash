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

public class CyberAttackScript : MonoBehaviour
{
    public Texture2D heart;
    public Texture2D noHeart;
    public Texture2D magnifyingGlassCursor;
    public Texture2D padlockCursor;

    public GameObject healthUI;

    public int currentHearts = 3;

    public GameObject DNSHijack;
    public Transform ParentObject;

    public ToolState currToolState;

    // Start is called before the first frame update
    void Start()
    {
        lastDNSHijack = Time.time + DNSHijackScript.RandomCD;
        currToolState = ToolState.Default;
    }

    public void RestartDefault()
    {
        
    }
    public void RestartWithParams()
    {

    }
    void RestartGeneral()
    {
    }

    float lastDNSHijack;
    float lastMITMA;
    float lastDDOS;
    
    void Update()
    {
        if (Time.time > lastDNSHijack)
        {
            
            GameObject newDNSHijack = Instantiate(DNSHijack, ParentObject);
            newDNSHijack.SetActive(true);
            lastDNSHijack = Time.time + DNSHijackScript.RandomCD;
        }
    }

    public void ReceiveMessage(bool isMaliciousMessage)
    {
        if (isMaliciousMessage)
        {
            healthUI.transform.GetChild(currentHearts - 1).GetComponent<Image>().sprite = Sprite.Create(noHeart, Rect.zero, new Vector2());
            currentHearts--;
        }
    }

    public void OnClick_MagnifyingGlassTool()
    {
        Cursor.SetCursor(magnifyingGlassCursor, new Vector2(), CursorMode.Auto);
        currToolState = ToolState.MagnifyingGlass;
    }

    public void OnClick_PadlockTool()
    {
        Cursor.SetCursor(padlockCursor, new Vector2(), CursorMode.Auto);
        currToolState = ToolState.Padlock;
    }

    public void OnClick_KeyboardTool()
    {
        Cursor.SetCursor(null, new Vector2(), CursorMode.Auto);
        currToolState = ToolState.Default;
    }
}
