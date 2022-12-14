using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateFood : MonoBehaviour
{
    public Transform Apple;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Instantiating Apple!!!");
        Instantiate(Apple, new Vector3(2.0F, 0, 0), Quaternion.identity);
    }
}
