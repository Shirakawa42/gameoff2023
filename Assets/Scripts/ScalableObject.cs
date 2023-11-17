using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class OnScaleChangedEvent : UnityEvent<GameObject, float> {}

public class ScalableObject : MonoBehaviour
{
    // The base Rigidbody mass value of this GameObject
    private float rbBaseMass;
    // The base scale value of this GameObject
    private Vector3 objBaseScale;
    protected float currentScale = 1.0f;
    private Rigidbody2D rb;

    public float maxScale = 2f;
    public float minScale = 0.5f;
    [Tooltip("At which scale this object will spawn (automatically set scale, rigidbody mass, etc...  on spawn)")]
    public float startingScale = 1;

    [Tooltip("Event called when a new scale had been applied to this object")]
    public OnScaleChangedEvent onScaleChanged;
    [Tooltip("Event called when this object had been scaled up")]
    public OnScaleChangedEvent onScaleUp;
    [Tooltip("Event called when this object had been scaled down")]
    public OnScaleChangedEvent onScaleDown;

    // Start is called before the first frame update
    void Start()
    {
        objBaseScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        rbBaseMass = rb.mass;

        if (startingScale > 0 && startingScale != 1)
        {
            SetScale(startingScale);
        }
    }

    public void ScaleUp(float scaleRate)
    {
        if (SetScale(currentScale * scaleRate))
        {
            onScaleUp.Invoke(gameObject, currentScale);
        }
    }

    public void ScaleDown(float scaleRate)
    {
        if (SetScale(currentScale / scaleRate))
        {
            onScaleDown.Invoke(gameObject, currentScale);
        }
    }

    // Return true if scale had been changed
    public virtual bool SetScale(float newScale)
    {
        // Check if new scale is in min/max range
        if (newScale < minScale || newScale > maxScale) return false;

        float scaleToApply = Mathf.Clamp(newScale, minScale, maxScale);
        if (scaleToApply == currentScale) return false;

        currentScale = scaleToApply;
        rb.mass = rbBaseMass * currentScale;
        transform.localScale = objBaseScale * currentScale;
        onScaleChanged.Invoke(gameObject, currentScale);

        return true;
    }

    public void ResetScale()
    {
        SetScale(startingScale);
    }

    public float getCurrentScale()
    {
        return currentScale;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw collider bounding box, based on startingScale
        //TODO calculate bounds of multiple colliders
        if (startingScale == 1) return;
        Collider2D coll = GetComponent<Collider2D>();
        if (!coll) return;

        Bounds collBounds = coll.bounds;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(collBounds.center, collBounds.size * startingScale);
    }
}
