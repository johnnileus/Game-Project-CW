using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayerOnStart : MonoBehaviour{

    [SerializeField] private GameObject teleportTo;

    void Start(){
        GameObject.FindWithTag("Player").transform.position = teleportTo.transform.position;
    }
    
}
