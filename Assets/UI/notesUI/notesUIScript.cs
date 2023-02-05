using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class notesUIScript : MonoBehaviour
{
    public event EventHandler OnOpen;
    public event EventHandler OnClose;
    public GameObject NotesPanel;
    public characterAnimScript CharacterScript;

    public Image NotePageImg;
    public List<Sprite> NotePages;
    public int pageIndex = 0;

    public static int end = 2;

    // Start is called before the first frame update
    void Start()
    {
        NotePageImg.sprite = NotePages[pageIndex];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextNote()
    {
        pageIndex = Math.Clamp(pageIndex + 1, 0, end);
        NotePageImg.sprite = NotePages[pageIndex];

    }

    public void PrevNote()
    {
        pageIndex = Math.Clamp(pageIndex - 1, 0, end);
        NotePageImg.sprite = NotePages[pageIndex];
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
