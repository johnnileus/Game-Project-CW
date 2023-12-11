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
            { "head", 3f},
            {  "upper chest", 1.2f},
            {  "middle chest", 1.3f},
            {  "lower chest", 1f},
            {  "hip", 1f},
            {  "upper leg", .8f},
            {  "lower leg", .6f},
            {  "upper arm", .8f},
            {  "lower arm", .7f}
        };
    }

    public void OnHit(float dmg){
        parentEnemy.DealDamage(dmg * dmgDict[hitboxSection] * Random.Range(0.9f, 1.1f));
    }

    public void KillEnemy(){
        parentEnemy.Kill();
    }
    
    // Update is called once per frame
    void Update(){
        
    }
}
