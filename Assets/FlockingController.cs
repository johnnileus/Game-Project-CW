using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FlockingController : MonoBehaviour{

    [SerializeField] private GameObject FishesGO;
    [SerializeField] private GameObject FishPrefab;

    private List<Fish> fishes = new List<Fish>();

    [SerializeField] private int fishAmt;
    [SerializeField] private float fishSpeed;
    [SerializeField] private float visionDist;
    [SerializeField] private float barrelSize;
    [SerializeField] private float barrelEdgeSize;
    [SerializeField] private float edgeForce;
    [SerializeField] private float alignmentStrength;
    [SerializeField] private float repulsionStrength;
    [SerializeField] private float maxRepulsion;


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
            Vector3 pos = new Vector3(0, 0, 0);
            GameObject newFishGO = Instantiate(FishPrefab, pos, Quaternion.identity, FishesGO.transform);
        }
    }

    private void UpdateFish(){
        for (int i = 0; i < fishAmt; i++) {
            Transform child = FishesGO.transform.GetChild(i);
            Fish fish = fishes[i];

            SetFishGOPos(fish, child);
            Vector3 vel = new Vector3(fish.vel.x, 0, fish.vel.y);
            child.transform.LookAt(child.transform.position + vel);
        }
    }

    private void Update(){
        for (int i = 0; i < fishAmt; i++) {
            Fish fish = fishes[i];
            fish.pos += fish.vel * Time.deltaTime;
            float distToCenter = Vector2.Distance(fish.pos, Vector2.zero);

            Vector2 totalVel = Vector2.zero;
            Vector2 totalPos = Vector2.zero;
            Vector2 totalRepulsion = Vector2.zero;

            for (int j = 0; j < fishAmt; j++) {
                if (i != j) {
                    float dist = Vector2.Distance(fish.pos, fishes[j].pos);
                    if (dist < visionDist) {
                        // alignment
                        totalVel += fishes[j].vel;
                        // cohesion
                        totalPos += fishes[j].pos;
                        // repulsion
                        Vector2 vecToFish = (fishes[j].pos - fish.pos).normalized;
                        totalRepulsion += vecToFish * (1 / Vector2.Distance(fish.pos, fishes[j].pos));

                    }   
                }
            }
            

            
            Vector2 avgVel = totalVel / fishAmt;
            Vector2 avgPos = totalPos / fishAmt;
            Vector2 avgRepulsion = totalRepulsion / fishAmt;

            fish.vel = Vector2.Lerp(fish.vel, avgVel,Mathf.Min(alignmentStrength* Time.deltaTime, 1));
            
            Vector2 repulsion = avgRepulsion * repulsionStrength * Time.deltaTime;
            if (repulsion.magnitude > maxRepulsion) {
                repulsion = repulsion.normalized * maxRepulsion;
            }

            fish.vel -= repulsion;
            
            //reflect if outside of barrel
            if (distToCenter > barrelSize) {
                fish.vel = Vector2.Reflect(fish.vel, -fish.pos.normalized);
                fish.pos.Normalize();
                fish.pos *= barrelSize - .01f;
            } //push away from edge of barrel
            else if (distToCenter > barrelEdgeSize) {
                fish.vel -= edgeForce * fish.pos * Time.deltaTime;
            }

            
            
            
            float speedMag = fish.vel.magnitude;
            fish.vel = fish.vel * fishSpeed / speedMag;

            fishes[i] = fish;
            UpdateFish();
        }
    }
}