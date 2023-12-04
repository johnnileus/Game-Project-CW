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
    
    private float alertness = 0;
    [SerializeField] private float alertIncRate;
    [SerializeField] private float alertDecRate;
    
    private NavMeshAgent agent;
    private GameObject player;
    [SerializeField] private GameObject coverPoints;
    [SerializeField] private Animator modelAnimator;
    [SerializeField] private LayerMask layersToIgnore;
    [SerializeField] private float enemyHeadHeight;

    [SerializeField] private float patrolDelay;
    private float lastPatrol;


    private Vector3 prevPos;
    private enum AnimState{
        Idle,
        Walking,
        Running,
        Prone,
        Dead
    }

    private bool alerted = false;
    private AnimState currentAnim = AnimState.Idle;

    
    // Start is called before the first frame update
    void Start(){
        health = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        lastPatrol = Time.time;
    }

    public void DealDamage(float dmg){
        health -= dmg;
        if (health <= 0) {
            modelAnimator.Play("pistol death");
            health = 0;
        }
        currentAnim = AnimState.Dead;
        print($"{health} {dmg}");
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
        if (currentAnim != AnimState.Dead) {

            Vector3 curPos = transform.position + new Vector3(0, enemyHeadHeight, 0);
            Vector3 playerPos = player.transform.position;
            float speed = Vector3.Distance(curPos, prevPos) / Time.deltaTime;



            //check for los
            if (Physics.Linecast(curPos, playerPos, ~layersToIgnore)) {
                alertness -= alertDecRate * Time.deltaTime;
                if (alertness < 0) {
                    alertness = 0;
                }
            }
            else {
                alertness += alertIncRate * Time.deltaTime;
            }

            if (!alerted) {
                if (alertness > 1) {
                    SetAlert(true);
                }

                if (lastPatrol + patrolDelay < Time.time) {
                    SetNewPatrol();
                    lastPatrol = Time.time;
                }
            }

            //animations
            if (speed < .01) {
                modelAnimator.Play("pistol idle");
                if (alerted) {
                    turnToPlayer();
                }
            }
            else {
                modelAnimator.Play("pistol walk");
                // modelAnimator.speed = speed / agent.speed;
            }



            prevPos = curPos;
        }
    }   
}
