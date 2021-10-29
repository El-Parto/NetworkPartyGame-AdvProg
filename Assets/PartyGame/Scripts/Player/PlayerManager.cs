using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [SerializeField] private Button moveLeft;
    [SerializeField] private Button moveRight;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    // How far the player is allowed to move
    public float movementBounds = 8;
    // How far the player has moved from the start point
    [SerializeField] private float movementDistance;
    // The player's score (I think it has to be public because other scripts are accessing it)
    public int playerScore;
    // The player's health
    public int playerHealth;
    
    // Start is called before the first frame update
    void Start()
    {
        //moveLeft.onClick.AddListener(MoveCharacterLeft);
        //moveRight.onClick.AddListener(MoveCharacterRight);
        
    }

    void Update()
    {
        if(isMovingLeft || Input.GetKey(KeyCode.A))
            if (movementDistance > movementBounds * -1)
            {
                gameObject.transform.position += (Vector3.left * moveSpeed )* Time.deltaTime;
                movementDistance -= moveSpeed * Time.deltaTime;
            }
            
            
        if(isMovingRight || Input.GetKey(KeyCode.D))
            if (movementDistance < movementBounds)
            {
                gameObject.transform.position += (Vector3.right * moveSpeed) * Time.deltaTime;
                movementDistance += moveSpeed * Time.deltaTime;
            }
            
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