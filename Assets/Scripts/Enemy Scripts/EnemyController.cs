using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour{
    
    private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private float lookAtRotSpeed;
    
    private bool alerted = false;
    private float alertness = 0;
    [SerializeField] private float alertIncRate;
    [SerializeField] private float alertDecRate;
    
    private NavMeshAgent agent;
    private GameObject player; 
    [HideInInspector] public GameObject coverPoints;
    [SerializeField] private Animator modelAnimator;
    [SerializeField] private LayerMask layersToIgnore;
    [SerializeField] private float enemyHeadHeight;

    [SerializeField] private float patrolDelay;
    private float lastPatrol;

    [SerializeField] private AnimationCurve hitChance;
    [SerializeField] private GameObject gunFlash;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootDelay;
    [SerializeField] private float shootOffset;
    private float lastShot;

    [SerializeField] private GameObject yellowAlert;
    [SerializeField] private GameObject redAlert;
    
    
    private Vector3 prevPos;
    private enum ActionStates{
        Idle,
        Walking,
        Prone,
        Dead
    }

    public DungeonRoom parentRoom;
    private ActionStates curState = ActionStates.Idle;

    
    // Start is called before the first frame update
    void Start(){
        health = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        lastPatrol = Time.time;
        lastShot = Time.time;
    }

    public void DealDamage(float dmg){
        health -= dmg;
        if (health <= 0) {
            Kill();
        }
    }

    public void Kill(){
        if (curState != ActionStates.Dead) {
            modelAnimator.Play("pistol death");
            curState = ActionStates.Dead;
            agent.enabled = false;
            parentRoom.ReduceEnemyCount();
            PlayerScoreManager.instance.IncrementKillCount();
            
            health = 0;}
        
    }

    private void SetNewPatrol(){
        float angle = Random.Range(0, 2*Mathf.PI);
        Vector3 newPoint = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        agent.SetDestination(newPoint * 3 + transform.position);
    }

    private Transform GetClosestCover(Vector3 pos){
        float dist = 99999;
        Transform closestCover = null;

        foreach (Transform child in coverPoints.transform) {
            if (child.GetComponent<boolAttribute>().value == false) {
                float newDist = Vector3.Distance(pos, child.position);
                if (newDist < dist) {
                    closestCover = child;
                    dist = newDist;
                }
            }
        }

        if (closestCover) {
            return closestCover;
        }
        return null;
        
    }

    private void SetAlert(bool alert){
        alerted = alert;
        if (alert) {
            Transform closestCover = GetClosestCover(transform.position);
            agent.SetDestination(closestCover.position);
            closestCover.GetComponent<boolAttribute>().value = true;
        }
    }

    private void turnToPlayer(){
        var rot = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, lookAtRotSpeed * Time.deltaTime);
    }
    
    // Update is called once per frame
    void Update(){
        if (curState != ActionStates.Dead) {

            Vector3 curPos = transform.position + new Vector3(0, enemyHeadHeight, 0);
            Vector3 playerPos = player.transform.position;
            float dist = Vector3.Distance(curPos, playerPos);
            float speed = Vector3.Distance(curPos, prevPos) / Time.deltaTime;



            //check for line of sight
            bool LOS = !Physics.Linecast(curPos, playerPos, ~layersToIgnore);
            if (LOS) {
                alertness += alertIncRate * Time.deltaTime;
            }
            else {
                alertness -= alertDecRate * Time.deltaTime;
                if (alertness < 0) {
                    alertness = 0;
                }
            }
            
            //patrol
            if (!alerted) {
                if (lastPatrol + patrolDelay < Time.time) {
                    SetNewPatrol();
                    lastPatrol = Time.time;
                }
                if (alertness > 1) {
                    SetAlert(true);
                }
            }
            
            // set alert symbol
            if (alerted && curState != ActionStates.Dead) {
                redAlert.SetActive(true);
                yellowAlert.SetActive(false);
            } else if (alertness > 0 && curState != ActionStates.Dead) {
                redAlert.SetActive(false);
                yellowAlert.SetActive(true);
            }
            else {
                redAlert.SetActive(false);
                yellowAlert.SetActive(false);
            }

            //animations
            if (speed < .01) {
                if (alerted) {
                    turnToPlayer();
                    if (curState != ActionStates.Prone) {
                        modelAnimator.Play("pistol idle2kneel");
                        curState = ActionStates.Prone;
                    }
                }
                else {
                    curState = ActionStates.Idle;
                    modelAnimator.Play("pistol idle");
                }
            }
            else {
                curState = ActionStates.Walking;
                modelAnimator.Play("pistol walk");
                // modelAnimator.speed = speed / agent.speed;
            }
            
            
            //shoot player
            if (curState == ActionStates.Prone && LOS) {
                if (lastShot + shootDelay < Time.time) {
                    lastShot = Time.time + Random.Range(-shootOffset, shootOffset);
                    Instantiate(gunFlash, shootPoint.position, Quaternion.identity);
                    if (Random.Range(0f, 1f) < hitChance.Evaluate(dist)) {
                        player.GetComponent<PlayerHealthManager>().DamagePlayer(10);
                    }
                }
                else {
                    
                }
            }

            prevPos = curPos;
        }
    }   
}
