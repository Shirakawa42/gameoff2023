using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class PlayWwiseEvent : MonoBehaviour
{
    public AK.Wwise.Event[] events;

    public void Play(int i)
    {
        events[i].Post(gameObject);
    }
}
