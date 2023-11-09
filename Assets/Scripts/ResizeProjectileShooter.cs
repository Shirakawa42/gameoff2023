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

    public void ShootSizeUp(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (onCooldown) return;

        SpawnProjectile(spawnPosition, spawnRotation, true);
    }

    public void ShootSizeDown(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (onCooldown) return;

        SpawnProjectile(spawnPosition, spawnRotation, false);
    }

    private void SpawnProjectile(Vector3 spawnPosition, Quaternion spawnRotation, bool sizeUp)
    {
        ResizeProjectile projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation).GetComponent<ResizeProjectile>();
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
