using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeProjectileShooter : MonoBehaviour
{
    [Tooltip("Cooldown between shots (seconds)")]
    public float cooldown = 2f;
    // Projectile prefab
    public GameObject projectilePrefab;
    public GameObject shooterParent; // Parent of this projectile, so it will be temporarly ignored by its own projectiles
    public bool preventCollisionWithShooterOnSpawn = true;

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
        ResizeProjectile projectile = Instantiate(projectilePrefab, transform.position, transform.rotation).GetComponent<ResizeProjectile>();
        projectile.SetScaleUpOnImpact(sizeUp);
        resetCooldown();

        // Prevent immediate collision with shooter -> assign this shooter to the projectile
        if (preventCollisionWithShooterOnSpawn)
        {
            projectile.SetShooter(shooterParent);
        }
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
