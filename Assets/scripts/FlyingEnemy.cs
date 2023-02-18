using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    bool isMoving = false;
    bool isDescending = false;
    bool shouldLeave=false;
    Vector3 goalPos;
    float timeSpan = 0;
    float timeElapsed = 0;
    Vector3 direction;
    Vector3 startPos;
    Vector3 leavingDirection;
    float descendTime;
    void Start()
    {
        descendTime = Random.Range(0.6f, 0.85f);
        leavingDirection=new Vector3(2,6, Random.Range(-3, 3));
        //leavingDirection.Normalize();
        gameObject.tag = "obstacle";
    }
    public void StartMoveTo(float timeSpan, Vector3 pos)
    {
        goalPos = pos;
        this.timeSpan = timeSpan;
        timeElapsed = 0;
        isMoving = true;
        direction= pos - transform.position;
        direction.Normalize();
        startPos=transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        if (timeElapsed > timeSpan* descendTime && isMoving && !isDescending)
        {
           // Debug.Log(timeElapsed);
            isDescending = true;
            GetComponent<Animator>().SetBool("isAttacking",true);
            GetComponent<Animator>().SetTrigger("Attack");
        }
        if (timeElapsed > timeSpan && isMoving)
        {
            GetComponent<Animator>().SetBool("isAttacking", false);
            
            isMoving = false;
            shouldLeave = true;
            isDescending=false;
        }
        if(shouldLeave && timeElapsed > timeSpan * 2)
        {
            transform.position+= leavingDirection *Time.deltaTime;
        }
        else if (shouldLeave)
        {
            Vector3 pos = transform.position;
            pos.y=Mathf.Max(0.8f, pos.y- Time.deltaTime * 0.6f);
            transform.position=pos;
        }
        timeElapsed += Time.deltaTime;
        if (isMoving)
        {
            if(isDescending)
            {
                
                Vector3 pos = (timeElapsed / timeSpan) * goalPos + (1 - timeElapsed / timeSpan) * startPos;

               // pos.y -= (5.5f * Time.deltaTime);

                 float y = (1-(timeElapsed - (timeSpan * 0.7f)) / (timeSpan * 0.3f)) * startPos.y 
                    + (((timeElapsed - (timeSpan * 0.7f)) / (timeSpan * 0.3f))) * 1.3f;

                transform.position = new Vector3(pos.x, y,pos.z);
            }
            else
            {
                
                Vector3 pos = (timeElapsed /timeSpan) * goalPos + (1- timeElapsed / timeSpan) * startPos;
                transform.position=new Vector3(pos.x,1.8f,pos.z); 
            }
            //transform.position = pos;
        }
    }
}
