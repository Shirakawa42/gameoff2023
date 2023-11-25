using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeItem : MonoBehaviour
{
    static private float scaleRateModifier = 2;
    static private Color scaleUpColor = Color.yellow;
    static private Color scaleDownColor = Color.cyan;

    public bool scaleUp = true;
    public bool doRespawn = true;
    public float respawnTime = 6f;

    private SpriteRenderer sprite;
    private Collider2D coll;
    private bool isEnabled = true;
    private float timeBeforeRespawn;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = scaleUp ? scaleUpColor : scaleDownColor;
        coll = GetComponent<Collider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (doRespawn && !isEnabled)
        {
            timeBeforeRespawn -= Time.deltaTime;
            if (timeBeforeRespawn <= 0)
            {
                setEnabled(true);
            }
        }
    }

    private void setEnabled(bool enable)
    {
        Color newSpriteColor = sprite.color;
        newSpriteColor.a = enable ? 1f : 0.2f;
        sprite.color = newSpriteColor;

        coll.enabled = enable;
        isEnabled = enable;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<ScalableObject>())
        {
            ScalableObject obj = collision.GetComponent<ScalableObject>();
            if (scaleUp)
            {
                obj.ScaleUp(scaleRateModifier);
            }
            else
            {
                obj.ScaleDown(scaleRateModifier);
            }

            setEnabled(false);
            if (doRespawn) timeBeforeRespawn = respawnTime;
        }
    }
}
