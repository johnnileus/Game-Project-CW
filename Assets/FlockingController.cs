using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine.Utility;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FlockingController : MonoBehaviour{

    [Header("Fish")]
    [SerializeField] private GameObject FishesGO;
    [SerializeField] private GameObject FishPrefab;
    [SerializeField] private int fishAmt;
    [SerializeField] private float fishSpeed;
    [SerializeField] private float visionDist;
    
    [Header("Barrel")]
    [SerializeField] private float barrelSize;
    [SerializeField] private float barrelEdgeSize;
    [SerializeField] private float edgeForce;
    
    [Header("Repulsion")]
    [SerializeField] private float repulsionVisionDist;
    [SerializeField] private float repulsionStrength;

    [SerializeField] private float maxRepulsion;
    
    [Header("Cohesion")]
    [SerializeField][Range(0f,.99f)] private float cohesionStrength;

    [Header("Alignment")]
    [SerializeField][Range(0f,.99f)] private float alignmentStrength;

    [Header("Avoidance")] // from mouse
    [SerializeField] private float avoidanceStrength;
    [SerializeField] private float avoidanceRadius;
    
    [Header("Other")] [SerializeField] private float gunRadius;

    private List<Fish> fishes = new List<Fish>();
    private Vector2 mousePos;
    
    private struct Fish{
        public Vector2 pos;
        public Vector2 vel;
    }

    private void SetFishGOPos(Fish fish, Transform GO){
        GO.position = new Vector3(fish.pos.x, 0, fish.pos.y) + FishesGO.transform.position;
    }

    private void Start(){
        for (int i = 0; i < fishAmt; i++) {
            Fish newFish = new Fish();
            newFish.pos = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            newFish.vel = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * fishSpeed;
            fishes.Add(newFish);
            Vector3 pos = new Vector3(0, Random.Range(-1f, 1f), 0);
            GameObject newFishGO = Instantiate(FishPrefab, pos, Quaternion.identity, FishesGO.transform);
        }
    }

    public void UpdateMousePos(Vector2 newPos){
        mousePos = newPos;
    }
    
    //update fish game objects to line up with fishes array
    private void UpdateFish(){
        for (int i = 0; i < fishAmt; i++) {
            Transform child = FishesGO.transform.GetChild(i);
            Fish fish = fishes[i];
            
            SetFishGOPos(fish, child);
            
            Vector3 vel = new Vector3(fish.vel.x, 0, fish.vel.y);
            child.transform.LookAt(child.transform.position + vel);
        }
    }

    public int GetFishCount(){
        return fishes.Count;
    }
    
    public void AttackFish(Vector2 pos){
        print(pos);
        for (int i = fishAmt - 1; i >= 0; i--) {
            if (Vector2.Distance(pos, fishes[i].pos) < gunRadius) {
                KillFish(i);
            }
        }
    }
    
    private void KillFish(int num){
        fishes.RemoveAt(num);
        Destroy(FishesGO.transform.GetChild(num).gameObject);
        fishAmt--;
        print($"killed fish {num}");
        
    }
    //improve
    private void Update(){
        for (int i = 0; i < fishAmt; i++) {
            Fish fish = fishes[i];
            fish.pos += fish.vel / 100;
            float distToCenter = Vector2.Distance(fish.pos, Vector2.zero);

            Vector2 totalVel = Vector2.zero;
            Vector2 totalPos = Vector2.zero;
            Vector2 totalRepulsion = Vector2.zero;
            float fishInRange = 0;
            float fishInRangeRep = 0;
            for (int j = 0; j < fishAmt; j++) {
                if (i != j) {
                    float dist = Vector2.Distance(fish.pos, fishes[j].pos);
                    if (dist < visionDist) {
                        fishInRange++;
                        // alignment
                        totalVel += fishes[j].vel;
                        // cohesion
                        totalPos += fishes[j].pos;
                        // repulsion
                        if (dist < repulsionVisionDist) {
                            fishInRangeRep++;
                            Vector2 vecToFish = (fishes[j].pos - fish.pos).normalized;
                            Vector2 fishRep = vecToFish * (1 / Vector2.Distance(fish.pos, fishes[j].pos));
                            totalRepulsion += fishRep;
                            
                            
                        }

                    }   
                }
            }


            if (fishInRange > 0) {
                Vector2 avgVel = totalVel / fishAmt; //alignment
                Vector2 avgPos = totalPos / fishInRange; //cohesion
                
                //alignment
                Vector2 newVel = Vector2.Lerp(fish.vel, avgVel, alignmentStrength);
                fish.vel = newVel;
                
                //cohesion
                Vector2 cohesion = Vector2.Lerp(fish.pos, avgPos, cohesionStrength);
                fish.vel -= (fish.pos - cohesion);
            }

            if (fishInRangeRep > 0) {
                Vector2 avgRepulsion = totalRepulsion / fishAmt; //repulsion
                
                //repulsion
                Vector2 repulsion = avgRepulsion * repulsionStrength;
                if (repulsion.magnitude > maxRepulsion) {
                    repulsion = repulsion.normalized * maxRepulsion;
                }
                fish.vel -= repulsion;
            }

            //reflect if outside of barrel
            if (distToCenter > barrelSize) {
                fish.vel = Vector2.Reflect(fish.vel, -fish.pos.normalized);
                fish.pos.Normalize();
                fish.pos *= barrelSize - .01f;
            } //push away from edge of barrel linearly
            else if (distToCenter > barrelEdgeSize) {

                float edgeDepth = fish.pos.magnitude - barrelEdgeSize;
                float repulsionFromEdge = edgeDepth / (barrelSize - barrelEdgeSize) * edgeForce;
                fish.vel -= fish.pos.normalized * repulsionFromEdge;

            }

            //avoid mouse
            float distToMouse = Vector2.Distance(mousePos, fish.pos);
            if (distToMouse < avoidanceRadius) {
                fish.vel += (fish.pos - mousePos).normalized / distToMouse * avoidanceStrength;
            }
            
            //lerp to normalise
            float speedMag = fish.vel.magnitude;
            fish.vel = fish.vel * fishSpeed / speedMag;

            fishes[i] = fish;
            UpdateFish();
        }
    }
}