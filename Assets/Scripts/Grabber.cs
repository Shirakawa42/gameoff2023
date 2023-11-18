using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grabber : MonoBehaviour
{
    //static private Vector2 ThrowDir = new Vector2(1f, .5f).normalized;

    private InteractibleItem grabbedItem;
    private CharacterController playerController;

    public float grabRange = .2f;
    public Vector2 throwOffset = new Vector2(0, 0);
    [Tooltip("Scalable player object using this grabber")]
    public ScalablePlayer player;
    // TODO ? cooldown to avoid grab action spam

    private void Awake()
    {
        playerController = player.GetComponent<CharacterController>();
        player.OnScaleChanged.AddListener(onPlayerScaleChanged);
    }

    // Grab a nearby item if possible
    public void Grab(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (grabbedItem)
        {
            ThrowItem();
            return;
        }

        grabbedItem = getNearestGrabbableItem();
        if (!grabbedItem)
        {
            return; //TODO put Grab action on cooldown
        }

        grabbedItem.Grab(this, transform);// TODO ? custom grabAnchor transform
    }

    public bool IsGrabbingObj()
    {
        return grabbedItem != null;
    }

    public bool CanGrab(InteractibleItem objToGrab)
    {
        return player.strenght >= objToGrab.GetMass(); //TODO rework calculation ?
    }

    private InteractibleItem getNearestGrabbableItem()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, grabRange);
        InteractibleItem nearestItem = null;

        float minItemDistance = float.PositiveInfinity;
        InteractibleItem itemToCheck;
        float distanceToCheck;
        foreach (Collider2D coll in colls)
        {
            itemToCheck = coll.gameObject.GetComponent<InteractibleItem>();
            if (!itemToCheck || !itemToCheck.grabbable) continue;
            // Can player grab this item (ex: enough strenght) ?
            if (!CanGrab(itemToCheck)) continue;


            distanceToCheck = Vector2.Distance(transform.position, itemToCheck.transform.position);
            if (distanceToCheck < minItemDistance)
            {
                nearestItem = itemToCheck;
                minItemDistance = distanceToCheck;
            }
        }

        return nearestItem;
    }

    private void ThrowItem()
    {
        Vector3 vecOffset = throwOffset;
        float aimAngle = Vector2.SignedAngle(Vector2.right, playerController.aimDir);
        vecOffset = Quaternion.Euler(0f, 0f, aimAngle) * vecOffset;
        grabbedItem.Throw(playerController.aimDir, 5, playerController.transform.position + vecOffset); // TODO Use scalable player strenght for 'power' param
        grabbedItem = null;
    }

    public void ReleaseItem()
    {
        grabbedItem.Release(transform.position + new Vector3(throwOffset.x, throwOffset.y, 0));
        grabbedItem = null;
    }

    private void onPlayerScaleChanged(GameObject playerGameObj, float newScale)
    {
        // Player scale changed -> check if he can keep holding the object
        if (IsGrabbingObj() && !CanGrab(grabbedItem))
        {
            ReleaseItem();
        }
    }

    private void Update()
    {
        if (grabbedItem)
        {
            grabbedItem.transform.position = transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}
