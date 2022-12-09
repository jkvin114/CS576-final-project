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
    void Start()
    {
        
    }
    public void StartMoveTo(float speed,Vector3 pos,int d)
    {
        goalPos = pos;
        this.speed= speed;
        direction = (Direction)d;
        isMoving= true;
    }
    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < goalPos.z && direction == Direction.RIGHT)
        {
            isMoving=false;
        }
        if (transform.position.z > goalPos.z && direction == Direction.LEFT)
        {
            isMoving = false;
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
