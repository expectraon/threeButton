using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseUI : MonoBehaviour {
    private Text phase;
	// Use this for initialization
	void Awake () {
        phase = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        phase.text = (GameManager.Instance.PhaseNumber+1).ToString();
	}
}
