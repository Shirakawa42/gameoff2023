using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeProjectile : MonoBehaviour
{
    static private float speed = 8f;
    static private float scaleRateModifier = 2;
    static private Color scaleUpColor = Color.yellow;
    static private Color scaleDownColor = Color.cyan;
    static private float lifetime = 4f; // Seconds before the projectile dissapear (if no scalable target had been touched)

    // If true the touched ScalableObject will be scaled up, else scaled down
    private bool scaleUpOnImpact = true;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private float currentLifetime = lifetime;
    private GameObject shooter = null; // The shooter of this projectile, to ignore collision when this projectile just spawned (Optional)
    
    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = scaleUpOnImpact ? scaleUpColor : scaleDownColor;
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetScaleUpOnImpact(bool doScaleUp)
    {
        scaleUpOnImpact = doScaleUp;
        sprite.color = scaleUpOnImpact ? scaleUpColor : scaleDownColor;
    }

    public void SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Hit geometry -> bounce on it
        Vector2 newDir = Vector2.Reflect(transform.right, collision.GetContact(0).normal);
        transform.rotation = Quaternion.FromToRotation(Vector2.right, newDir);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Avoid immediate collision with the shooter of this projectile
        if (shooter && collision.gameObject == shooter) return;

        // Hit scalable object -> resize it
        if (collision.gameObject.GetComponent<ScalableObject>())
        {
            ScalableObject obj = collision.gameObject.GetComponent<ScalableObject>();
            if (scaleUpOnImpact)
            {
                obj.ScaleUp(scaleRateModifier);
            }
            else
            {
                obj.ScaleDown(scaleRateModifier);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Remove temporary collision ignore with the shooter of this projectile
        if (shooter) shooter = null;
    }

    void FixedUpdate()
    {
        currentLifetime -= Time.deltaTime;
        if (currentLifetime <= 0)
        {
            Destroy(gameObject);
            return;
        }

        rb.MovePosition(transform.position + transform.right * speed * Time.deltaTime);
    }
}
