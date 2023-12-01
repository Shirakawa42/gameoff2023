using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class PlayMenuMusic : MonoBehaviour
{
    public AK.Wwise.Event menuMusic;
    // Start is called before the first frame update
    void Start()
    {
        menuMusic.Post(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
