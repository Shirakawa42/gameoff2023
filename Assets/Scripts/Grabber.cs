using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grabber : MonoBehaviour
{
    //static private Vector2 ThrowDir = new Vector2(1f, .5f).normalized;

    private InteractibleItem grabbedItem;

    public float grabRange = .2f;
    public Vector2 throwOffset = new Vector2(0, 0);
    [Tooltip("Player object using this grabber")]
    public GameObject playerObj;
    public CharacterController playerController;
    // TODO ? cooldown to avoid grab action spam

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
        //Vector2 throwDirection = new Vector2(ThrowDir.x * Mathf.Sign(playerObj.transform.localScale.x), ThrowDir.y); //TODO use isFacingRight
        grabbedItem.Throw(playerController.aimDir, 5, throwOffset);
        grabbedItem = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}
