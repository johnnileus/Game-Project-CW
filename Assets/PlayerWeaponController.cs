using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour{

    private Camera cam;
    [SerializeField] private LayerMask layersToIgnore;

    [SerializeField] private GameObject testball;


    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) {
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
            print(hit.collider.gameObject.GetComponent<EnemyHitbox>().hitboxSection);
            testball.transform.position = destination;
        }
    }
}
