using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPool : MonoBehaviour {
    public GameObject poolingTarget = null; //오브젝트 풀링 할 대상
    public int poolSize = 0;                //오브젝트 풀의 크기
    private RectTransform tr;                   //중복 참조를 막기 위해 미리 트랜스폼 주소를 갖고있음
    private int index = 0;                  //외부에서 오브젝트를 생성해달라고 요청했을 때 보낼 오브젝트 번호
	// Use this for initialization
	void Awake () {
        tr = GetComponent<RectTransform>();
        InitObjectPool();
	}
    
    void InitObjectPool()   //오브젝트 풀 초기화(오브젝트 생성)
    {
        if (poolSize > 0)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject temp = Instantiate(poolingTarget) as GameObject;
                temp.GetComponent<RectTransform>().SetParent(tr);
                temp.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                temp.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 외부에서 오브젝트 요청할 때 사용.
    /// 리턴값이 null이면 모든 오브젝트가 사용중이라 요청에 응답할 수 없음(왠만해서는 이럴 일이 없을 것 같음)
    /// 그게 아니라면 비활성화된 오브젝트 리턴
    /// </summary>
    /// <returns></returns>
    public GameObject GetObject()
    {
        index = -1;
        for(int i=0;i<tr.childCount;i++)
        {
            if (tr.GetChild(i).gameObject.activeSelf == true)
                continue;
            else
            {
                index = i;
                break;
            }
        }
        if (index == -1)
            return null;
        else
            return tr.GetChild(index).gameObject;
    }
}
