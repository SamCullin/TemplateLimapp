using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private float secondsCounter;

    private void Awake()
    {
        MakeGameManager();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        

    }

    
    void MakeGameManager()
    {
        // implementing singleton pattern with this class
        if(instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.secondsCounter = Time.timeSinceLevelLoad;
    }

    public float GetSeconds()
    {
        return secondsCounter;
    }

}
