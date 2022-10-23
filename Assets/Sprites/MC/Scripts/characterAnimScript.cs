using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class InteractArgs : EventArgs
{
    public GameObject CollidedGameObj { get; set; }
    public InteractArgs(GameObject collidedGameObj)
    {
        CollidedGameObj = collidedGameObj;
    }
}

public class characterAnimScript : MonoBehaviour
{
    public event EventHandler<InteractArgs> InteractStart;

    public dialogHandlerScript dialogHandlerScript;


    public Rigidbody2D rb;
    public Animator animator;

    public float speed = 5f;

    float timeElapsed;

    float horizontal;
    float vertical;

    public List<GameObject> CurrentInteractableObjects;
    GameObject CurrentChattableCharacter;
    GameObject speechBubbleGameObj;

    public bool canControl = true;

    void Start()
    {
        timeElapsed = 0;
        speechBubbleGameObj = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canControl)
        {
            horizontal = vertical = 0;
            return;
        }
        
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");


        // capture keyboard events
        if (Input.GetKeyDown(KeyCode.Z))
        {
            InteractStart?.Invoke(this, new InteractArgs(CurrentChattableCharacter));
            Debug.Log("Currently talking to: " + CurrentChattableCharacter?.name);
        }

    }

    private void FixedUpdate()
    {
        Vector2 newPosition = new Vector2(transform.position.x + horizontal * Time.fixedDeltaTime * speed, transform.position.y + vertical * Time.fixedDeltaTime * speed);
        rb.MovePosition(newPosition);


        // reset
        animator.SetBool("r", false);
        animator.SetBool("l", false);
        animator.SetBool("u", false);
        animator.SetBool("d", false);

        if (horizontal > 0)
            animator.SetBool("r", true);
        else if (horizontal < 0)
            animator.SetBool("l", true);
        else if (vertical > 0)
            animator.SetBool("u", true);
        else if (vertical < 0)
            animator.SetBool("d", true);

        animator.SetBool("moving", horizontal != 0 || vertical != 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
        {
            if (spriteRenderer.sortingLayerName == "Player")
            {
                CurrentChattableCharacter = collision.gameObject;
                speechBubbleGameObj.SetActive(true);
            }

        }
        else if (collision.gameObject.TryGetComponent<SortingGroup>(out SortingGroup sortingGroup))
        {
            if (sortingGroup.sortingLayerName == "Player")
            {
                CurrentChattableCharacter = collision.gameObject;
                speechBubbleGameObj.SetActive(true);
            }
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        CurrentChattableCharacter = null;
        speechBubbleGameObj.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
        {
            if (spriteRenderer.sortingLayerName == "Player")
            {
                CurrentChattableCharacter = collision.gameObject;
                speechBubbleGameObj.SetActive(true);
            }
            
        }
        else if (collision.gameObject.TryGetComponent<SortingGroup>(out SortingGroup sortingGroup))
        {
            if (sortingGroup.sortingLayerName == "Player")
            {
                CurrentChattableCharacter = collision.gameObject;
                speechBubbleGameObj.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CurrentChattableCharacter = null;
        speechBubbleGameObj.SetActive(false);
    }
}
