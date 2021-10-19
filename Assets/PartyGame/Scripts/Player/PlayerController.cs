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
    
    // Start is called before the first frame update
    void Start()
    {
       // moveLeft.onClick.AddListener(MoveCharacterLeft);
        //moveRight.onClick.AddListener(MoveCharacterRight);
        
    }

    void Update()
    {
       if(isMovingLeft || Input.GetKey(KeyCode.A))
           gameObject.transform.position += (Vector3.left * moveSpeed )* Time.deltaTime;
       if(isMovingRight || Input.GetKey(KeyCode.D))
           gameObject.transform.position += (Vector3.right  * moveSpeed) * Time.deltaTime;
    }

    public void OnClickMoveLeft() => MoveCharacterLeft();
    
    
    public void OnClickMoveRight() => MoveCharacterRight();


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

    
}
