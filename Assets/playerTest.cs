using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerTest : MonoBehaviour
{
    public GameObject level;
    // Start is called before the first frame update
    void Start()
    {
        
      
        StartCoroutine(generate());

    }
    public IEnumerator generate()
    {
        yield return new WaitForSeconds(1);

        Level sn = level.GetComponent<Level>();
        int count = 3;
        for (int i = 0; i < count; i++)
        {
          //  sn.createChunk();
            yield return new WaitForSeconds(0.2f);

        }
        yield return null;
        for (int i = 0; i < 5; i++)
        {
            //  deleteEarliestChunk();
            yield return new WaitForSeconds(0.2f);

        }
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
