using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shop : MonoBehaviour {
    private ScrollRect scroll;

	// Use this for initialization
	void Awake () {
        scroll = GetComponent<ScrollRect>();
	}

    private void OnEnable()
    {
        float width = scroll.content.GetComponent<GridLayoutGroup>().cellSize.x * scroll.content.transform.childCount;
        scroll.content.sizeDelta = new Vector2(width, scroll.content.sizeDelta.y);
    }

    public void ShopExit()
    {
        SceneManager.LoadScene("Main");
    }
}
