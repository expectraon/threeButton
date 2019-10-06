using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    public Image curSkillGauge;
    public Image feverIcon;
    public int colorIndex = 0;
    private Vector3 ButtonPushStart = Vector3.zero;
    private Vector3 ButtonPushEnd = Vector3.zero;
    private float skillActiveLength = 80.0f; //이 거리 이상을 움직여야 스킬 발동
                                            // Update is called once per frame
    void Update()
    {
        UpdateSkillGauge();
    }

    void UpdateSkillGauge()
    {
        float percentage = GameManager.Instance.GetCurSkillGauge(colorIndex) / GameManager.Instance.MaxSkillGauge;
        curSkillGauge.fillAmount = percentage;
        if (percentage > 0.9f)
        {
            feverIcon.enabled = true;
        }
        else
            feverIcon.enabled = false;
    }

    public void SkillDetectStart(int index)
    {
        ButtonPushStart = Input.mousePosition;
    }

    public void SkillDetectEnd(int index)
    {
        if (feverIcon.enabled == false)
        {
            GameManager.Instance.ButtonTouch(colorIndex);
        }
        else
        {
            ButtonPushEnd = Input.mousePosition;
            Vector3 moveVec = ButtonPushEnd - ButtonPushStart;
            Debug.Log(moveVec.magnitude);
            if (moveVec.y > 0 && (moveVec.magnitude > skillActiveLength))
            {
                Skill();
                Debug.Log("SkillActivate " + index);
            }
            else
                GameManager.Instance.ButtonTouch(colorIndex);
        }
    }

    void Skill()
    {
        GameManager.Instance.Skill(colorIndex);
    }
}
