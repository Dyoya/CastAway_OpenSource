using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObtainableObject : MonoBehaviour
{
    //private int hp = 10;
    public bool Destroyed = false;

    public void DestroyMe()
    {
        Destroy(gameObject);
    }

    public bool getDestroyed()
    {      
        return Destroyed;
    }
    
    public void ChangeDestroyed()
    {
        Destroyed = !Destroyed;
    }
}
