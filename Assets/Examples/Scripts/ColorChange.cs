using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChange : MonoBehaviour
{
    [SerializeField]
    
    private GameObject sphere;
    private Renderer sphereRenderer;
    private Color newSphereColor;
    private float random1, random2, random3;



    // Start is called before the first frame update
    void Start()
    {
        sphereRenderer = sphere.GetComponent<Renderer>();
        gameObject.GetComponent<Button>().onClick.AddListener(ChangeSphereColor);


    }

        
        private void ChangeSphereColor()
        {
            random1 = Random.Range(0f,1f);
            random2 = Random.Range(0f,1f);
            random3 = Random.Range(0f,1f);

            newSphereColor = new Color(random1, random2, random3, 1f);
            sphereRenderer.material.SetColor("_Color", newSphereColor);
        }









    
}
