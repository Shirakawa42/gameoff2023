using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ultraBasicController : MonoBehaviour
{
    private PortalableObject portalableObject;

    private void Start()
    {
        portalableObject = GetComponent<PortalableObject>();
    }

    private void Update()
    {
        if (GetComponent<PortalableObject>().isClone)
            return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.velocity = new Vector2(-5, rb.velocity.y);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.velocity = new Vector2(5, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
}
