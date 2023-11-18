using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaScript : MonoBehaviour
{
    void Start()
    {
        Globals.playerManager.OnArenaEnter();
    }
}
