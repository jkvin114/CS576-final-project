using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gem : MonoBehaviour
{
    private bool up = true;
    private int count = 0;
    // Start is called before the first frame update
    private float obtainTime = 0.3f;
    private bool isObtained=false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isObtained)
        {
            Vector3 p = transform.position;
            if (obtainTime > 0.1) {
                p.y += 4f * Time.deltaTime;
            }
            else
            {
                p.y -= 3f * Time.deltaTime;
            }
            
            transform.position = p;
            obtainTime -=Time.deltaTime;
            if(obtainTime < 0) Destroy(gameObject);
            return;
        } 

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
        transform.localScale = new Vector3(0.05f,0.05f,0.05f);
        isObtained=true;
    }
}
