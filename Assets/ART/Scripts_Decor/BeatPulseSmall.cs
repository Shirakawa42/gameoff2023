using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BeatPulseSmall : MonoBehaviour
{

    public float BPM = 100f;
    //public Color StartColor;
    //public Color EndColor;
    //private Material mat;
    public Light2D blinksmall;

    // Use this for initialization
    void Start()
    {
        blinksmall = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var baseValue = Mathf.Cos(((Time.time * Mathf.PI) * (BPM / 60f)) % Mathf.PI);
        blinksmall.intensity = Mathf.Lerp(4, 10, baseValue);
        //mat.SetColor("_EmissionColor", target);
    }
}
