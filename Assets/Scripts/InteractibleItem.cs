using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleItem : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D coll; //TODO handle multiple colliders ?
    private ScalableObject scObj;
    private Grabber grabber;

    public bool grabbable = true;
    public bool punchable = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        scObj = GetComponent<ScalableObject>();
        scObj.onScaleChanged.AddListener(OnScaleChanged);
    }

    public void Grab(Grabber grabber, Transform grabAnchor)
    {
        // Parent and move object to player grabber, and ignore collision with the grabber
        if (!grabbable) return;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        foreach (Collider2D grabberColl in grabber.playerObj.GetComponents<Collider2D>())
        {
            Physics2D.IgnoreCollision(coll, grabberColl);
        }
        this.grabber = grabber;
        transform.SetParent(grabAnchor);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        OnGrab(grabber);
    }

    public void Throw(Vector2 direction, float power, Vector2 offsetPos)
    {
        // Unparent object with grabber and throw it
        transform.localPosition = offsetPos; // offset item from grab point
        transform.SetParent(null);
        rb.isKinematic = false;
        foreach (Collider2D grabberColl in grabber.playerObj.GetComponents<Collider2D>()) // TODO ? handle better collision reactivation with player
        {
            Physics2D.IgnoreCollision(coll, grabberColl, false);
        }
        Grabber thrower = this.grabber;
        this.grabber = null;
        rb.AddRelativeForce(power * direction.normalized, ForceMode2D.Impulse);
        rb.AddTorque(2f * Mathf.Deg2Rad * Mathf.Sign(direction.x) * power, ForceMode2D.Impulse);

        OnThrow(thrower);
    }

    public void Punch(Vector2 direction, float power)
    {
        if (!punchable) return;
        //rb.AddForce(direction * power); // Commented for now, waiting for Punch mechanic implementation

        OnPunch(power);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<CharacterController>())
        {
            CharacterController player = collision.gameObject.GetComponent<CharacterController>();
            OnCollisionWithPlayer(player, collision);
        }
        else
        {
            OnCollision(collision);
        }
    }

    // ---------- Overridable methods ----------

    public void OnPunch(float punchPower)
    {
    }

    private void OnGrab(Grabber grabber)
    {
    }

    private void OnThrow(Grabber thrower)
    {
    }

    private void OnScaleChanged(GameObject that, float newScale)
    {
    }

    private void OnCollisionWithPlayer(CharacterController player, Collision2D collision) //TODO add a "Player" component to players instead of using the controller ?
    {
    }

    private void OnCollision(Collision2D collision)
    {
    }
}
