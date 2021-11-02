using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.AnimatedValues;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NetworkPartyGame.Physics
{
    public class Ball : MonoBehaviour
    {
        // Speed of ball
        public float speed = 10f;
        // The ball...
        public GameObject ball;
        // Determines if you can kick
        public bool canKick;
<<<<<<< HEAD
        // Start is called before the first frame update
        void Start()
        {
=======
        // The spot where the balls spawn
        public GameObject ballSpawnPosition;
        // The game manager
        public GameManager GameManager;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        private void Awake()
        {
            // Todo: Find a way to spawn the ball that doesn't suck
            ballSpawnPosition = GameObject.Find("Ball Spawn Position");
            // Doing this is probably BAD
            GameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
            // Makes sure it's at the ball transform position when spawned
            transform.position = ballSpawnPosition.transform.position;
>>>>>>> parent of 3285688 (Scoring and player health)
            // Picks a random starting direction
            ball.transform.rotation = Quaternion.Euler(0, Random.Range(0,360), 0);
        }

        // Update is called once per frame
        void Update()
<<<<<<< HEAD
=======
        {

        }

        private void OnCollisionEnter(Collision collision)
>>>>>>> parent of 3285688 (Scoring and player health)
        {
            // NOTE: THIS WOULD NOT WORK IN FIXED UPDATE
            //ball.transform.position += ball.transform.forward * speed * Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space) && canKick)
            {
                Debug.Log("should be working");
                // double the speed (might need to fix this later so things don't get TOO fast)
                speed *= 2;
                // Casts a ray in front of the ball towards the object it hits
                // If you can kick and you hit the space key
                if (UnityEngine.Physics.Raycast(ball.transform.position, ball.transform.forward, out RaycastHit hit))
                {
                    // Reflects the ball to go the other way
                    ball.transform.forward = Vector3.Reflect(ball.transform.forward, hit.normal);
                }
                // Sets cankick to false so you can't mash space to get infinite speed
                canKick = false;
            }
        }

        void FixedUpdate()
        {
            /*rigidbody.AddForce(ball.transform.forward * speed * Time.deltaTime, ForceMode.Impulse);
            rigidbody.velocity = Min(rigidbody.velocity, Vector3.one * speed);*/
            // Sets the angular velocity to 0 to avoid spins
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            // Moves the ball in the forward direction
            GetComponent<Rigidbody>().MovePosition(ball.transform.position + ball.transform.forward * speed * Time.deltaTime);

        }

        private void OnCollisionEnter(Collision collision)
        {
            // Casts a ray in front of the ball towards the object it hits
            if (UnityEngine.Physics.Raycast(ball.transform.position, ball.transform.forward, out RaycastHit hit))
            {
                // Reflects the ball to go the other way
                ball.transform.forward = Vector3.Reflect(ball.transform.forward, hit.normal);
            }

            if(collision.collider.CompareTag("Bumper") && speed <= 19.9f) // if ball speed is below 20
                speed *= 1.35f; // can multiply it's speed byy a fair amount
            if(collision.collider.CompareTag("Bumper") && speed >= 20) // if ball speed is above or equal  to 20, multiply by small margin instead
                speed *= 1.00f;


            // When the ball collides with a bumper (might change this later to cover all collisions)
            /*if (collision.gameObject.tag == "Bumper")
            {
                
                // ball.transform.rotation = Quaternion.Euler(0, (180 - ball.transform.rotation.y), 0);
            }*/
        }

        private void OnTriggerEnter(Collider collider)
        {
            // If the ball enters a kickzone
            if (collider.gameObject.tag == "Kickzone")
            {
                // set cankick to true
                canKick = true;
            }
<<<<<<< HEAD
=======
            // If the ball enters a scorezone
            if (collider.gameObject.tag == "Scorezone")
            {
                // Spawn a new ball and destroy this one
                // THIS IS ALSO WHERE SCORING WOULD GO
                // todo: Make the ball store which player last hit it, and which player was scored against
                GameManager.SpawnBall();
                Destroy(this.gameObject);
            }
>>>>>>> parent of 3285688 (Scoring and player health)
        }
        private void OnTriggerExit(Collider collider)
        {
            // if the ball exits the kickzone
            if (collider.gameObject.tag == "Kickzone")
            {
                // set cankick to false
                canKick = false;
            }
        }
    }
}