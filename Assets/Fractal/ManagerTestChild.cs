using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerTestChild : MonoBehaviour
{
    [SerializeField]
    public ManagerTestScript parent;


    private Renderer mat; 

    public void Start()
    {
        if (this.mat == null)
        {
            this.mat = this.GetComponent<Renderer>();
        }

        this.parent.listener.AddListener(this.UpdateColor);

    }


    public void UpdateColor(Color color)
    {

        this.mat.material.color = color;
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}