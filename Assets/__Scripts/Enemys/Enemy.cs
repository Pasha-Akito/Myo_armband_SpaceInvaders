using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Sprite[] animationSprites;//array of available sprites to cycle
    public float animationTime = 0.7f;//duration of each sprite
    private SpriteRenderer spriteRenderer;//which sprite is rendered
    private int currentAnimation;//keeps track of current sprite 
    public System.Action killed;// if Enemy is dead
    public AudioClip hitSound;


    private void Awake()
    {
        //looks for component that enemy script is attached to
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        InvokeRepeating(nameof(Animation), this.animationTime, this.animationTime);//calls method ever couple of set interval
    }
    private void Animation()
    {
        currentAnimation++;//next fram
        if (currentAnimation >= this.animationSprites.Length)
        {
            currentAnimation = 0;
        }

        spriteRenderer.sprite = this.animationSprites[currentAnimation];//update sprite
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Laser>())
        {

            SoundController sc = FindObjectOfType<SoundController>();
            if (sc)
            {
                sc.PlayOneShot(hitSound);
            }

            this.killed.Invoke();
            this.gameObject.SetActive(false);//not active on killed
            //destroy laser
            Destroy(collision.gameObject);
            //destroy the enemy
            Destroy(gameObject);

        }
    }


}
