using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablesManager : MonoBehaviour{

    [SerializeField] private float interactDist;

    //singleton
    public static InteractablesManager instance;
    
    //returns the closest active interactable item
    public (Transform, float) FindClosestItem(Vector3 plrPos){
        if (transform.childCount > 0) {
            float closestDist = Single.PositiveInfinity;
            Transform closestItem = null;
            for (int i = 0; i < transform.childCount; i++) {
                
                Transform item = transform.GetChild(i);
                if (item.GetComponent<Interactable>().active) {
                    float dist = (plrPos - item.transform.position).magnitude;
                    if (dist < closestDist) {
                        closestDist = dist;
                        closestItem = item;
                    }
                }
            }
            return (closestItem, closestDist);
        }
        else {
            return (null, Single.PositiveInfinity);
        }
    }
    
    private void Awake(){
        if (instance != null && instance != this) { Destroy(this); } 
        else { instance = this; } 
    }
    
    
    private void Update(){
        
        if (Input.GetKeyDown(KeyCode.E)) { //activate closest interactable if in range
            (Transform closest, float dist) = FindClosestItem(GameObject.FindWithTag("Player").transform.position);
            if (dist < interactDist) {
                closest.GetComponent<Interactable>().OnPress();
            }
        }
    }
}
