using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Interactable : MonoBehaviour{
    public bool active;

    enum Options{
        Button,
        FishBarrel
    }

    [SerializeField] private Options option;
    
    [SerializeField] private GameObject[] objectsToAnimate;
    [SerializeField] private GameObject[] objectsToDelete;
    [SerializeField] private GameObject[] objectsToDrop;

    [SerializeField] private GameObject FishBarrelPlatform;
    private List<GameObject> disableOnFishBarrel = new List<GameObject>();

    private void Start(){
        disableOnFishBarrel.Add(GameObject.FindWithTag("Player"));
        disableOnFishBarrel.Add(GameObject.FindWithTag("MainCanvas"));
        disableOnFishBarrel.Add(GameObject.FindWithTag("Map"));

        transform.parent = InteractablesManager.instance.transform;
    }

    public void OnPress(){
        if (active) {
            if (option == Options.Button) {
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
            else { // if fish barrel
                foreach (var i in disableOnFishBarrel) {
                    i.SetActive(false);
                }

                GameObject platform = Instantiate(FishBarrelPlatform);
                platform.GetComponent<BarrelController>().enableOnExit = disableOnFishBarrel;
                active = false;
            }
        }
    }
}

