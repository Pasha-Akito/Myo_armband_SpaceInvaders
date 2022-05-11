using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Laser : MonoBehaviour
{

    public float laserSpeed = 20.0f;
    public float laserLife = 0.5f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, laserLife);
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(0.0f, laserSpeed);
    }
}
