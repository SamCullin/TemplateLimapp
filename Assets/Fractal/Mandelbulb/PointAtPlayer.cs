using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public Transform player;
    [SerializeField]
    public Material mat;

    [SerializeField]
    public float speed = 1;
    [SerializeField]
    [Range(2, 20)]
    public float fractalMotion = 2;

    private bool directionForward = true;



    void Start()
    {
        if(this.mat == null)
        {
            this.mat = this.GetComponent<Material>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(fractalMotion < 20 && directionForward)
        {
            fractalMotion += speed * Time.deltaTime;
            if(fractalMotion > 20)
            {
                directionForward = false;
            }
        }
        else
        {
            fractalMotion -= speed * Time.deltaTime;
            if(fractalMotion < 2)
            {
                directionForward = true;
            }
        }
        this.mat.SetFloat("_Exponent", fractalMotion);
        var targetRotation = Quaternion.LookRotation( this.transform.position - this.player.position); // Flipped To show the correct side of the shader
        transform.rotation = targetRotation;
        //this.transform.LookAt()
        //this.transform.rotation.SetLookRotation(player.transform.position);
    }
}
