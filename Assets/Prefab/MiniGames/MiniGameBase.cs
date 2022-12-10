using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MiniGameBase : MonoBehaviour
{
    [HideInInspector]
    public bool isCorrect;
    public event EventHandler Close;
    public event EventHandler Check;
    public virtual float RandomCD { get; }

    public virtual void OnClose()
    {
        Close?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnCheck()
    {
        Check?.Invoke(this, EventArgs.Empty);
    }
}