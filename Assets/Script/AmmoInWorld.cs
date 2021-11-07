using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoInWorld : MonoBehaviour
{
    public int Ammo = 0;
    // Start is called before the first frame update
    void Start()
    {
        Ammo = (int)Random.Range(2f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
