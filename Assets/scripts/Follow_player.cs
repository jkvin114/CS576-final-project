using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_player : MonoBehaviour
{
    public Transform player;

    void Update()
    {
     //   transform.position = player.transform.position + new Vector3(-0.7f, 2, 0);
    
        transform.position = new Vector3(player.transform.position.x-0.7f,3,0.925f);
    }

}
