using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    internal bool isMoving=false;
    Direction direction = Direction.LEFT;
    Vector3 goalPos;
    float speed = 0;
    string[] animStates=new string[6] {"Sleep","Eat","Sit","Attack1", "Attack2", "Attack3" };
    void Start()
    {
        gameObject.tag = "obstacle";

    }
    public void StartMoveTo(float speed,Vector3 pos,int d)
    {
        goalPos = pos;
        this.speed= speed;
        direction = (Direction)d;
        isMoving= true;
        GetComponent<Animator>().SetBool(Random.Range(0,4)==0?"Jump": "Run Forward", true);
        GetComponent<Animator>().speed= speed/2;
    }
    // Update is called once per frame
    void Update()
    {
        if ((transform.position.z < goalPos.z && direction == Direction.RIGHT 
            || transform.position.z > goalPos.z && direction == Direction.LEFT )
            && isMoving)
        {
            isMoving=false;
            GetComponent<Animator>().SetBool("Run Forward", false);
            GetComponent<Animator>().SetBool("Jump", false);
            GetComponent<Animator>().SetBool(animStates[Random.Range(0, animStates.Length)], true);

        }

        if(isMoving)
        {
            Vector3 pos= transform.position;
            if (direction == Direction.RIGHT)
            {
                pos.z -= speed * Time.deltaTime;
            }
            else if (direction == Direction.LEFT)
            {
                pos.z += speed * Time.deltaTime;
            }
            else if (direction == Direction.FORWARD)
            {
                pos.x += speed * Time.deltaTime;
            }
            transform.position = pos;
        }
    }
}
