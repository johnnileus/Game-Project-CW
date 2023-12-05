using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VignetteEffect : MonoBehaviour{

    public float radius;
    public float feather;
    public Color colour;
    public Shader shader;
    private Material mat;
    
    void Awake(){
        mat = new Material(shader);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination){
        mat.SetFloat("_Radius", radius);
        mat.SetFloat("_Feather", feather);
        mat.SetColor("_TintColour", colour);
        Graphics.Blit(source,destination,mat);
    }
}
