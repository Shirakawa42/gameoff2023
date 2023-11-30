using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    public enum GameState { OnMenu, OnArena };

    public static GameState gameState = GameState.OnMenu;
    public static PlayerManager playerManager;
    public static HUD hud;

    public static OvelayManager ovelayManager;
}
