
using UnityEngine;
using System.Collections;
 
// Makes objects float 
public class Floating : MonoBehaviour {
    // User Inputs
    public float degreesPerSecond = 0.1f;
    public float amplitude = 0.5f;
    public float frequency = 0.5f;
 
    // Position Storage Variables
    Vector3 posOffset = new Vector3 ();
    Vector3 tempPos = new Vector3 ();
 
    // Use this for initialization
    void Start () {
        // Store the starting position & rotation of the object
        posOffset = transform.position;
    }
     
    // Update is called once per frame
    void Update () {
        // Spin object around Y-Axis
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
 

 float number = Random.Range (0f, 0.5f);


        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin (Time.fixedTime * frequency * 0.1f) * amplitude * number;
        tempPos.z += Mathf.Sin (Time.fixedTime * frequency * 0.1f) * amplitude * number;
        tempPos.x += Mathf.Sin (Time.fixedTime * frequency * 0.1f) * amplitude * number;
        transform.position = tempPos;
    }
}