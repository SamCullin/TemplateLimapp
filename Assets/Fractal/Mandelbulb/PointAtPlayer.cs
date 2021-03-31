using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform player;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var targetRotation = Quaternion.LookRotation( this.transform.position - this.player.position); // Flipped To show the correct side of the shader
        transform.rotation = targetRotation;
        //this.transform.LookAt()
        //this.transform.rotation.SetLookRotation(player.transform.position);
    }
}
