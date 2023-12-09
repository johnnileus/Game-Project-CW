using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BarrelController : MonoBehaviour{
    private FlockingController flock;
    private Camera cam;
    private float startTime;
    private bool ended;
    private bool won;

    [SerializeField] private float minigameTime;
    [SerializeField] private LayerMask barrelPlaneLayer;

    [SerializeField] private GameObject BarrelCanvas;
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private GameObject LoseScreen;

    [HideInInspector] public GameObject[] enableOnExit;
    
    public void SendToWorld(){
        foreach (var i in enableOnExit) {
            i.SetActive(true);
            if (won) {
                PlayerHealthManager.instance.HealPlayer(100);
            }
        }
        

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CursorManager.instance.SetCursor(false);
        Destroy(gameObject);
    }
    
    // Start is called before the first frame update
    void Start(){
        flock = transform.GetComponent<FlockingController>();
        startTime = Time.time;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CursorManager.instance.SetCursor(true);
        
        cam = transform.GetComponentInChildren<Camera>();

        BarrelCanvas = transform.GetChild(0).gameObject;
        BarrelCanvas.SetActive(false);
        WinScreen = BarrelCanvas.transform.GetChild(0).gameObject;
        LoseScreen = BarrelCanvas.transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update(){
        if (!ended) {
            if (startTime + minigameTime < Time.time) {
                BarrelCanvas.SetActive(true);
                LoseScreen.SetActive(true);
                ended = true;
            }
            else {
                if (Input.GetMouseButtonDown(0)) {

                    RaycastHit hit;
                    Vector3 destination;
                    Vector2 mousePos = Input.mousePosition;
                    Ray ray = cam.ViewportPointToRay(new Vector3(mousePos.x/Screen.width, mousePos.y/Screen.height, 0f));
                    print(mousePos);
                    if (Physics.Raycast(ray, out hit, 100, barrelPlaneLayer)) {
                        print(hit.point);
                        destination = hit.point - transform.position;
                        Vector2 pos = new Vector2(destination.x, destination.z);
                        flock.AttackFish(pos);
                    }
                    int rem = flock.GetFishCount();
                    if (rem == 0) {
                        BarrelCanvas.SetActive(true);
                        WinScreen.SetActive(true);
                        ended = true;
                        won = true;

                    }
                }

            }
        
        }
    }
}
