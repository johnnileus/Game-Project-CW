using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerWeaponController : MonoBehaviour{


    [SerializeField] private LayerMask layersToIgnore;

    [SerializeField] private GameObject SparkPS;
    [SerializeField] private GameObject BloodPS;

    [SerializeField] private float pistolShootDelay;
    private float lastShot;


    private Camera cam;

    // Start is called before the first frame update
    void Start(){
        lastShot = Time.time;
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        //checks if timer to shoot is over when lmb is clicked
        if (Input.GetMouseButton(0) && lastShot + pistolShootDelay < Time.time) {
            lastShot = Time.time; 
            RaycastHit hit;
            Vector3 destination;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            
            //raycast from camera
            if (Physics.Raycast(ray, out hit, 100, ~layersToIgnore)) {
                destination = hit.point;
            
            }
            else {
                destination = ray.GetPoint(100);
            }
            
            //if ray hit an enemy's hitbox
            if (hit.collider && hit.collider.gameObject.CompareTag("EnemyHitbox")) {
                Instantiate(BloodPS, destination, Quaternion.identity, gameObject.transform);
                hit.collider.gameObject.GetComponent<EnemyHitbox>().OnHit(1);
            }
            else {
                Instantiate(SparkPS, destination, Quaternion.identity, gameObject.transform);
            }
        }
    }
}
