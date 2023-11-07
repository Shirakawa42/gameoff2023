using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        Globals.portalManager = GetComponent<PortalManager>();
    }
}
