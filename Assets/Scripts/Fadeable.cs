using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fadeable : MonoBehaviour
{   
    private Material mat;
    public float speed = 0.01f;
    public string ColorPropertyName = "_Color";

    private bool fadingIn;
    private bool fadingOut;
    

    void Start()
    {
        mat = gameObject.GetComponent<MeshRenderer>().material;
    }

    public void FadeIn()
    {
        fadingIn = true;
        fadingOut = false;
    }

    public void FadeOut()
    {
        fadingIn = false;
        fadingOut = true;
    }
    
    void Update()
    {
        if (!fadingIn && !fadingOut) return;
        var col = mat.GetColor(ColorPropertyName);
        if (fadingIn)
        {
            
            if (col.a >= 1f)
            {
                fadingIn = false;
                return;
            }
            col.a += speed;
            mat.SetColor(ColorPropertyName, col);
        }
        else if (fadingOut)
        {
            if (col.a <= 0f)
            {
                fadingOut = false;
                return;
            }
            col.a -= speed;
            mat.SetColor(ColorPropertyName, col);
        }
    }
}
