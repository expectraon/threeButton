using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    private Text coinText;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        coinText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCoin();
    }

    void UpdateCoin()
    {
        coinText.text = GameManager.Instance.Score.ToString("000000");
    }
}
