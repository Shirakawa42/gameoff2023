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

    // Projectile shooter
    public ResizeProjectileShooter projShooter;
    public Transform projShooterPivot;
    // Grabber
    public Grabber grabber;

    private Rigidbody2D rb;
    private ScalablePlayer scalablePlayer;
    private float horizontal;
    private float maxSpeed = 6f;
    private float baseAcceleration = 20f;
    private float jumpingPower = 5f;
    private bool isFacingRight = true;
    public Vector2 aimDir { get; private set; }
    private bool inAimMode = false;
    private PlayerInput playerInput;
    private bool isMouse = false;

    public List<Sprite> playerskins;


    // smash part
    private SmashDetection smashDetection;
    const float smashedMinSpeed = 6f; // Exit "smashed" state when speed goes under this value
    public float decelerationOnSmash = 10f; //Speed lost per second when ejected by smash

    /* 
     * while (isSmashed)
     *      if (rb.hitSomething)
     *          if (objHitted is wall && rb.velocity > speedToDie)
     *              die();
     */

    private void Awake()
    {
        scalablePlayer = GetComponent<ScalablePlayer>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        smashDetection = GetComponentInChildren<SmashDetection>();
        playerInput = GetComponent<PlayerInput>();
        isMouse = playerInput.currentControlScheme == "Keyboard&Mouse";
        DontDestroyOnLoad(gameObject);
        GetComponent<SpriteRenderer>().sprite = playerskins[playerInput.playerIndex];
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
        if (!context.performed) return;

        smashDetection.Smash();
    }

    private void MouseLook()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector2 directionToMouse = mouseWorldPosition - projShooterPivot.position;
        aimDir = directionToMouse.normalized;
        float aimAngle = Vector2.SignedAngle(Vector2.right, aimDir);
        projShooterPivot.rotation = Quaternion.Euler(0f, 0f, aimAngle);
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (isMouse)
        {
            MouseLook();
            return;
        }

        // Aim left/right idle rotation
        if (context.canceled && !inAimMode && !isMouse)
        {
            projShooterPivot.rotation = Quaternion.Euler(0f, 0f, isFacingRight ? 0f : 180f);
            aimDir = isFacingRight ? Vector2.right : Vector2.left;
            return;
        }

        aimDir = context.ReadValue<Vector2>();
        float aimAngle = Vector2.SignedAngle(Vector2.right, aimDir);
        // Not in aim mode -> snap to 45 degrees
        //if (!inAimMode)
        //{
        //    aimAngle = Snapping.Snap(aimAngle, 45f);
        //}
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
        if (grabber.IsGrabbingObj()) return;

        projShooter.ShootSizeUp();
    }

    public void ShootProjSizeDown(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (grabber.IsGrabbingObj()) return;

        projShooter.ShootSizeDown();
    }

    void FixedUpdate()
    {
        float acceleration = baseAcceleration * Time.deltaTime;

        if (IsGrounded())
            acceleration *= 3f;

        if (!scalablePlayer.isSmashed) // if the player isn't smashed then do regular movement
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
        else // "Smashed" state
        {
            CheckSmashOnWall();
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, rb.velocity.magnitude - decelerationOnSmash * Time.deltaTime);
            if (rb.velocity.magnitude < smashedMinSpeed)
            {
                rb.velocity = new Vector2(0, 0);
                scalablePlayer.QuitSmashState();
            }
        }

        if (isMouse)
            MouseLook();

        //Debug.Log(rb.velocity.sqrMagnitude);

        if ((!isFacingRight && horizontal > 0) || (isFacingRight && horizontal < 0))
            Flip();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (scalablePlayer.isSmashed)
        {
            //?
        }
    }

    /**
     * 
     **/
    private void CheckSmashOnWall()
    {
        if ((IsFrontOnWall() || IsBackOnWall()) && rb.velocity.sqrMagnitude > scalablePlayer.speedToDie)
        {
            scalablePlayer.Die();
        }
    }
}
