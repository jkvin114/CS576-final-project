using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_player : MonoBehaviour
{
    bool shaking=false;
    float shakeElapsed = 0;
    public bool mobile=false;
    void Update()
    {
        //   transform.position = player.transform.position + new Vector3(-0.7f, 2, 0);
    }
    public void Follow(Transform tf)
    {
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
        if(mobile)
        {
            transform.position = new Vector3(tf.position.x + 1.1f, 4.5f + y, 0.925f + z);
        }
        else
        {
            transform.position = new Vector3(tf.position.x - 0.8f, 3f + y, 0.925f + z);
        }
         
        //

    }
    public void Shake()
    {
        shaking = true;
        shakeElapsed = 0;
    }

}
