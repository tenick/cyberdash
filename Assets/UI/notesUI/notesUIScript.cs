using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class notesUIScript : MonoBehaviour
{
    public event EventHandler OnOpen;
    public event EventHandler OnClose;
    public GameObject NotesPanel;
    public characterAnimScript CharacterScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenNote()
    {
        NotesPanel.SetActive(true);
        CharacterScript.canControl = false;
        OnOpen?.Invoke(this, EventArgs.Empty);
    }

    public void CloseNote()
    {
        NotesPanel.SetActive(false);
        CharacterScript.canControl = true;
        OnClose?.Invoke(this, EventArgs.Empty);
    }
}
