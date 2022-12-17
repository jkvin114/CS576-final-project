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
    bool alive = true;
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
        if(running_velocity<=3.5f)
            running_velocity += 0.15f;
        else
            running_velocity += 0.07f;

        animation_controller.speed = running_velocity;

        UIController.GetComponent<Timer>().startTime = 300.0f / running_velocity;
        return running_velocity;    
    }
    IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 orignalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!alive) break;
            float z = transform.position.z+Random.Range(-1f, 1f) * magnitude;
            float y = transform.position.y+ Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(transform.position.x, y,z);
            elapsed += Time.deltaTime;
            yield return 0;
        }
        orignalPosition.x = transform.position.x;
        transform.position = orignalPosition;
    }
    public void death()
    {
        if (!alive) return;
        SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Gameover);

        alive = false;
        animation_controller.SetTrigger("Death");
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 90);
        running_velocity = 0;
    }

    public void setInvulnerable(int time) {
        isInvulnerable = true;
        invulTime = time;
        transform.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().material = transparentMat;

    }
    void collideObject(Collider other)
    {
        if (other.gameObject.CompareTag("obstacle") && !isInvulnerable)
        {
            other.gameObject.tag = "Untagged";

            //   Debug.Log("obstacle");
            //return;
            setInvulnerable(3);
            StartCoroutine(Shake(0.15f, 0.1f));
            UIController.GetComponent<Timer>().HitsObstacle();
            MainCamera.GetComponent<Follow_player>().Shake();
            SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Hit);
            SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.FoxHit);

        }
        if (other.gameObject.CompareTag("gem"))
        {
            other.gameObject.tag = "Untagged";
            other.gameObject.GetComponent<gem>().Obtain();
            UIController.GetComponent<Timer>().HitsNormalGem();
            Vector3 pos = transform.position;
            pos.x = transform.position.x + 0.4f;
            GameObject particle = Instantiate(gem_particle, pos, Quaternion.identity);

            particle.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
            particle.transform.parent = other.transform;
            particle.GetComponent<ParticleSystem>().Play();
            SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Gem);

        }
        if (other.gameObject.CompareTag("gem_special"))
        {
            other.gameObject.tag = "Untagged";

            Vector3 pos = transform.position;
            pos.x = transform.position.x + 0.2f;
            GameObject particle = Instantiate(gem_particle, pos, Quaternion.identity);

            other.gameObject.GetComponent<gem>().Obtain();
            UIController.GetComponent<Timer>().HitsSpecialGem();
            particle.transform.localScale = new Vector3(0.006f, 0.006f, 0.006f);
            particle.transform.parent = other.transform;
            particle.GetComponent<ParticleSystem>().Play();
            SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Gem2);

        }
        if (other.gameObject.CompareTag("key_food"))
        {
            other.gameObject.tag = "Untagged";

            other.gameObject.GetComponent<food>().Obtain();
            UIController.GetComponent<Timer>().HitsCorrectFood();
            Vector3 pos = transform.position;
            pos.x = transform.position.x + 0.2f;
            GameObject particle = Instantiate(food_particle, pos, Quaternion.identity);

            particle.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            particle.GetComponent<ParticleSystem>().Play();
            SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.RightFood);

        }
        if (other.gameObject.CompareTag("wrong_food"))
        {
            other.gameObject.tag = "Untagged";

            other.gameObject.GetComponent<food>().Obtain();
            UIController.GetComponent<Timer>().HitsWrongFood();
            SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.WrongFood);

        }
        if (other.gameObject.CompareTag("prey"))
        {
            other.gameObject.tag = "Untagged";

            animation_controller.SetTrigger("Eat");
            other.gameObject.GetComponent<Prey>().caught();
            UIController.GetComponent<Timer>().HitsSpecialGem();
            Vector3 pos = transform.position;
            pos.x = transform.position.x + 0.2f;
            GameObject particle = Instantiate(white_particle, pos, Quaternion.identity);
            SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Eat);

            particle.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            particle.transform.parent = other.transform;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        collideObject(other);
    }
    private void OnTriggerStay(Collider other)
    {
        collideObject(other);
    }
    void Update()
    {
        if (!alive) return;
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
