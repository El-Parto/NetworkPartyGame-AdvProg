using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [SerializeField] private Button moveLeft;
    [SerializeField] private Button moveRight;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;
    public bool canKick; // this turns true when invis' hitbox has a ball 
    [SerializeField] private GameObject kickVisPrefab; // the kick mechanic's visualiser.
    
    
    // Start is called before the first frame update
    void Start()
    {
       // moveLeft.onClick.AddListener(MoveCharacterLeft);
        //moveRight.onClick.AddListener(MoveCharacterRight);
        
    }

    void Update()
    {
        MovePlayer();
       
       if(Input.GetKeyDown(KeyCode.Space) && canKick)
       {
           VisualiseKick();
           // KickBall();
       }
       
       
    }

    public void OnClickMoveLeft() => MoveCharacterLeft();
    
    
    public void OnClickMoveRight() => MoveCharacterRight();



    /// <summary>
    /// This is to ensure touch countrols deactivate upon button Up in the GUI
    /// </summary>
    public void OnReleaseMove()
    {
        isMovingLeft = false;
        isMovingRight = false;
        gameObject.transform.position += Vector3.zero;  
    } 
    
   public void MoveCharacterLeft()
   {
       //find direction facing via Rotation
       isMovingLeft = true;
       
       
   }
   
   
   public void MoveCharacterRight()
   {
       isMovingRight = true;
       //find direction facing via Rotation
       
   }


   /// <summary>
   /// Allows the player to move left or right based on bools and input
   /// Where isMovingLeft/Right is used for the UI controls and the Input is used for Keyboard controls.
   /// </summary>
   public void MovePlayer()
   {
       if(isMovingLeft || Input.GetKey(KeyCode.A))
           gameObject.transform.position += (Vector3.left * moveSpeed )* Time.deltaTime;
       if(isMovingRight || Input.GetKey(KeyCode.D))
           gameObject.transform.position += (Vector3.right  * moveSpeed) * Time.deltaTime;

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

    
}
