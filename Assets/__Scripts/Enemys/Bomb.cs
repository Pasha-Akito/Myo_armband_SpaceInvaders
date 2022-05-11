using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Vector3 orientation;
    [SerializeField] float speed = 10;
    public System.Action destroyed;

    // Update is called once per frame
    private void Update()
    {
        this.transform.position += this.speed * Time.deltaTime * this.orientation;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this.destroyed != null)
        {
            this.destroyed.Invoke();
        }
        
        Destroy(this.gameObject);
    }
}
