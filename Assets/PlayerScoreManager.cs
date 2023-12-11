using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScoreManager : MonoBehaviour{

    private int enemiesKilled;
    private int barrelsCompleted;
    
    
    
    static public PlayerScoreManager instance;

    private void Awake(){
        if (instance != null && instance != this) { Destroy(this); } 
        else { instance = this; } 
    }

    public void IncrementKillCount(){
        enemiesKilled++;
    }

    public void IncrementBarrels(){
        barrelsCompleted++;
        print(barrelsCompleted);
    }
    
    public float GetDifficulty(){
        //replace with good difficulty calc
        float dif = Mathf.Log(enemiesKilled + barrelsCompleted);
        print($"{dif}");
        return dif;
    }
    
    public void ResetScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
