using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpGauge : MonoBehaviour {
    private Image curHp;

    void Awake()
    {
        Init();    
    }

    void Init()
    {
        curHp = GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
        UpdateGauge();
	}

    void UpdateGauge()
    {
        curHp.fillAmount = GameManager.Instance.GetHpPercentage();
    }
}
