using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public Rigidbody Body = null;
    public int Damage = 1;

    private float time = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time > 5f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamages(Damage);
        }
        else
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null)
            {
                character.TakeDamages(Damage);
            }
        }

        Destroy(gameObject);
    }
}
