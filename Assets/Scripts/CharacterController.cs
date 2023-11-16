using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    public Transform groundCheck;
    public Transform wallCheckFront;
    public Transform wallCheckBack;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float horizontal;
    private float maxSpeed = 6f;
    private float baseAcceleration = 20f;
    private float jumpingPower = 5f;
    private bool isFacingRight = true;


    // smash part
    private SmashDetection smashDetection;
    public float smashIntensity;
    private bool isSmashed = false;

    public float speedToDie;

    /* 
     * while (isSmashed)
     *      if (rb.hitSomething)
     *          if (objHitted is wall && rb.velocity > speedToDie)
     *              die();
     */
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        smashDetection = GetComponentInChildren<SmashDetection>();
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsFrontOnWall()
    {
        return Physics2D.OverlapCircle(wallCheckFront.position, 0.2f, groundLayer);
    }

    private bool IsBackOnWall()
    {
        return Physics2D.OverlapCircle(wallCheckBack.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        else if (context.performed && IsFrontOnWall())
        {
            if (isFacingRight)
                rb.velocity = new Vector2(-jumpingPower, jumpingPower);
            else
                rb.velocity = new Vector2(jumpingPower, jumpingPower);
        }
        else if (context.performed && IsBackOnWall())
        {
            if (isFacingRight)
                rb.velocity = new Vector2(jumpingPower, jumpingPower);
            else
                rb.velocity = new Vector2(-jumpingPower, jumpingPower);
        }

        if (context.canceled && rb.velocity.y > 0)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            //Debug.Log("Fire!");
            if (smashDetection.hitSomething)
            {
                Vector2 smashDir = new Vector2(smashDetection.target.transform.position.x - transform.position.x, smashDetection.target.transform.position.y - transform.position.y);
                Rigidbody2D targetRb = smashDetection.target.GetComponent<Rigidbody2D>();
                targetRb.AddForce(smashDir * smashIntensity, ForceMode2D.Impulse);
                smashDetection.target.GetComponent<CharacterController>().isSmashed = true;
            }
            
        }
    }

    void Update()
    {
        float acceleration = baseAcceleration * Time.deltaTime;
       
        if (IsGrounded())
            acceleration *= 3f;

        if (!isSmashed) // if the player isn't smashed then do regular movement
        {
            if ((rb.velocity.x < maxSpeed && horizontal > 0) || (rb.velocity.x > -maxSpeed && horizontal < 0))
            {
                float newSpeed = rb.velocity.x + (horizontal * acceleration);
                if (newSpeed > maxSpeed)
                    rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
                else if (newSpeed < -maxSpeed)
                    rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
                else
                    rb.velocity = new Vector2(rb.velocity.x + (horizontal * acceleration), rb.velocity.y);
            }
            else if (horizontal == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x * (1f - acceleration), rb.velocity.y);
                if (rb.velocity.x < 0.1f && rb.velocity.x > -0.1f)
                    rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        else
        {
            CheckSmashOnWall();
            rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y); // decrease velocity slower than regular movement
            if (rb.velocity.x < 0.1f && rb.velocity.x > -0.1f)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                isSmashed = false;
            }
        }

        //Debug.Log(rb.velocity.sqrMagnitude);
        
        if ((!isFacingRight && horizontal > 0) || (isFacingRight && horizontal < 0))
            Flip();
    }


    private void CheckSmashOnWall()
    {
        if ((IsGrounded() || IsFrontOnWall() || IsBackOnWall()) && rb.velocity.sqrMagnitude > speedToDie)
        {
            Debug.Log("You die!");
        }
    }
}
