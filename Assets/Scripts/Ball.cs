using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The script holding all methods and logic of the player ball.
/// </summary>
public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    private TrajectoryLine lineTrajectory;

    [SerializeField]
    private float launchSpeed;
    [SerializeField]
    private Vector2 collisionDamping;

    private Vector2 direction;
    private Vector2 speed;
    private Vector2 lastVelocity;

    private bool isLaunched;

    private Vector3 pos;
    private Vector3 distance;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineTrajectory = GetComponent<TrajectoryLine>();
    }

    // Update is called once per frame
    void Update()
    {
        LaunchBall();
    }
    private void FixedUpdate()
    {
        // Use the distance between the ball and cursor to determine how to update the line trajectory
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        distance = pos - transform.position;
        lastVelocity = rb.velocity;
        if (!isLaunched)
        {
            lineTrajectory.UpdateLine(distance.normalized * launchSpeed);
        }
    }
    private void LaunchBall()
    {
        // Get the distance between the cursor and the ball
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        distance = pos - transform.position;

        // if left key is presed, launch the ball
        if (Input.GetButtonDown("Fire1"))
        {
            if (isLaunched) { return; }

            rb.velocity = distance.normalized * launchSpeed;
            isLaunched = true;
            GameManager.instance.hasGameStarted = true;
            lineTrajectory.HideTrajectory();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // if the collided object is the player pad...
        if (other.collider.CompareTag("Player"))
        {
            // add the pad's current x velocity to a dampened version of the player ball's x velocity.
            Vector2 velocity;
            velocity.x = other.collider.attachedRigidbody.velocity.x + (lastVelocity.x * collisionDamping.x);
            velocity.y = -lastVelocity.y;

          //  direction = Vector2.Reflect(velocity.normalized, other.GetContact(0).normal);
         //   velocity = velocity.magnitude * direction;

            rb.velocity = velocity;
        }
        // else, if it's something like the block or wall...
        else
        {
            // retain the ball's velocity, but change it's direction according to the reflective direction of the object hit.
            direction = Vector2.Reflect(lastVelocity.normalized, other.GetContact(0).normal);
            speed = lastVelocity.magnitude * direction ;

            rb.velocity = speed;
        }
    }
}
