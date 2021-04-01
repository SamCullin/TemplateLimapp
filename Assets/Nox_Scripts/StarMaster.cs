using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMaster : MonoBehaviour
{
    private float yAngle = 0.0f;
    private GameObject stars;
    // Start is called before the first frame update
    void Start()
    {
        stars = GameObject.FindWithTag("stars");
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateStar();
        stars.transform.Rotate(0.0f, yAngle, 0.0f, Space.Self);
    }

    void RotateStar()
    {
        if (yAngle <= 360)
            yAngle += 0.00001f;
        else
            yAngle = 0.0f;
    }
}
