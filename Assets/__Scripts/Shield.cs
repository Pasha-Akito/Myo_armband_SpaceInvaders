using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Laser>() || collision.GetComponent<Bomb>())
        {
            //destroys bomb or laser
            Destroy(collision.gameObject);
            //destroys shield part
            Destroy(gameObject);

        }
    }
}
