using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour{

    [HideInInspector] public int roomID;
    [HideInInspector] public int startDirection;
    [HideInInspector] public float roomRotation;

    public GameObject[] spawners;

    public int enemyCount;

    public GameObject northDoor;
    public GameObject eastDoor;
    public GameObject southDoor;
    public GameObject westDoor;

    private Dictionary<int, GameObject> int2Door = new Dictionary<int, GameObject>();

    // Start is called before the first frame update
    void Start(){
        
        int2Door = new Dictionary<int, GameObject>() {
            { 0, northDoor },
            { 1, eastDoor },
            { 2, southDoor },
            { 3, westDoor },
        };
        
        
        int newDir = (startDirection - Mathf.RoundToInt(roomRotation / 90) + 4) % 4;
        if (roomID != -1) {int2Door[newDir].SetActive(false);}



    }

    // Update is called once per frame
    void Update()
    {
        // if (enemyCount == 0 && roomID != -1) {
        //    bool enemiesSpawned = true;
        //    foreach (var spawner in spawners) {
        //        if (!spawner.GetComponent<EnemySpawner>().enemiesSpawned) {
        //            enemiesSpawned = false;
        //        }
        //    }
        //
        //    if (enemiesSpawned) { // unlock room
        //        if (northDoor) {northDoor.SetActive(false);}
        //        if (eastDoor) {eastDoor.SetActive(false);}
        //        if (southDoor) {southDoor.SetActive(false);}
        //        if (westDoor) {westDoor.SetActive(false);}
        //    }
        // }
    }
}
