using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class food : MonoBehaviour
{
    private bool up = true;
    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if (up)
        {
            pos.y += 0.05f * Time.deltaTime;
        }
        else
        {
            pos.y -= 0.05f * Time.deltaTime;
        }
        count++;
        if(count> 500)
        {
            up = !up;
            count= 0;
        }
        transform.position = pos;
        Vector3 rot=transform.rotation.eulerAngles;
        rot.y += 50 *Time.deltaTime;
        transform.rotation=Quaternion.Euler(rot);
    }
    public void Obtain()
    {
        Destroy(gameObject);
    }
}
