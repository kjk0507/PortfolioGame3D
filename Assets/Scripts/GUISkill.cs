using RPGSetting;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RPGSetting;

public class GUISkill : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> skillInfoList = new List<TextMeshProUGUI>();
    public Image icon;
    public enum E_SKILL_INFO { NAME, TYPE, DROP, EXPLAIN, COOLDOWN, Require, ICON }

    public void SettingSkill(Skill skill)
    {
        skillInfoList[(int)E_SKILL_INFO.NAME].text = skill.name;
        skillInfoList[(int)E_SKILL_INFO.TYPE].text = "분류 : " + (skill.type == "Active" ? "액티브" : "패시브") + (skill.modeType == "both" ? "(요새 겸용)" : "");
        skillInfoList[(int)E_SKILL_INFO.DROP].text = "체력 : " + skill.dropLocation;
        skillInfoList[(int)E_SKILL_INFO.EXPLAIN].text = "요세 추가 체력 : " + skill.explain;
        skillInfoList[(int)E_SKILL_INFO.COOLDOWN].text = "쿨타임 : " + skill.coolDown;
        skillInfoList[(int)E_SKILL_INFO.Require].text = "제작조건 : " + skill.Require;
        //icon.sprite = Resources.Load<Sprite>("Images/RPG_inventory_icons/" + skill.icon);
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
}
