using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour{
    private float health;
    private NavMeshAgent agent;
    private GameObject player;

    [SerializeField] private GameObject coverPoints;
    [SerializeField] private Animator modelAnimator;

    [SerializeField] private float patrolDelay;
    private float lastPatrol;



    private enum AnimState{
        Idle,
        Walking,
        Running,
        Prone
    }

    private bool alerted = false;
    private AnimState currentAnim = AnimState.Idle;

    [SerializeField] private float maxHealth;
    // Start is called before the first frame update
    void Start(){
        health = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");;
        lastPatrol = Time.time;
    }

    public void DealDamage(float dmg){
        health -= dmg;
        print($"{health} {dmg}");
    }

    private void SetNewPatrol(){
        float angle = Random.Range(0, 2*Mathf.PI);
        Vector3 newPoint = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        agent.SetDestination(newPoint * 3 + transform.position);
    }

    private Vector3 GetClosestCover(Vector3 pos){
        float dist = 99999;
        Transform closestCover = coverPoints.transform.GetChild(0);
        
        foreach (Transform child in coverPoints.transform) {
            float newDist = Vector3.Distance(pos, child.position);
            if (newDist < dist) {
                closestCover = child;
                dist = newDist;
            }
        }

        return closestCover.position;
    }
    
    // Update is called once per frame
    void Update(){

        
        
        Vector3 currentPos = transform.position;
        Vector3 playerPos = player.transform.position;
        
        
        //distance alert
        // if (Vector3.Distance(playerPos, gameObject.transform.position) < 5) {
        //     alerted = true;
        // }

        
        if (alerted) {
            print("a");
            //agent.destination = getClosestCover(currentPos);
            agent.SetDestination(GetClosestCover(currentPos));
            // find closest cover
            //print(getClosestCover(currentPos));
        }
        else {
            if (lastPatrol + patrolDelay < Time.time) {
                SetNewPatrol();
                lastPatrol = Time.time;
            }
        }
    }
}
