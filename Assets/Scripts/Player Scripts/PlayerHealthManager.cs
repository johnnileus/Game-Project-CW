using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour{

    //singleton
    public static PlayerHealthManager instance;
    
    [SerializeField] private float maxHealth;
    [SerializeField] private AnimationCurve vignetteRadius;
    [SerializeField] private VignetteEffect vignetteScript;
    [SerializeField] private GameObject gameOverCanvas;

    private float lastDamaged;
    [SerializeField] private float healDelay;
    [SerializeField] private float healRate;

    private float health;
    // Start is called before the first frame update
    void Start(){
        health = maxHealth;
    }
    
    private void Awake(){
        if (instance != null && instance != this) { Destroy(this); } 
        else { instance = this; } 
    }

    private void UpdateVignette(){
        vignetteScript.radius = vignetteRadius.Evaluate(health / maxHealth);
    }
    
    public void DamagePlayer(float amt){
        health -= amt;
        if (health <= 0) {
            health = 0;
            gameOverCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        lastDamaged = Time.time;
        UpdateVignette();
    }

    public void HealPlayer(float amt){
        health += amt;
        if (health >= maxHealth) {
            health = maxHealth;
        }
        UpdateVignette();
    }
    
    // Update is called once per frame
    void Update()
    {
        //autoheal
        if (lastDamaged + healDelay < Time.time) {
            HealPlayer(healRate * Time.deltaTime);
        }
    }
}
