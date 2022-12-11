using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class instructionsScript : MonoBehaviour
{
    public bool AllowSpace = true;
    public event EventHandler Close;
    void Update()
    {
        if (AllowSpace && Input.GetKeyDown(KeyCode.Space))
        {
            this.gameObject.SetActive(false);
            Close?.Invoke(this, EventArgs.Empty);
        }
    }
}
