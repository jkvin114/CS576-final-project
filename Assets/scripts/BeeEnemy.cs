using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeEnemy : MonoBehaviour
{
    Vector3 targetPos;
    Fox player;
    float speed;
    bool soundPlayed=false;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "obstacle";
        transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    public void StartMove(Vector3 targetPos, Fox player)
    {
        this.player= player;
        this.targetPos=targetPos;
    }
    // Update is called once per frame
    void Update()
    {
        if(targetPos.x - player.transform.position.x <= -1)
        {
            Destroy(gameObject);
            return;
        }

        if(targetPos.x - player.transform.position.x > 0)
        {

            float time = Mathf.Abs(targetPos.x - player.transform.position.x) / player.running_velocity;
            Debug.Log(time);
            speed = Mathf.Abs(targetPos.x - transform.position.x) / time;
            if (!soundPlayed && time < 1)
            {
                SFXManager.sfx_instance.Audio.PlayOneShot(SFXManager.sfx_instance.Bee);
                soundPlayed = true;
            }
        }

        Vector3 pos = transform.position;
        pos.x-=(speed * Time.deltaTime);
        transform.position = pos;
    }
}
