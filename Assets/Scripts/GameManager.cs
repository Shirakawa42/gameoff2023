using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public int nbPlayers = 2;
    private int[] scores;




    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialisation();
        DisplayScore();
    }


    private void Initialisation()
    {
        scores = new int[nbPlayers];
    }

    public void IncPlayerScore(int playerId)
    {
        scores[playerId]++;
        DisplayScore();
    }

    public void DisplayScore()
    {
        for (int i = 0; i < nbPlayers; i++)
        {

            Debug.Log("Player" + (i+1) + " > " + scores[i]);
        }
    }

}
