using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class OnScaleChangedEvent : UnityEvent<GameObject, float> {}

public class ScalableObject : MonoBehaviour
{
    protected enum ScaleChange { Up, Down, Unchanged }

    // The base Rigidbody mass value of this GameObject
    private float rbBaseMass = 0f;
    // The base scale value of this GameObject
    private Vector3 objBaseScale;
    protected float currentScale = 1.0f;
    private Rigidbody2D rb; // Optional

    public float maxScale = 2f;
    public float minScale = 0.5f;
    [Tooltip("At which scale this object will spawn (automatically set scale, rigidbody mass, etc...  on spawn)")]
    public float startingScale = 1;

    [Tooltip("Event called when a new scale had been applied to this object")]
    public OnScaleChangedEvent OnScaleChanged;
    [Tooltip("Event called when this object had been scaled up")]
    public OnScaleChangedEvent OnScaleUp;
    [Tooltip("Event called when this object had been scaled down")]
    public OnScaleChangedEvent OnScaleDown;

    // Start is called before the first frame update
    void Start()
    {
        objBaseScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        if (rb) rbBaseMass = rb.mass;

        if (startingScale > 0 && startingScale != 1)
        {
            HandleScaleChange(SetScale(startingScale));
        }
    }

    public void ScaleUp(float scaleRate)
    {
        HandleScaleChange(SetScale(currentScale * scaleRate));
    }

    public void ScaleDown(float scaleRate)
    {
        HandleScaleChange(SetScale(currentScale / scaleRate));
    }

    // Return true if scale had been changed
    protected virtual ScaleChange SetScale(float newScale)
    {
        // Check if new scale is in min/max range
        if (newScale < minScale || newScale > maxScale) return ScaleChange.Unchanged;

        float scaleToApply = Mathf.Clamp(newScale, minScale, maxScale);
        if (scaleToApply == currentScale) return ScaleChange.Unchanged;

        ScaleChange scaleChange = (newScale > currentScale) ? ScaleChange.Up : ScaleChange.Down;

        currentScale = scaleToApply;
        if (rb) rb.mass = rbBaseMass * currentScale;

        if (transform.parent) // Ensures scale is recalculated in world space in case if the object is parented to something (ex: obj grabbed by player)
        {
            transform.localScale = Vector3.Scale(objBaseScale * currentScale, new Vector3(
                1f / transform.parent.lossyScale.x,
                1f / transform.parent.lossyScale.y,
                1f / transform.parent.lossyScale.z)
            );
        }
        else
        {
            transform.localScale = objBaseScale * currentScale;
        }

        return scaleChange;
    }

    private void HandleScaleChange(ScaleChange scaleChange)
    {
        if (scaleChange == ScaleChange.Unchanged) return;

        // Broadcast event(s)
        OnScaleChanged.Invoke(gameObject, currentScale);
        if (scaleChange == ScaleChange.Up)
        {
            OnScaleUp.Invoke(gameObject, currentScale);
        }
        else
        {
            OnScaleDown.Invoke(gameObject, currentScale);
        }
    }

    public void ResetScale()
    {
        HandleScaleChange(SetScale(startingScale));
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
