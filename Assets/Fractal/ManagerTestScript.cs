using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;




public class ManagerTestScript : MonoBehaviour
{

    public TestEvent listener = new TestEvent();

    [SerializeField()]
    public float rspeed = 1;

    [SerializeField()]
    public float gspeed = 2;

    [SerializeField()]
    public float bspeed = 3;


    public float red = 0;

    public float green = 0;

    public float blue = 0;


    void Start()
    {

    }

    float UpdateColor(float color, float speed)
    {
        var diff = speed * Time.deltaTime;
        var new_color = (color + diff) > 1 ? color - diff : color + diff ;
        //new_color = Mathf.Clamp(Mathf.Lerp(color, new_color, 1f), 0f, 1f);
        return new_color;
    }

    // Update is called once per frame
    void Update()
    {

        red = UpdateColor(red, rspeed);
        green = UpdateColor(green, gspeed);
        blue = UpdateColor(blue, bspeed);


        var color = new Color(red, green, blue);
        this.listener.Invoke(color);
    }
}

public class TestEvent: UnityEvent<Color>
{

}

