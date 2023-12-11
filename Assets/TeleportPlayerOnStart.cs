using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayerOnStart : MonoBehaviour{

    [SerializeField] private GameObject teleportTo;
    // Start is called before the first frame update
    void Start(){
        GameObject.FindWithTag("Player").transform.position = teleportTo.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
