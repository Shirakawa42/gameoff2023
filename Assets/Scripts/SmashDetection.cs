using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashDetection : MonoBehaviour
{
    public bool hitSomething { get; set; } = false;
    public GameObject target;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (ObjIsHittable(collision))
            {
                hitSomething = true;
                target = collision.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hitSomething = false;
        target = null;
    }

    private bool ObjIsHittable(Collider2D collision)
    {
        return collision.tag == "Player";
    }
}
