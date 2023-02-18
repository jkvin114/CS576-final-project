using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitPrey : Prey
{
    // Start is called before the first frame update
    void Start()
    {

        animator = GetComponent<Animator>();
        gameObject.tag = "prey";
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        animator.SetTrigger("Idle");

        player = GameObject.Find("Fox");
        reactionDistance = Random.Range(4, 7);
        if(Random.Range(0,10)== 0 ) reactionDistance=0.6f;
    }

    protected override void setAnimatorSpeed(float speed)
    {
        if (animator != null)
            animator.speed= speed * 3;
    }
    protected override void onStart()
    {

    }
    public override void caught()
    {
        
        
        retire();
    }
    protected override void retire()
    {
        SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Rabbit);
        dead = true;
        isMoving= false;

        if (animator != null)
            animator.SetTrigger("Dead");
    }
    public override void Move()
    {
        //SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Rabbit);

        animator.SetTrigger("Run");
        base.Move();

    }

}
