using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHitbox : MonoBehaviour{

    [SerializeField] private EnemyController parentEnemy;
    public string hitboxSection;

    private Dictionary<string, float> dmgDict = new Dictionary<string, float>();

    // Start is called before the first frame update
    void Start(){
        dmgDict = new Dictionary<string, float> {
            { "head", 9.1f},
            {  "upper chest", 8f},
            {  "middle chest", 7f},
            {  "lower chest", 6f},
            {  "hip", 5f},
            {  "upper leg", 4f},
            {  "lower leg", 3f},
            {  "upper arm", 2f},
            {  "lower arm", 1f}
        };
    }

    public void OnHit(float dmg){
        parentEnemy.DealDamage(dmg * dmgDict[hitboxSection]);
    }
    
    // Update is called once per frame
    void Update(){
        
    }
}
