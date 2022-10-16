using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class characterAnimScript : MonoBehaviour
{
    public dialogHandlerScript dialogHandlerScript;


    public Rigidbody2D rb;
    public Animator animator;

    public float speed = 5f;

    float timeElapsed;

    float horizontal;
    float vertical;

    public bool canControl = true;

    void Start()
    {
        timeElapsed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canControl)
            return;
        
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        rb.MovePosition(new Vector2(transform.position.x + horizontal * Time.fixedDeltaTime * speed, transform.position.y + vertical * Time.fixedDeltaTime * speed));


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
        if (collision.gameObject.name == "Character2")
        {
            dialogHandlerScript.Restart();
        }
    }
}
