using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public Material fractal;


    // Start is called before the first frame update
    void Awake()
    {
        
        if(this.fractal == null)
        {
            fractal = this.GetComponent<Material>();
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {

        fractal.SetVector("_Position", this.transform.position);

        Graphics.Blit(src, dst, fractal);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
