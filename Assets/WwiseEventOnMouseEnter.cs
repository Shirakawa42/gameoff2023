using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class WwiseEventOnMouseEnter : MonoBehaviour
{
    public AK.Wwise.Event eventWwise;

    private void OnMouseEnter()
    {
        // Code à exécuter lorsque la souris survole le GameObject
        Debug.Log("Souris survolant l'objet");

        // Déclencher l'événement Wwise
        eventWwise.Post(gameObject);
    }

}