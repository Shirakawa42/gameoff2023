using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalableObject : MonoBehaviour
{
    public GameObject original;
    public GameObject clone;
    public bool isClone = false;
    public Rigidbody2D connectedRigidbody; 

    public void Init(bool isClone, GameObject original, GameObject clone, Rigidbody2D connectedRigidbody)
    {
        this.isClone = isClone;
        this.original = original;
        this.clone = clone;
        this.connectedRigidbody = connectedRigidbody;
    }

    private void SyncRigidbodies()
    {
        Rigidbody2D originalRigidbody = original.GetComponent<Rigidbody2D>();
        Rigidbody2D cloneRigidbody = clone.GetComponent<Rigidbody2D>();
        cloneRigidbody.velocity = new Vector2(-originalRigidbody.velocity.x, originalRigidbody.velocity.y);
        cloneRigidbody.angularVelocity = originalRigidbody.angularVelocity;
    }

    private void FixedUpdate() {
        if (isClone)
        {
            SyncRigidbodies();
        }
    }
}
