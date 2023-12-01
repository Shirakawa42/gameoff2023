using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BeatPulse : MonoBehaviour
{

    public float BPM = 100f;
    //public Color StartColor;
    //public Color EndColor;
    //private Material mat;
    public Light2D blink;

    // Use this for initialization
    void Start()
    {
        blink = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var baseValue = Mathf.Cos(((Time.time * Mathf.PI) * (BPM / 60f)) % Mathf.PI);
        blink.intensity = Mathf.Lerp(8, 16, baseValue);
        //mat.SetColor("_EmissionColor", target);
    }
}
