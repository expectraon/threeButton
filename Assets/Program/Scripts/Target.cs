using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour {
    [System.NonSerialized]
    public float moveSpeed = 75; //이동속도
    private Transform trans;    //중복참조 방지용 포인터
    private int index = 0;      //타겟이 몇번 루트에 있는지 표시
    private Image circle;       //이미지 참조(임시)
    private Image[] childCircle;//각 외곽 이미지 참조
    private int grade = 1;      //외곽 갯수
	// Use this for initialization
	void Awake () {
        Init();
	}

    void OnEnable()
    {
        grade = Random.Range(1+GameManager.Instance.PhaseNumber, 3 + GameManager.Instance.PhaseNumber);
        ColorInit(grade);
    }

    // Update is called once per frame
    void Update () {
        trans.Translate(Vector2.down * moveSpeed * GameManager.Instance.GameSpeed * TimeManager.Instance.deltaTime);
	}

    /// <summary>
    /// 초기화용 함수
    /// </summary>
    void Init()
    {
        trans = transform;
        circle = GetComponent<Image>();
        childCircle = GetComponentsInChildren<Image>();
    }
    
    /// <summary>
    /// 색상초기화
    /// </summary>
    /// <param name="grade">몇겹의 원으로 구성되어 있는가? 1 ~ 4 </param>
    void ColorInit(int grade = 1)
    {
        if (childCircle == null) return;
        for(int i=0;i<childCircle.Length;i++)
        {
            childCircle[i].enabled = false;
        }
        Queue<int> colorQueue = new Queue<int>();
        Color[] colorArray = { Color.red, Color.white, Color.blue };
        for(int i=0;i<grade;i++)
        {
            int arrIndex = i * 2;
            colorQueue = ColorQueueMix();
            int temp = Random.Range(0, 2);//만약 0이면 한 외곽이 한가지 색만 가지고, 1이면 한 외곽이 두가지 색을 가진다.
            int fillOrigin = (i % 2 == 0) ? (int)Image.Origin360.Right : (int)Image.Origin360.Top;
            if(temp == 0)   //한 외곽이 한가지 색만 가질 경우
            {
                int colorIndex = colorQueue.Dequeue();
                childCircle[arrIndex].enabled = true;
                childCircle[arrIndex].color = colorArray[colorIndex];
                childCircle[arrIndex].fillAmount = 1.0f;
                childCircle[arrIndex].fillClockwise = true;
                childCircle[arrIndex].fillOrigin = fillOrigin;
                colorIndex = colorQueue.Dequeue();
                childCircle[arrIndex + 1].enabled = false;
                childCircle[arrIndex + 1].color = colorArray[colorIndex];
                childCircle[arrIndex + 1].fillAmount = 0;
                childCircle[arrIndex + 1].fillClockwise = false;
                childCircle[arrIndex + 1].fillOrigin = fillOrigin;
            }
            else if(temp == 1)  //한 외곽이 두가지 색을 가질 경우
            {
                int colorIndex = colorQueue.Dequeue();
                childCircle[arrIndex].enabled = true;
                childCircle[arrIndex].color = colorArray[colorIndex];
                childCircle[arrIndex].fillAmount = 0.5f;
                childCircle[arrIndex].fillClockwise = true;
                childCircle[arrIndex].fillOrigin = fillOrigin;
                colorIndex = colorQueue.Dequeue();
                childCircle[arrIndex + 1].enabled = true;
                childCircle[arrIndex + 1].color = colorArray[colorIndex];
                childCircle[arrIndex + 1].fillAmount = 0.5f;
                childCircle[arrIndex + 1].fillClockwise = false;
                childCircle[arrIndex + 1].fillOrigin = fillOrigin;
            }
        }
    }

    /// <summary>
    /// 타겟이 체력판정선에 닿았을 때 체력판정
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Gauge"))
        {
            GameManager.Instance.TargetLost(index,0,grade);
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 타겟이 체력판정선을 넘어가면 타겟 없앰(갑자기 없어지는거 방지)
    /// </summary>
    /// <param name="collision"></param>
    /*private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Gauge"))
        {
            gameObject.SetActive(false);
        }
    }*/

    /// <summary>
    /// 현재 타겟이 몇번 루트에 있는지 알려줌
    /// </summary>
    /// <param name="newIndex">루트 번호</param>
    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }
    
    /// <summary>
    /// 타겟 파괴 프로세스
    /// 버튼 측에서 SendMessage 형태로 호출
    /// </summary>
    /// <param name="index">색상코드 0 : 빨간색, 1 : 흰색, 2 : 파란색</param>
    void ProcessBreak(int index)
    {
        if (childCircle == null) return;
        Color[] colorArray = { Color.red, Color.white, Color.blue };
        
        if(childCircle[0].color == colorArray[index])
        {
            GameManager.Instance.TargetCrack(index);
            if(childCircle[1].enabled == true)  //가장 외곽의 왼쪽 색상이 일치하고 나머지 반대편이 있다
            {
                childCircle[0].enabled = false;
                childCircle[0].color = Color.gray;
                childCircle[1].fillAmount = 1;
            }
            else //하위 외곽 복사
            {
                ColorCopy();
            }
        }
        else if(childCircle[1].color==colorArray[index])
        {
            GameManager.Instance.TargetCrack(index);
            if (childCircle[0].enabled == true)  //가장 외곽의 오른쪽 색상이 일치하고 나머지 반대편이 있다
            {
                childCircle[0].fillAmount = 1;
                childCircle[1].enabled = false;
                childCircle[1].color = Color.gray;
            }
            else //하위 외곽 복사
            {
                ColorCopy();
            }
        }        
        else //일치하지 않는다.
        {
            //틀렸을 때 할 행동
            GameManager.Instance.SubCurHp(1);
        }
    }

    /// <summary>
    /// 하위 외곽의 색상과 방향정보를 상위 외곽으로 복사
    /// </summary>
    void ColorCopy()
    {
        for(int i=2;i<childCircle.Length;i++)
        {
            if (i >= grade * 2)
            {
                childCircle[i].enabled = false;
            }
            else
            {
                childCircle[i - 2].enabled = childCircle[i].enabled;
                childCircle[i - 2].color = childCircle[i].color;
                childCircle[i - 2].fillAmount = childCircle[i].fillAmount;
                childCircle[i - 2].fillClockwise = childCircle[i].fillClockwise;
                childCircle[i - 2].fillOrigin = childCircle[i].fillOrigin;
            }
        }

        if (grade > 1)
            grade--;
        else if (grade == 1)
        {
            gameObject.SetActive(false);
            GameManager.Instance.TargetLost(index,1,grade);
        }
        for(int i=2;i<childCircle.Length;i++)
        {
            if(i>=grade*2)
            {
                childCircle[i].enabled = false;
            }
        }
    }

    /// <summary>
    /// 한 외곽에 중복된 색깔이 나오지 않게 하기위해 큐에 색 하나씩 넣어놨다가 빼서 쓴다.
    /// </summary>
    /// <returns></returns>
    Queue<int> ColorQueueMix()
    {
        Queue<int> tempQueue = new Queue<int>();
        int[] tempArray = { 0, 1, 2 };
        for(int i=0;i<100;i++)
        {
            int source = Random.Range(0, tempArray.Length);
            int dest = Random.Range(0, tempArray.Length);
            int temp = tempArray[source];
            tempArray[source] = tempArray[dest];
            tempArray[dest] = temp;
        }
        for(int i=0;i<tempArray.Length;i++)
        {
            tempQueue.Enqueue(tempArray[i]);
        }
        return tempQueue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index">무슨 색을 없앨지</param>
    public void Skill(int index)
    {
        Color[] colorArray = { Color.red, Color.white, Color.blue };
        for(int i=0;i<childCircle.Length;i++)   //일치하는 색상 제거
        {
            if(childCircle[i].color == colorArray[index])   //현재 검사하는 원의 색이 스킬색과 같다면
            {
                //비활성화 절차
                childCircle[i].enabled = false;
                childCircle[i].fillAmount = 0;
                childCircle[i].color = Color.gray;
            }
        }

        for(int i=0;i<childCircle.Length;i++)   //빈자리가 생긴 인접 색깔로 메움
        {
            Image compareTarget;
            if(i%2==0)  //인덱스가 짝수일떄
            {
                compareTarget = childCircle[i + 1];
            }
            else        //인덱스가 홀수일때
            {
                compareTarget = childCircle[i - 1];
            }
            if(compareTarget.enabled == false && childCircle[i].enabled == true)
            {
                childCircle[i].fillAmount = 1.0f;
            }
        }

        for(int i=0;i<childCircle.Length;i++)   //색상 복사 절차
        {
            for(int j=2;j<childCircle.Length;j+=2)
            {
                if(childCircle[j-2].enabled == false && childCircle[j-1].enabled == false)  //검사 중인 외곽 바로 윗단계 외곽 없으면 복사 실행
                {
                    childCircle[j - 2].enabled = childCircle[j].enabled;
                    childCircle[j - 2].color = childCircle[j].color;
                    childCircle[j - 2].fillAmount = childCircle[j].fillAmount;
                    childCircle[j - 2].fillClockwise = childCircle[j].fillClockwise;
                    childCircle[j - 2].fillOrigin = childCircle[j].fillOrigin;

                    childCircle[j - 1].enabled = childCircle[j+1].enabled;
                    childCircle[j - 1].color = childCircle[j+1].color;
                    childCircle[j - 1].fillAmount = childCircle[j+1].fillAmount;
                    childCircle[j - 1].fillClockwise = childCircle[j+1].fillClockwise;
                    childCircle[j - 1].fillOrigin = childCircle[j+1].fillOrigin;

                    childCircle[j].enabled = false;
                    childCircle[j].color = Color.gray;
                    childCircle[j].fillAmount = 0;

                    childCircle[j+1].enabled = false;
                    childCircle[j+1].color = Color.gray;
                    childCircle[j+1].fillAmount = 0;
                }
            }
        }

        int gradeTemp = 0;
        for(int i=0;i<childCircle.Length;i+=2)
        {
            if (childCircle[i].enabled == false && childCircle[i + 1].enabled == false)
            {

            }
            else
                gradeTemp++;
        }
        grade = gradeTemp;
        if(grade == 0)
        {
            gameObject.SetActive(false);
            GameManager.Instance.TargetLost(index, 1, grade);
        }
    }
}