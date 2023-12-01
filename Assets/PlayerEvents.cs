using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

public class PlayerEvents : MonoBehaviour
{
   public AK.Wwise.Event event1;
   public AK.Wwise.Event event2;
   public AK.Wwise.Event event3;
   public AK.Wwise.Event event4;
   public AK.Wwise.Event event5;

   public void PlaySizeDown()
   {
    event1.Post(gameObject);
   }

   public void PlaySizeUp()
   {
    event2.Post(gameObject);
   }

   public void PlaySmash()
   {
    event3.Post(gameObject);
   }

   public void PlayHit()
   {
    event4.Post(gameObject);
   }

}
