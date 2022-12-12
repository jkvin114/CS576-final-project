using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_player : MonoBehaviour
{
    public Transform player;
    bool shaking=false;
    float shakeElapsed = 0;
    void Update()
    {
        //   transform.position = player.transform.position + new Vector3(-0.7f, 2, 0);
        float z = 0;
        float y = 0;
        if (shakeElapsed < 0.3f && shaking)
        {
            z = Random.Range(-1f, 1f) * 0.1f;
            y = Random.Range(-1f, 1f) * 0.1f;
            shakeElapsed += Time.deltaTime;
        }
        else if (shaking)
        {
            shaking = false;
        }
        
           

        transform.position = new Vector3(player.transform.position.x-0.7f,3+y,0.925f+z);
    }
    public void Shake()
    {
        shaking = true;
        shakeElapsed = 0;
    }

}
