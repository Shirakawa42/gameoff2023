using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalablePlayer : ScalableObject
{
    // When player scale is modified, it will be reset after X seconds
    private const float ScaleResetCooldown = 4f;

    private float scaleResetTime = 0f;
    private bool onScaleResetCooldown = false;

    //code dégeu
    public CharacterController characterController;
     

    private void Awake()
    {

    }

    public override bool SetScale(float newScale)
    {
        if (currentScale < newScale)
            characterController.sizeScale++;
        else
            characterController.sizeScale--;

        bool scaleChanged = base.SetScale(newScale);

        if (scaleChanged) updateStatsOnScale(newScale);

        // new scale is different than base scale -> start reset cooldown, stop it otherwise
        if (currentScale == startingScale)
        {
            stopScaleResetCooldown();
        }
        else
        {
            startScaleResetCooldown();
        }

        return scaleChanged;
    }

    private void updateStatsOnScale(float newScale)
    {
        //TODO
             

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
