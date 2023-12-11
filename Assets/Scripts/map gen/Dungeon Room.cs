using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = System.Numerics.Vector2;

public class DungeonRoom : MonoBehaviour{

    [HideInInspector] public int roomID;
    [HideInInspector] public int startDirection;
    [HideInInspector] public float roomRotation;

    private bool initialised = false;
    private bool enemiesSpawned = false;
    [SerializeField] private int enemyTotal;
    private int enemyCount;
    private GameObject coverPoints;

    [SerializeField] private GameObject enemyPrefab;
    
    public GameObject northDoor;
    public GameObject eastDoor;
    public GameObject southDoor;
    public GameObject westDoor;

    private Dictionary<int, GameObject> int2Door = new Dictionary<int, GameObject>();

    

    // Start is called before the first frame update
    void Start(){
        enemyCount = enemyTotal;
        int2Door = new Dictionary<int, GameObject> {
            { 0, northDoor },
            { 1, eastDoor },
            { 2, southDoor },
            { 3, westDoor },
        };
        coverPoints = transform.GetChild(0).gameObject;
        
        SpawnEnemies();
        
        
        int newDir = (startDirection - Mathf.RoundToInt(roomRotation / 90) + 4) % 4;
        if (roomID != -1) {int2Door[newDir].SetActive(false);}
    }

    public void SpawnEnemies(){
        if (!initialised) { 
            for (int i = 0; i < enemyTotal; i++) {
                Vector3 pos = new Vector3(Random.Range(-20f, 20f), 3f,Random.Range(-20f, 20f)) + transform.position;
                GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity, transform);
                EnemyController enemyScript = enemy.GetComponent<EnemyController>();
                enemyScript.parentRoom = this;
                enemyScript.coverPoints = coverPoints;
            }
        }
       
    }
    
    public void ReduceEnemyCount(){
        enemyCount--;
        if (enemyCount == 0) {
            if (northDoor) {northDoor.SetActive(false);}
            if (eastDoor) {eastDoor.SetActive(false);}
            if (southDoor) {southDoor.SetActive(false);}
            if (westDoor) {westDoor.SetActive(false);}
        }
    }
    
}
