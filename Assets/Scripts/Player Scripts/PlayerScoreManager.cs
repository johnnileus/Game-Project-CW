using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScoreManager : MonoBehaviour{

    private int enemiesKilled;
    private int barrelsCompleted;
    
    
    //singleton
    public static PlayerScoreManager instance;

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
        float dif = Mathf.Log(enemiesKilled + barrelsCompleted);// replace with good difficulty calc
        print($"{dif}");
        return dif;
    }
    
    public void ResetScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
