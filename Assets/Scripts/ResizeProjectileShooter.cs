using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeProjectileShooter : MonoBehaviour
{
    [Tooltip("Cooldown between shots (seconds)")]
    public float cooldown = 2f;
    // Projectile prefab
    public GameObject projectilePrefab;

    private float cooldownTime = 0f;
    private bool onCooldown = false;

    public void ShootSizeUp()
    {
        if (onCooldown) return;

        SpawnProjectile(true);
    }

    public void ShootSizeDown()
    {
        if (onCooldown) return;

        SpawnProjectile(false);
    }

    private void SpawnProjectile(bool sizeUp)
    {
        //TODO avoid immediate collision with the shooter (deactivate until the shooter left the projectile trigger on spawn ?)
        ResizeProjectile projectile = Instantiate(projectilePrefab, transform.position, transform.rotation).GetComponent<ResizeProjectile>();
        projectile.SetScaleUpOnImpact(sizeUp);
        resetCooldown();
    }

    private void resetCooldown()
    {
        cooldownTime = cooldown;
        onCooldown = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Update cooldown
        if (onCooldown)
        {
            cooldownTime -= Time.deltaTime;
            if (cooldownTime <= 0)
            {
                onCooldown = false;
            }
        }
    }
}
