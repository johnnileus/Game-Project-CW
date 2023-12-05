using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Interactable : MonoBehaviour{
    public bool active;

    [SerializeField] private GameObject[] objectsToAnimate;
    [SerializeField] private GameObject[] objectsToDelete;
    [SerializeField] private GameObject[] objectsToDrop;

    private void Start(){
        
    }

    public void OnPress(){
        
        //animate object array
        foreach (var i in objectsToAnimate) {
            i.GetComponent<Animation>().Play();
        }
        
        //delete object array
        foreach (var i in objectsToDelete) {
            Destroy(i);
        }

        foreach (var i in objectsToDrop) {
            i.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        active = false;
    }
}
