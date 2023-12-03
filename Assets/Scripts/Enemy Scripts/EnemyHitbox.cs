using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHitbox : MonoBehaviour{

    [SerializeField] private EnemyController parentEnemy;
    public string hitboxSection;

    
    // Start is called before the first frame update
    void Start(){
        
    }

    public void OnHit(){
        parentEnemy.DealDamage(1);
    }
    
    // Update is called once per frame
    void Update(){
        
    }
}
