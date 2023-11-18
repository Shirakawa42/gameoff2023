using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalablePlayer : ScalableObject
{
    // When player scale is modified, it will be reset after X seconds
    private const float ScaleResetCooldown = 4f;
    // Starting strenght
    private const float StartingStrenght = 1f;

    private float scaleResetTime = 0f;
    private bool onScaleResetCooldown = false;
    // Strenght, used for grab, throw and punch
    public float strenght { get; private set; } = StartingStrenght;

    protected override ScaleChange SetScale(float newScale)
    {
        ScaleChange scaleChange = base.SetScale(newScale);

        if (scaleChange != ScaleChange.Unchanged) updateStatsOnScale(newScale);

        // new scale is different than base scale -> start reset cooldown, stop it otherwise
        if (currentScale == startingScale)
        {
            stopScaleResetCooldown();
        }
        else
        {
            startScaleResetCooldown();
        }

        return scaleChange;
    }

    private void updateStatsOnScale(float newScale)
    {
        strenght = StartingStrenght * newScale;
    }

    private void startScaleResetCooldown()
    {
        scaleResetTime = ScaleResetCooldown;
        onScaleResetCooldown = true;
    }

    private void stopScaleResetCooldown()
    {
        onScaleResetCooldown = false;
    }

    private void Update()
    {
        // Scale reset cooldown
        if (onScaleResetCooldown)
        {
            scaleResetTime -= Time.deltaTime;
            if (scaleResetTime <= 0)
            {
                ResetScale();
            }
        }
    }
}
