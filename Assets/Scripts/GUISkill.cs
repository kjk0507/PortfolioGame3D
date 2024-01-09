using RPGSetting;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUISkill : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> skillInfoList = new List<TextMeshProUGUI>();
    public Image icon;
    public TextMeshProUGUI skillName;
    public Button m_buyButton;
    public enum E_SKILL_INFO { NAME, TYPE, DROP, EXPLAIN, COOLDOWN, REQUIRE, PRESSKEY, ICON, BUTTON }

    public void SettingSkill(Skill skill)
    {
        skillInfoList[(int)E_SKILL_INFO.NAME].text = skill.name + (skill.isHave ? " (습득)" : " (미습득)");
        skillInfoList[(int)E_SKILL_INFO.TYPE].text = "분류 : " + (skill.type == "Active" ? "액티브" : "패시브") + (skill.modeType == "both" ? "(요새 겸용)" : "");
        skillInfoList[(int)E_SKILL_INFO.DROP].text = "획득처 : " + skill.dropLocation;
        skillInfoList[(int)E_SKILL_INFO.EXPLAIN].text = "설명 : " + skill.explain;
        skillInfoList[(int)E_SKILL_INFO.COOLDOWN].text = "쿨타임 : " + skill.coolDown;
        skillInfoList[(int)E_SKILL_INFO.REQUIRE].text = "제작조건 : " + skill.Require + " + " + skill.money + " 골드";
        skillInfoList[(int)E_SKILL_INFO.PRESSKEY].text = "단축키 : " + skill.pressKey;
        icon.sprite = Resources.Load<Sprite>("Images/Skillmage/" + skill.icon);
        if(m_buyButton != null)
        {
            if (skill.isHave)
            {
                m_buyButton.gameObject.SetActive(false);
            }
            else
            {
                m_buyButton.onClick.AddListener(() => { TrytoBuySkill(skill); });
            }
        }
    }

    [SerializeField] GridLayoutGroup m_gridContent;
    [SerializeField] List<GUISkill> m_guiSkillList = new List<GUISkill>();
    public void SetConnentSize()
    {
        float height = m_guiSkillList.Count * m_gridContent.cellSize.y;
        RectTransform rectTransform = m_gridContent.GetComponent<RectTransform>();
        //rectTransform.sizeDelta.y = height;
        Vector2 vSize = rectTransform.sizeDelta;
        vSize.y = height;
        rectTransform.sizeDelta = vSize;
    }

    public void SetSkill(Status player)
    {
        Object prefabSkillInfo = Resources.Load("Prefabs/SkillInfo");
        foreach (Skill skill in player.skillList)
        {
            GameObject objSkillInfo = Instantiate(prefabSkillInfo, m_gridContent.transform) as GameObject;
            GUISkill guiSkillInfo = objSkillInfo.GetComponent<GUISkill>();
            guiSkillInfo.SettingSkill(skill);
        }
        //Destroy(prefabItemButton);
        //SetConnentSize();
    }

    public void ResetSkillList()
    {
        foreach (var skill in m_guiSkillList)
        {
            Destroy(skill.gameObject);
        }
        m_guiSkillList.Clear();
    }

    // 스킬 구매
    public void TrytoBuySkill(Skill skill)
    {
        if(skill.money <= GameManager.m_cInstance.playerStatus.money)
        {
            ChangeIsHave(skill);
            m_buyButton.gameObject.SetActive(false);
            skillName.text = skill.name + (skill.isHave ? " (습득)" : " (미습득)");
            GameManager.m_cInstance.playerStatus.money -= skill.money;
            GameManager.m_cInstance.SetComment("구매했습니다.");
        }
        else
        {
            GameManager.m_cInstance.SetComment("돈이 부족합니다.");
        }

    }

    public void ChangeIsHave(Skill skill)
    {
        for(int i = 0;  i < GameManager.m_cInstance.playerStatus.skillList.Count; i++)
        {
            if (GameManager.m_cInstance.playerStatus.skillList[i].skillCode == skill.skillCode)
            {
                GameManager.m_cInstance.playerStatus.skillList[i].isHave = true;
                break;
            }
        }
    }
}
