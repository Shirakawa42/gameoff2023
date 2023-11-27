using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

// player 1: killed player
// player 2: killer player (optional)
public class OnPlayerDiedEvent : UnityEvent<ScalablePlayer, ScalablePlayer> { }

public class ScalablePlayer : ScalableObject
{
    // When player scale is modified, it will be reset after X seconds
    private const float ScaleResetCooldown = 4f;
    // Starting strenght
    private const float StartingStrenght = 5f;

    private float scaleResetTime = 0f;
    private bool onScaleResetCooldown = false;
    public bool isSmashed = false;
    // Strenght, used for grab, throw and punch
    public float strenght { get; private set; } = StartingStrenght;
    public float speedToDie = 12f;
    public ScalablePlayer smashedBy { get; private set; } = null;
    public OnPlayerDiedEvent OnPlayerDied = new OnPlayerDiedEvent();
    public int playerIndex = -1;

    public void Smash(ScalablePlayer smasher, Vector2 smashDir, float power)
    {
        rb.AddForce(smashDir * power, ForceMode2D.Impulse);
        isSmashed = true;
        smashedBy = smasher;
    }

    public void QuitSmashState()
    {
        isSmashed = false;
        smashedBy = null;
    }

    public void Die()
    {
        OnPlayerDied.Invoke(this, smashedBy);
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        ResetScale();
        QuitSmashState();
        rb.velocity = Vector2.zero;
    }

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
