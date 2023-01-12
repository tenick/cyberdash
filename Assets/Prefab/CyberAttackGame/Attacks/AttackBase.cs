using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AttackBase : MonoBehaviour
{
    public int MinCooldown = 35;
    public float CDAddVariance = .33f; // [0,1] : random percentage of MinCooldown to be added
    public virtual float RandomCD
    {
        get
        {
            return MinCooldown * (float)(1 + CDAddVariance * UnityEngine.Random.Range(0f, 1f)) * (1 + UnityEngine.Random.Range(0f, 1f) / UnityEngine.Random.Range(1, 100));
        }
    }

    public int TimeToReachServerInSecs = 60;

    public Vector2 ServerPosition;

    protected CyberAttackScript cyberAttackScript;
    protected float startTime;
    protected float endTime;
    protected int speed;
    protected bool started;
    protected Vector2 dnsHijackInitPosition;
    protected RectTransform dnsHijackRectTransform;
}