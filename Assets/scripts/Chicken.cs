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

    void Start()
    {
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);
        running_velocity = 3.5f;
        side_velocity = 3.0f;
        velocity = 0.0f;
    }

    void Update()
    {
        bool going_left = Input.GetKey("a");
        bool going_right = Input.GetKey("d");

        if(going_left && !going_right) {
            velocity = -1.0f;
        } else if(!going_left && going_right) {
            velocity = 1.0f;
        } else {
            velocity = 0.0f;
        }

        movement_direction = new Vector3(side_velocity * velocity, 0.0f, running_velocity);

        character_controller.Move(movement_direction * Time.deltaTime);
    }
}
