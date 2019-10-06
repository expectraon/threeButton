using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {
    private static TimeManager sInstance;
    public static TimeManager Instance
    {
        get
        {
            if(sInstance==null)
            {
                GameObject newObject = new GameObject("TimeManager");
                sInstance = newObject.AddComponent<TimeManager>();
            }
            return sInstance;
        }
    }

    //[System.NonSerialized]
    public float time = 0;
    //[System.NonSerialized]
    public float deltaTime = 0;
    //[System.NonSerialized]
    public float timeScale = 1;
    private float beforeTimeScale = 1;
    private float fixedDeltatime = 0;
    private Vector3 gravity;
    void Awake()
    {
        sInstance = this;    
    }

    // Use this for initialization
    void Start () {
        time = Time.time;
        deltaTime = Time.deltaTime;
        timeScale = Time.timeScale;
        beforeTimeScale = Time.timeScale;
        fixedDeltatime = Time.fixedDeltaTime;
        gravity = Physics.gravity;    
    }

    // Update is called once per frame
    void Update () {
        if (beforeTimeScale != timeScale)
        {
            beforeTimeScale = timeScale;
            deltaTime = Time.deltaTime * timeScale;
        }
        time += deltaTime;
	}
}
