using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;
using static UnityEngine.ParticleSystem;
public class Fox : MonoBehaviour
{
    public Vector3 movement_direction;
    public float running_velocity;
    public float side_velocity;
    public Material normalMat;
    public Material transparentMat;
    private Animator animation_controller;
    public GameObject UIController;
    public GameObject MainCamera;
    private float velocity;
    private int lane;
    private float directionChangeBlockingTime = 0;
    float directionChangeSpeed=8;
    public float directionChangeDelay;
    Direction currentDirection = Direction.FORWARD;
    private bool isInvulnerable=false;
    private float invulTime = 0;
    public GameObject food_particle;
    public GameObject gem_particle;
    public GameObject white_particle;
    int inputBuffer = 0;
    void Start()
    {
        animation_controller = GetComponent<Animator>();
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);
        side_velocity = 0.5f;
        velocity = 0.1f;
        lane = 2;
        animation_controller.SetTrigger("Run");
        animation_controller.speed = running_velocity*1.3f;

    }
    public float increaseSpeed()
    {
        if(running_velocity<=4f)
            running_velocity += 0.2f;
        animation_controller.speed = running_velocity;
        if (running_velocity > 3.0f) {
        } 
        return running_velocity;    
    }
    IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 orignalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float z = transform.position.z+Random.Range(-1f, 1f) * magnitude;
            float y = transform.position.y+ Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(transform.position.x, y,z);
            elapsed += Time.deltaTime;
            yield return 0;
        }
        orignalPosition.x = transform.position.x;
        transform.position = orignalPosition;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("obstacle")  && !isInvulnerable)
        {
            //   Debug.Log("obstacle");
            //return;
            isInvulnerable = true;
            invulTime = 3;
            transform.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().material=transparentMat;
            StartCoroutine(Shake(0.15f, 0.1f));
            UIController.GetComponent<Timer>().HitsObstacle();
            MainCamera.GetComponent<Follow_player>().Shake();
        }
        if (other.gameObject.CompareTag("gem"))
        {
            other.gameObject.GetComponent<gem>().Obtain();
            UIController.GetComponent<Timer>().HitsNormalGem();
            Vector3 pos = transform.position;
            pos.x = transform.position.x + 0.4f;
            GameObject particle = Instantiate(gem_particle, pos, Quaternion.identity);

            particle.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            particle.GetComponent<ParticleSystem>().Play();
        }
        if (other.gameObject.CompareTag("gem_special"))
        {
            Vector3 pos = transform.position;
            pos.x = transform.position.x + 0.2f;
            GameObject particle = Instantiate(gem_particle, pos, Quaternion.identity);

            other.gameObject.GetComponent<gem>().Obtain();
            UIController.GetComponent<Timer>().HitsSpecialGem();
            particle.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            particle.GetComponent<ParticleSystem>().Play();
        }
        if (other.gameObject.CompareTag("key_food"))
        {

            other.gameObject.GetComponent<food>().Obtain();
            UIController.GetComponent<Timer>().HitsCorrectFood(); 
            Vector3 pos = transform.position;
            pos.x = transform.position.x + 0.2f;
            GameObject particle = Instantiate(food_particle, pos, Quaternion.identity);

            particle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            particle.GetComponent<ParticleSystem>().Play();
        }
        if (other.gameObject.CompareTag("wrong_food"))
        {
            other.gameObject.GetComponent<food>().Obtain();
            UIController.GetComponent<Timer>().HitsWrongFood();
        }
        if (other.gameObject.CompareTag("prey"))
        {
            animation_controller.SetTrigger("Eat");
            other.gameObject.GetComponent<Prey>().caught();
            UIController.GetComponent<Timer>().HitsSpecialGem();
            Vector3 pos = transform.position;
            pos.x = transform.position.x + 0.2f;
            GameObject particle = Instantiate(white_particle, pos, Quaternion.identity);

            particle.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }
    void Update()
    {
        MainCamera.GetComponent<Follow_player>().Follow(transform);
        invulTime -= Time.deltaTime;
        if (invulTime < 0 && isInvulnerable)
        {
          //  Debug.Log("end invul");

            transform.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().material = normalMat;
            isInvulnerable = false;
        }
        bool going_left = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
        bool going_right = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
        float laneOffset = 0;
        if (directionChangeBlockingTime <0)
        {
            currentDirection = Direction.FORWARD;
        }
        else
        {
            if (going_left) inputBuffer = 1;
            else if (going_right) inputBuffer = 2;
        }

        if (((going_left && !going_right)|| inputBuffer==1 )&& directionChangeBlockingTime < 0) {
            //  velocity = -1.0f;
            if (lane > 0) {
                lane--;
                directionChangeBlockingTime = directionChangeDelay;
                currentDirection = Direction.LEFT;
                animation_controller.SetTrigger("Left");
            }
            inputBuffer = 0;
        } else if(((!going_left && going_right) || inputBuffer==2) && directionChangeBlockingTime < 0) {
            // velocity = 1.0f;
            if (lane < 4) { 
                lane++;
                directionChangeBlockingTime = directionChangeDelay;
                currentDirection = Direction.RIGHT;
                animation_controller.SetTrigger("Right");
            }
            inputBuffer = 0;
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
        Level.bottomY + (Mathf.Sin(Time.time * 30*running_velocity)+1)/50.0f, Level.laneCoordinates[lane]+ laneOffset);
            

        //movement_direction = new Vector3(side_velocity * velocity, 0.0f, running_velocity);

        //character_controller.Move(movement_direction * Time.deltaTime);
    }
}
