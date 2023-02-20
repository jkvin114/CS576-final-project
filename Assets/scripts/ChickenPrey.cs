using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class ChickenPrey : Prey
{
    
    // Start is called before the first frame update
    void Start()
    {

        animator = GetComponent<Animator>();
        gameObject.tag = "prey";
        animator.SetBool("Eat", true);
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        player = GameObject.Find("Fox");
        reactionDistance = Random.Range(2, 4);
        Debug.Log(player);
        if (Random.Range(0, 10) == 0) reactionDistance = 0.6f;
    }
    public override void caught()
    {
        
        retire();
    }
    protected override void retire()
    {
        SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Chicken);
        dead =true;
        isMoving = false;
        
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 90);
        if (animator != null) animator.SetBool("Run", false);

        transform.position = new Vector3(transform.position.x,Level.bottomY+0.02f,transform.position.z);
    }
    protected override void onStart()
    {
        
    }
    public override void Move()
    {
      //  SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Chicken);

        animator.SetBool("Eat", false);
        animator.SetBool("Run", true);
        base.Move();

    }
    
 
    
}
