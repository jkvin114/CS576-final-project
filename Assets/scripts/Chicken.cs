using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Chicken : MonoBehaviour
{
    public Vector3 movement_direction;
    public float running_velocity;
    public float side_velocity;
    
    private Animator animation_controller;
    private CharacterController character_controller;
    private float velocity;
    private int lane;
    private float directionChangeBlockingTime = 0;
    public float directionChangeSpeed;
    float directionChangeDelay = 2;
    Direction currentDirection = Direction.FORWARD;
    void Start()
    {
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);
        running_velocity = 1.4f;
        side_velocity = 0.5f;
        velocity = 0.1f;
        lane = 2;

    }
    public float increaseSpeed()
    {
        if(running_velocity<=4f)
            running_velocity += 0.2f;
        return running_velocity;    
    }
    void Update()
    {
        bool going_left = Input.GetKeyDown(KeyCode.LeftArrow);
        bool going_right = Input.GetKeyDown(KeyCode.RightArrow);
        float laneOffset = 0;
        if (directionChangeBlockingTime <0)
        {
            currentDirection = Direction.FORWARD;
        }

        if (going_left && !going_right && directionChangeBlockingTime < 0) {
            Debug.Log("left");
            //  velocity = -1.0f;
            if (lane > 0) { 
                lane--;
                directionChangeBlockingTime = directionChangeDelay;
                currentDirection = Direction.LEFT;
            }
        } else if(!going_left && going_right && directionChangeBlockingTime < 0) {
            // velocity = 1.0f;
            if (lane < 4) { 
                lane++;
                directionChangeBlockingTime = directionChangeDelay;
                currentDirection = Direction.RIGHT;
            }
        } else {
            velocity = 0.0f;
        }
        directionChangeBlockingTime -= Time.deltaTime * running_velocity * directionChangeSpeed;

        if (directionChangeBlockingTime >= 0)
        {
            if (currentDirection == Direction.LEFT)
            {
                laneOffset = -Level.laneWidth * directionChangeBlockingTime / directionChangeDelay;
            }
            else if (currentDirection == Direction.RIGHT)
            {
                laneOffset = Level.laneWidth * directionChangeBlockingTime / directionChangeDelay;
            }
        }

        transform.position=new Vector3(transform.position.x+running_velocity*Time.deltaTime,
            transform.position.y, Level.laneCoordinates[lane]+ laneOffset);
        

        //movement_direction = new Vector3(side_velocity * velocity, 0.0f, running_velocity);

        //character_controller.Move(movement_direction * Time.deltaTime);
    }
}
