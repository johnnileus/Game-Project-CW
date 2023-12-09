using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour{

    public static CursorManager instance { get; private set; }

    [SerializeField] private Texture2D barrelCursor;
    private bool active = false;


    public void SetCursor(bool active){
        this.active = active;
        if (active) {
            Vector2 cursorCenter = new Vector2(barrelCursor.width, barrelCursor.height) / 2;
            Cursor.SetCursor(barrelCursor, cursorCenter, CursorMode.Auto);
        }
        else {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

    }

    private void Awake(){
        if (instance != null && instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            instance = this; 
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
