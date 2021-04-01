using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TestScript : MonoBehaviour
{
    [System.Serializable]
    public struct ColorItem
    {
        public bool enabled;
        public string materialValue;
        public Color startValue;
        public Color endValue;
    }

    [System.Serializable]
    public struct FloatItem
    {
        public bool enabled;
        public string materialValue;
        public float startValue;
        public float endValue;
    }

    [SerializeField]
    private FloatItem Exponent;
    [SerializeField]
    private ColorItem PrimaryColor;
    [SerializeField]
    private ColorItem SecondaryColor;
    [SerializeField]
    private ColorItem GlowColor;

    [SerializeField]
    public Material mat;

    [SerializeField]
    public float speed = 0.1f;
    [SerializeField]
    [Range(0, 1)]
    public float fractalMotion = 0;
    private bool directionForward = true;



    void Start()
    {
        if (this.mat == null)
        {
            this.mat = this.GetComponent<Material>();
        }
    }

    float lerpFloatItem(FloatItem item, float lerper)
    {
        return Mathf.Lerp(item.startValue, item.endValue, lerper);
    }

    Color lerpColorItem(ColorItem item, float lerper)
    {
        return Vector4.Lerp(item.startValue, item.endValue, lerper);
    }





    // Update is called once per frame
    void Update()
    {
        if (directionForward)
        {
            fractalMotion += speed * Time.deltaTime;
            if (fractalMotion >= 1)
            {
                directionForward = false;
            }
        }
        else
        {
            fractalMotion -= speed * Time.deltaTime;
            if (fractalMotion <= 0)
            {
                directionForward = true;
            }
        }
        if (this.Exponent.enabled)
        {
            this.mat.SetFloat(this.Exponent.materialValue, lerpFloatItem(this.Exponent, fractalMotion));
        }
        if (this.PrimaryColor.enabled)
        {
            this.mat.SetVector(this.PrimaryColor.materialValue, lerpColorItem(this.PrimaryColor, fractalMotion));
        }
        if (this.SecondaryColor.enabled)
        {
            this.mat.SetVector(this.SecondaryColor.materialValue, lerpColorItem(this.SecondaryColor, fractalMotion));
        }

        if (this.GlowColor.enabled)
        {
            this.mat.SetVector(this.GlowColor.materialValue, lerpColorItem(this.GlowColor, fractalMotion));
        }


    }
}
