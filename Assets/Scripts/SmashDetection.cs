using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashDetection : MonoBehaviour
{
    public float smashCooldown = 1.5f;

    private ScalablePlayer player;
    private bool canSmash = true;
    private List<GameObject> targets = new List<GameObject>();

    private void Awake()
    {
        player = transform.parent.GetComponent<ScalablePlayer>();
    }

    public void Smash()
    {
        if (!canSmash || targets.Count <= 0) return;
        
        foreach (GameObject targetObj in targets)
        {
            Vector2 smashDir = new Vector2(targetObj.transform.position.x - player.transform.position.x, targetObj.transform.position.y - player.transform.position.y);

            if (targetObj.GetComponent<ScalablePlayer>())
            {
                targetObj.GetComponent<ScalablePlayer>().Smash(player, smashDir, player.strenght);
            }
            else if (targetObj.GetComponent<InteractibleItem>())
            {
                //obj.GetComponent<InteractibleItem>().Punch(playerController.) //TODO
            }
        }

        canSmash = false;
        StartCoroutine(CooldownCoroutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (objIsHittable(collision))
        {
            targets.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        targets.Remove(collision.gameObject);
    }

    private bool objIsHittable(Collider2D collision)
    {
        return collision.GetComponent<ScalablePlayer>() || collision.GetComponent<InteractibleItem>();
    }

    /**
     * Smash Cooldown
     **/
    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(smashCooldown);
        canSmash = true;
    }
}
