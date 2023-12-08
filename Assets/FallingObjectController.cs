using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FallingObjectController : MonoBehaviour{
    private bool active = true;
    
    private void OnTriggerEnter(Collider other){
        if (other.transform.tag == "EnemyHitbox" && active) {
            other.transform.GetComponent<EnemyHitbox>().KillEnemy();
            transform.GetComponent<NavMeshObstacle>().enabled = true;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update(){
        //disable collision if not moving
        active = !(transform.GetComponent<Rigidbody>().velocity.magnitude < 0.01f);

    }
}
