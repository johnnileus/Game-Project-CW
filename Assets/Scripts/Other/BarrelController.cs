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

     private GameObject BarrelCanvas;
     private GameObject WinScreen;
     private GameObject LoseScreen;

    [HideInInspector] public List<GameObject> enableOnExit;
    
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
        PlayerScoreManager.instance.IncrementBarrels();
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
    
    void Update(){
        if (!ended) {
            if (startTime + minigameTime < Time.time) { // check if time ran out
                BarrelCanvas.SetActive(true);
                LoseScreen.SetActive(true);
                ended = true;
            }
            else {
                
                
                //raycast from camera to barrel plane (plane is aligned with fish objects)
                RaycastHit hit;
                Vector3 destination;
                Vector2 mousePos = Input.mousePosition;
                Ray ray = cam.ViewportPointToRay(new Vector3(mousePos.x/Screen.width, mousePos.y/Screen.height, 0f));
                Vector2 pos = Vector2.zero;
                if (Physics.Raycast(ray, out hit, 100, barrelPlaneLayer)) {
                    destination = hit.point - transform.position;
                    pos = new Vector2(destination.x, destination.z);
                    
                }
                
                
                if (Input.GetMouseButtonDown(0)) {
                    flock.AttackFish(pos);
                    
                    int rem = flock.GetFishCount();
                    if (rem == 0) { // if no fish left
                        BarrelCanvas.SetActive(true);
                        WinScreen.SetActive(true);
                        ended = true;
                        won = true;

                    }
                }
                else {
                    flock.UpdateMousePos(pos); // update mouse position on plane for mouse avoidance
                }

            }
        
        }
    }
}
