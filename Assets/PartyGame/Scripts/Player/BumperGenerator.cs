using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NetworkPartyGame.Physics
{
    public class BumperGenerator : MonoBehaviour
    {
        //[SerializeField] private Ball ball;
        public bool canKick;

        [SerializeField] private GameObject kickVisPrefab; // the kick mechanic's visualiser.
        
        void Start()
        {
            //ball = FindObjectOfType<Ball>();
        }

        
        // Update is called once per frame
        private void Update()
        {

            



            if(Input.GetKeyDown(KeyCode.Space) && canKick)
            {
                VisualiseKick();
               // KickBall();
            }
            
        }



        public void VisualiseKick()
        {
            // the reason why we check twice is so that the UI button can only activate it once per click &&when in range of the ball.
            if(canKick)
            {
                Instantiate(kickVisPrefab, gameObject.transform); // instantiates the visualiser prefab
                canKick = false; // another setter for the can kick flag
            }

        }

        private void OnTriggerEnter(Collider collider)
        {
            // If the Kickzone finds a ball
            if (collider.gameObject.tag == "Ball")
            {
                Debug.Log("DING DING DING");
                // set cankick to true
                canKick = true;
            }
        }
        private void OnTriggerExit(Collider collider)
        {
            // if the ball exits the kickzone
            if (collider.gameObject.tag == "Ball")
            {
                Debug.Log("BING BING BING");
                // set cankick to false
                canKick = false;
            }
        }
/*
        private void KickBall()
        {
            
        
            // NOTE: THIS WOULD NOT WORK IN FIXED UPDATE
            //ball.transform.position += ball.transform.forward * speed * Time.deltaTime;

            Debug.Log("should be working");
            // double the speed (might need to fix this later so things don't get TOO fast)
            ball.speed *= 2;
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

    
*/  }  

}