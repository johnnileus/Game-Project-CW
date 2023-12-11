using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoomManager : MonoBehaviour{

    public int currentRoom = -1;

    // gets room id of the room the player entered
    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("RoomTrigger")) {
            currentRoom = other.transform.parent.GetComponent<DungeonRoom>().roomID;
        }
    }
}
