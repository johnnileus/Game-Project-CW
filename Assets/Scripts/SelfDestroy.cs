using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attach to object to destroy with timer
public class SelfDestroy : MonoBehaviour{
    [SerializeField] private float timeToLive;
    private float initTime;
    
    // Start is called before the first frame update
    void Start(){
        initTime = Time.time;
    }

    // Update is called once per frame
    void Update(){
        if (initTime + timeToLive < Time.time) {
            Destroy(gameObject);
        }
    }
}
