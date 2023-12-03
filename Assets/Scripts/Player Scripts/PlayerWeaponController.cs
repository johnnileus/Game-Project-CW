using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour{


    [SerializeField] private LayerMask layersToIgnore;

    [SerializeField] private GameObject testball;

    [SerializeField] private float shootDelay;
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
        //test if can shoot twice in frame
        if (Input.GetMouseButton(0) && lastShot + shootDelay < Time.time) {
            lastShot = Time.time; 
            RaycastHit hit;
            Vector3 destination;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out hit, 100, ~layersToIgnore)) {
                destination = hit.point;
            
            }
            else {
                destination = ray.GetPoint(100);
            }

            //hit.collider.gameObject;
            if (hit.collider && hit.collider.gameObject.tag == "EnemyHitbox") {
                print(hit.collider.gameObject.GetComponent<EnemyHitbox>().hitboxSection);
                hit.collider.gameObject.GetComponent<EnemyHitbox>().OnHit();
            }
            else {
                print("other");
            }
            
            //testball.transform.position = destination;
        }
    }
}
