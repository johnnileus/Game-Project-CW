using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoomManager : MonoBehaviour{

    public int currentRoom = -1;


    private void Start(){
        
    }

    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("RoomTrigger")) {
            currentRoom = other.transform.parent.GetComponent<DungeonRoom>().roomID;
        }
    }
}
