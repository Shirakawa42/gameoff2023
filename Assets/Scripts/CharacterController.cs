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
    public float jumpingPower = 5f;

    private Rigidbody2D rb;
    private float horizontal;
    private float maxSpeed = 6f;
    private float baseAcceleration = 0.2f;
    private bool isFacingRight = true;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsOnWall()
    {
        return Physics2D.OverlapCircle(wallCheckFront.position, 0.2f, groundLayer) || Physics2D.OverlapCircle(wallCheckBack.position, 0.2f, groundLayer);
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
        else if (context.performed && IsOnWall()) {
            if (isFacingRight)
                rb.velocity = new Vector2(-jumpingPower, jumpingPower);
            else
                rb.velocity = new Vector2(jumpingPower, jumpingPower);
        }

        if (context.canceled && rb.velocity.y > 0)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
    }

    void Update()
    {
        float acceleration = baseAcceleration;
        if (IsGrounded())
            acceleration *= 3f;

        if ((rb.velocity.x < maxSpeed && horizontal < 0) || (rb.velocity.x > -maxSpeed && horizontal > 0)) {
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


        if ((!isFacingRight && horizontal > 0) || (isFacingRight && horizontal < 0))
            Flip();
    }
}
