using RPGSetting;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIStoreButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_textName;
    [SerializeField] Button m_btnButton;
    [SerializeField] Button m_buyButton;

    public void SetItemInfo(ItemManager itemManager, GridLayoutGroup GUIposition)
    {
        m_textName.text = "아이템";
        m_btnButton.onClick.AddListener(() => { SetItem(itemManager, GUIposition); });
    }

    public void SetSkillInfo(Status player, GridLayoutGroup GUIposition)
    {
        m_textName.text = "스킬";
        m_btnButton.onClick.AddListener(() => { SetSkill(player, GUIposition); });
    }

    public void SetSkill(Status player, GridLayoutGroup GUIposition)
    {
        Object prefabSkillInfo = Resources.Load("Prefabs/BuySkillInfo");

        foreach (Transform child in GUIposition.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Skill skill in player.skillList)
        {
            GameObject objSkillInfo = Instantiate(prefabSkillInfo, GUIposition.transform) as GameObject;
            GUISkill guiSkillInfo = objSkillInfo.GetComponent<GUISkill>();
            guiSkillInfo.SettingSkill(skill);
        }
    }

    public void SetItem(ItemManager itemManager, GridLayoutGroup GUIposition)
    {
        Object prefabItemInfo = Resources.Load("Prefabs/BuyItemInfo");

        foreach (Transform child in GUIposition.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in itemManager.m_allItemList)
        {
            GameObject objItemInfo = Instantiate(prefabItemInfo, GUIposition.transform) as GameObject;
            GUIItemBuy guiItemInfo = objItemInfo.GetComponent<GUIItemBuy>();
            guiItemInfo.SettingItem(item);
        }
    }

}
