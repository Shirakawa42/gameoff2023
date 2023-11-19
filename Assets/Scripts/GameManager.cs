using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI ui_score;

    public int nbPlayers = 2;
    private int[] scores;




    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);

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

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void DisplayScore()
    {
        ui_score.text = "Marouflez!!!\r\nJ1 > " + scores[0] + " <||> " + scores[1] + " < J2";
        for (int i = 0; i < nbPlayers; i++)
        {
            Debug.Log("Player" + (i+1) + " > " + scores[i]);
        }
    }

}
