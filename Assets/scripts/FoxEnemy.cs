using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxEnemy : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    // Start is called before the first frame update
    internal bool isMoving = false;
    Direction direction = Direction.LEFT;
    Vector3 goalPos;
    float speed = 0;
    string[] animStates = new string[3] { "Sit1", "Sit1", "Attack"};
    void Start()
    {
        gameObject.tag = "obstacle";
        animator= GetComponent<Animator>();

    }
    public void StartMoveTo(float speed, Vector3 pos, int d)
    {
        goalPos = pos;
        this.speed = speed;
        direction = (Direction)d;
        isMoving = true;
        animator.SetTrigger("Run");
        animator.speed = speed / 2;
    }
    // Update is called once per frame
    void Update()
    {
        if ((transform.position.z < goalPos.z && direction == Direction.RIGHT
            || transform.position.z > goalPos.z && direction == Direction.LEFT)
            && isMoving)
        {
            isMoving = false;
  //          animator.SetTrigger("Run");
//            animator.SetTrigger("Jump");
            animator.SetTrigger(animStates[Random.Range(0, animStates.Length)]);

        }

        if (isMoving)
        {
            Vector3 pos = transform.position;
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
