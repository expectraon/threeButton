using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Global;

public class GameManager : MonoBehaviour {
    private static GameManager sInstance;   //싱글톤 생성용 변수
    public static GameManager Instance      // ""
    {
        get
        {
            return sInstance;
        }
    }
    private int routeCount = 4;             //루트 갯수
    public ObjectPool targetPool;           //타겟 생성용
    public RectTransform[] spawnPoint;      //타겟 생성 위치
    private Target recentTarget = null;     //가장 아래 있는 타겟
    private Queue<GameObject> targetQueue;  //타겟 생성 순서대로 들어가는 큐
    private bool[] isTargetExist;           //해당 루트에 타겟이 있는지 확인
    private float spawnTime = 0;            //마지막 생성된 시간
    private float spawnInterval = 5;        //생성 간격
    private float curHp = 0;                //현재 체력
    private float maxHp = 0;                //최대 체력
    private float score = 0;                //점수
    public float Score                      //점수 참조용 프로퍼티
    {
        get
        {
            return score;
        }
    }
    private float coin = 0;                 //코인
    public float Coin                      //코인 참조용 프로퍼티
    {
        get
        {
            return coin;
        }
    }

    private float maxSkillGauge = 10;        //최대 스킬 게이지(이 수치만큼 모이면 스킬 사용 가능)
    public float MaxSkillGauge               //최대 스킬 게이지 참조용 프로퍼티
    {
        get
        {
            return maxSkillGauge;
        }
    }
    private float[] curSkillGauge;          //각 버튼 별 스킬 게이지
    private int currentSpawnCount = 0;
    private float gameSpeed = 1;            //게임속도(2면 2배속, 0이면 일시정지)
    public float GameSpeed                  //게임속도 참조용 프로퍼티
    {
        get
        {
            return gameSpeed;
        }
    }
    private int phase = 0;
    public int PhaseNumber
    {
        get
        {
            return phase;
        }
    }

    void Awake () {
        Init();
	}
	
	void Update () {
        Spawn();
        RefreshTarget();
        GameOverCheck();
	}

    /// <summary>
    /// 초기화 함수
    /// </summary>
    void Init()
    {
        Screen.SetResolution(720, 1280, true);
        GLOBAL_VAR.LoadData();
        sInstance = this;
        //Debug.Log(GLOBAL_VAR.phaseInfo[0].speed);
        targetQueue = new Queue<GameObject>();
        isTargetExist = new bool[routeCount];
        spawnTime = TimeManager.Instance.time - spawnInterval;
        curHp = maxHp = 10;
        curSkillGauge = new float[3];
        for(int i=0;i<curSkillGauge.Length;i++)
        {
            curSkillGauge[i] = 0;
        }
    }

    /// <summary>
    /// 타겟 생성 함수
    /// 지정된 시간에 한번씩 생성하는데 만약 모든 루트에 타겟이 있다면 생성하지않는다.
    /// </summary>
    void Spawn()
    {
        if (TimeManager.Instance.time - spawnTime > spawnInterval)
        {
            spawnTime = TimeManager.Instance.time;
            bool isAllTargetExist = true;
            for(int i=0;i<isTargetExist.Length;i++)
            {
                if(isTargetExist[i]==false)
                {
                    isAllTargetExist = false;
                    break;
                }
            }
            if (isAllTargetExist == false)
            {
                int spawnIndex = 0;
                do
                {
                    spawnIndex = Random.Range(0, isTargetExist.Length);
                } while (isTargetExist[spawnIndex] == true);
                GameObject newTarget = targetPool.GetObject();
                newTarget.GetComponent<RectTransform>().position = spawnPoint[spawnIndex].position;
                newTarget.GetComponent<Target>().SetIndex(spawnIndex);
                targetQueue.Enqueue(newTarget);
                newTarget.SetActive(true);
                isTargetExist[spawnIndex] = true;
                currentSpawnCount++;
                if(currentSpawnCount == GLOBAL_VAR.spawnCount)
                {
                    currentSpawnCount = 0;
                    phase++;
                    phase = Mathf.Clamp(phase, 0, 2);
                }
            }
        }
    }

    /// <summary>
    /// 가장 하단에 있는 타겟 갱신
    /// </summary>
    void RefreshTarget()
    {
        if (targetQueue.Count > 0 && recentTarget == null)
            recentTarget = targetQueue.Dequeue().GetComponent<Target>();
    }

    /// <summary>
    /// 타겟을 놓치거나 파괴할때 호출
    /// </summary>
    /// <param name="index">루트 번호</param>
    /// <param name="type">0 : 놓침, 1 : 파괴(완전파괴)</param>
    /// <param name="grade">놓쳤을 경우 이 등급에 따라 체력이 감소</param>
    public void TargetLost(int index,int type,int grade)
    {
        recentTarget = null;
        isTargetExist[index] = false;
        switch(type)
        {
            case 0:     //놓침
                curHp -= grade;        
                break;
            case 1:     //부숨(완전파괴)
                break;
        }
    }

    /// <summary>
    /// 외곽 부쉈을때 점수처리
    /// </summary>
    public void TargetCrack(int index)
    {
        score += 10;
        coin += 10;
        AddCurSkillGauge(index, 1);
    }

    /// <summary>
    /// 버튼 클릭 시 호출
    /// 0 : 빨간색, 1 : 흰색, 2 : 파란색
    /// </summary>
    /// <param name="index"></param>
    public void ButtonTouch(int index=0)
    {
        if (recentTarget == null) return;
        recentTarget.SendMessage("ProcessBreak", index);
    }

    public float GetHpPercentage()
    {
        return curHp / maxHp;
    }

    public float GetCurSkillGauge(int index)
    {
        return curSkillGauge[index];
    }

    public void AddCurSkillGauge(int index,float amount)
    {
        curSkillGauge[index] += amount;
        Mathf.Clamp(curSkillGauge[index], 0, maxSkillGauge);
    }

    public void SubCurHp(float amount)
    {
        curHp -= amount;
    }

    void GameOverCheck()
    {
        if (curHp <= 0)
            SceneManager.LoadScene("Main");
    }

    public void SetDoubleSpeed(bool check)
    {
        if (check)
            gameSpeed = 2;
        else
            gameSpeed = 1;
    }

    public void Pause()
    {
        TimeManager.Instance.timeScale = 0;
    }

    public void Resume()
    {
        TimeManager.Instance.timeScale = 1;
    }

    public void GoHome()
    {
        SceneManager.LoadScene("Main");
    }

    public void Skill(int index)
    {
        if (curSkillGauge[index] < maxSkillGauge) return;
        curSkillGauge[index] = 0;
        recentTarget.Skill(index);
        for(int i=0;i<targetQueue.Count;i++)
        {
            GameObject temp = targetQueue.Dequeue();
            temp.GetComponent<Target>().Skill(index);
            targetQueue.Enqueue(temp);
        }
    }
}