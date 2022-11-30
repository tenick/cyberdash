using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomCharacter : MonoBehaviour
{
    public float speed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float horizontal;
    float vertical;

    public Rigidbody2D rb;
    public Animator animator;

    float randomCD;

    // Update is called once per frame
    void Update()
    {
        if (Time.time > randomCD)
        {
            randomCD = Time.time + Random.Range(1f, 3f);
            horizontal = Random.Range(-1, 2);
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
}
