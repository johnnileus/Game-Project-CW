using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour{
    private GameObject cameraGO;


    private void Start(){
        cameraGO = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update(){
        Vector3 target = new Vector3(cameraGO.transform.position.x, transform.position.y, cameraGO.transform.position.z);
        transform.LookAt(transform.position - (target - transform.position));
    }
}