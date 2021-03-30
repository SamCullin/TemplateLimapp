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
        
        this.secondsCounter = Time.timeSinceLevelLoad;
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
        
    }

    public float GetSeconds()
    {
        return secondsCounter;
    }

}
