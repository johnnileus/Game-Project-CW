using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectController : MonoBehaviour
{
    private void OnCollisionEnter(Collision other){
        if (other.transform.tag == "EnemyHitbox") {
            print("a");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
