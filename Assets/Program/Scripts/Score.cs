using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
    private Text scoreText;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        scoreText = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        UpdateScore();
	}

    void UpdateScore()
    {
        scoreText.text = GameManager.Instance.Score.ToString("00000");
    }
}
