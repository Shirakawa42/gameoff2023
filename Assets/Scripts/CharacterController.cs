using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    public Transform groundCheck;
    public Transform wallCheckFront;
    public Transform wallCheckBack;
    public LayerMask groundLayer;

    public ResizeProjectileShooter projShooter;
    public Transform projShooterPivot;

    private Rigidbody2D rb;
    private float horizontal;
    private float maxSpeed = 6f;
    private float baseAcceleration = 20f;
    private float jumpingPower = 5f;
    private bool isFacingRight = true;
    private Vector2 aimDir;
    private bool inAimMode = false;


    // mutliplayer data
    public int idPlayer;

    // size scale aspect
    [Range(0, 4)] public int sizeScale;
    [SerializeField] private int maxGap = 4; // peut être placer dans le GameManager ?

    // smash part
    private SmashDetection smashDetection;
    public float smashInitForce;
    private float smashIntensity;
    private bool isSmashed = false;
    private int idSmasher;
    
    public float smashCooldown; // in seconds
    private bool canSmash = true;

    /* 
     * while (isSmashed)
     *      if (rb.hitSomething)
     *          if (objHitted is wall && rb.velocity > speedToDie)
     *              die();
     */

    void Start()
    {
        smashInitForce = 1000;

        idPlayer = int.Parse(transform.name.Substring(6)) - 1;
        Debug.Log(idPlayer);

        rb = GetComponent<Rigidbody2D>();
        smashDetection = GetComponentInChildren<SmashDetection>();
        DontDestroyOnLoad(gameObject);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    private bool IsFrontOnWall()
    {
        return Physics2D.OverlapCircle(wallCheckFront.position, 0.1f, groundLayer);
    }

    private bool IsBackOnWall()
    {
        return Physics2D.OverlapCircle(wallCheckBack.position, 0.1f, groundLayer);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localRotation = Quaternion.Euler(0, isFacingRight ? 0 : 180, 0);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (inAimMode) return;

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

    public void Smash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (canSmash && smashDetection.hitSomething)
            {
                GameObject target = smashDetection.target;
                float sizeGap = sizeScale - target.GetComponent<CharacterController>().sizeScale;
                smashIntensity = smashInitForce * (1.2f * Mathf.Exp(0.7f * sizeGap));

                //Debug.Log("sizeGap : " + sizeGap + "\n" +
                //            "Initial Force : " + smashInitForce + "\n" +
                //            " = Intensity : " +  smashIntensity);

                if (sizeGap >= maxGap)
                {
                    Debug.Log("Ultra Smash ! Launch Automatic Win Procedure");
                    smashDetection.target.GetComponent<CharacterController>().Die();
                }

                Vector2 smashDir = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y);

                target.GetComponent<Rigidbody2D>().AddForce(smashDir * smashIntensity, ForceMode2D.Impulse);

                smashDetection.target.GetComponent<CharacterController>().isSmashed = true;
                smashDetection.target.GetComponent<CharacterController>().idSmasher = idPlayer;

                canSmash = false;
                StartCoroutine(CooldownCoroutine());
            }

        }
    }


    public void Look(InputAction.CallbackContext context)
    {
        // TODO mouse pointer aim
        // Aim left/right idle rotation
        if (context.canceled)
        {
            projShooterPivot.rotation = Quaternion.Euler(0f, 0f, isFacingRight ? 0f : 180f);
            return;
        }

        aimDir = context.ReadValue<Vector2>();
        float aimAngle = Vector2.SignedAngle(Vector2.right, aimDir);
        // Not in aim mode -> snap to 45 degrees
        if (!inAimMode)
        {
            aimAngle = Snapping.Snap(aimAngle, 45f);
        }
        projShooterPivot.rotation = Quaternion.Euler(0f, 0f, aimAngle);
    }

    // Lock player in place to allow precise aim with joystick (Gamepad only)
    public void AimMode(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            inAimMode = true;
            horizontal = 0f;
        }
        if (context.canceled) inAimMode = false;
    }

    public void ShootProjSizeUp(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        projShooter.ShootSizeUp();
    }

    public void ShootProjSizeDown(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        projShooter.ShootSizeDown();
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
            rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y); // decrease velocity slower than regular movement
            if (rb.velocity.x < 0.1f && rb.velocity.x > -0.1f)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                isSmashed = false;
                idSmasher = idPlayer;
            }
        }

        //Debug.Log(rb.velocity.sqrMagnitude);
        
        if ((!isFacingRight && horizontal > 0) || (isFacingRight && horizontal < 0))
            Flip();
    }

    /**
     * 
     **/
    public void Die()
    {
        
        Debug.Log("You die!");
        if (idSmasher != idPlayer)
            GameManager.Instance.IncPlayerScore(idSmasher);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Lethal")
        {
            Die();
        }
    }




    /**
     * Smash Cooldown
     **/
    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(smashCooldown);

        canSmash = true;
    }
}
