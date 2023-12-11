using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FallingObjectController : MonoBehaviour{
    private bool active = true;
    
    private void OnTriggerEnter(Collider other){
        if (other.transform.CompareTag("EnemyHitbox") && active) {
            other.transform.GetComponent<EnemyHitbox>().KillEnemy();
            transform.GetComponent<NavMeshObstacle>().enabled = true;
        }
    }
    
    void Update(){
        //disable collision if not moving
        active = !(transform.GetComponent<Rigidbody>().velocity.magnitude < 0.01f);

    }
}
