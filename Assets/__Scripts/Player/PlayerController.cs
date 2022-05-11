using UnityEngine;
using System.Collections;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Myo game object to connect with.
    // This object must have a ThalmicMyo script attached.
    public GameObject myo = null;

    // The pose from the last update. This is used to determine if the pose has changed
    // so that actions are only performed upon making them rather than every frame during
    // which they are active.
    private Pose lastPose = Pose.Unknown;


    public float horizontalSpeed = 2.0f;
    public Laser laserPrefab;
    public float firingRate = 0.5f;
    public System.Action killed;// if player is dead
    public AudioClip laserSound;
    public AudioClip deathSound;

    private GameObject laserParent;
    private Coroutine firingCoroutine;
    private bool runningCoroutine = false;
    private bool alive = true;

    private SpriteRenderer render;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private ThalmicMyo thalmicMyo;

    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        thalmicMyo = myo.GetComponent<ThalmicMyo>();
        audioSource = GetComponent<AudioSource>();

        laserParent = GameObject.Find("LaserParent");
        if (!laserParent)
        {
            laserParent = new GameObject("LaserParent");
        }
    }

    void Update()
    {

        // Check if the pose has changed since last update.
        // The ThalmicMyo component of a Myo game object has a pose property that is set to the
        // currently detected pose (e.g. Pose.Fist for the user making a fist). If no pose is currently
        // detected, pose will be set to Pose.Rest. If pose detection is unavailable, e.g. because Myo
        // is not on a user's arm, pose will be set to Pose.Unknown.
        if (thalmicMyo.pose != lastPose)
        {
            lastPose = thalmicMyo.pose;

            // Fire laser when fist is made
            // Vibrates the armband
            if (thalmicMyo.pose == Pose.Fist && alive)
            {
                thalmicMyo.Vibrate(VibrationType.Medium);

                //Firing Code here
                firingCoroutine = StartCoroutine(FireCoroutine());

                ExtendUnlockAndNotifyUserAction(thalmicMyo);

            }
            else if (runningCoroutine)
            {
                StopCoroutine(firingCoroutine);
            }


            // hand position idle 
            if (thalmicMyo.pose == Pose.Rest)
            {
                render.color = Color.cyan;
                Vector2 playerVelocity = new Vector2(0, 0);
                rb.velocity = playerVelocity;

                ExtendUnlockAndNotifyUserAction(thalmicMyo);
            }

            // right movement
            // red cause R
            else if (thalmicMyo.pose == Pose.WaveIn)
            {
                render.color = Color.red;
                Vector2 playerVelocity = new Vector2(horizontalSpeed, rb.velocity.y);
                rb.velocity = playerVelocity;


                ExtendUnlockAndNotifyUserAction(thalmicMyo);
            }

            // left movement
            // yellow cause L
            else if (thalmicMyo.pose == Pose.WaveOut)
            {
                render.color = Color.yellow;

                Vector2 playerVelocity = new Vector2(-horizontalSpeed, rb.velocity.y);
                rb.velocity = playerVelocity;

                ExtendUnlockAndNotifyUserAction(thalmicMyo);
            }

            else if (thalmicMyo.pose == Pose.DoubleTap)
            {

                if (Time.timeScale == 1)
                {
                    Time.timeScale = 0;
                }
                else
                {
                    Time.timeScale = 1;
                }
                

                ExtendUnlockAndNotifyUserAction(thalmicMyo);
            }


        }
    }

    private IEnumerator FireCoroutine()
    {
        while (true)
        {
            runningCoroutine = true;
            FireLaser();
            yield return new WaitForSeconds(firingRate);
            runningCoroutine = false;
        }
    }

    private void FireLaser()
    {
        Laser l = Instantiate(laserPrefab, laserParent.transform);
        l.transform.position = gameObject.transform.position;

        //playing sound
        if (audioSource)
            audioSource.PlayOneShot(laserSound);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Bomb>())
        {
            SoundController sc = FindObjectOfType<SoundController>();
            if (sc && alive)
            {
                sc.PlayOneShot(deathSound);
            }

            //destroy bomb
            Destroy(collision.gameObject);
            //destroy the player
            alive = false;
            render.enabled = false;

            StartCoroutine(Dead());
        }
    }

    private IEnumerator Dead()
    {
        yield return new WaitForSecondsRealtime(5);
        SceneManager.LoadScene(0);
    }

    // seems to just be the standard 
    void ExtendUnlockAndNotifyUserAction(ThalmicMyo myo)
    {
        ThalmicHub hub = ThalmicHub.instance;

        if (hub.lockingPolicy == LockingPolicy.Standard)
        {
            myo.Unlock(UnlockType.Timed);
        }
        //un comment when done
        myo.NotifyUserAction();
    }



}
