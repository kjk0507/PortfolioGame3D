using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RPGSetting;
using UnityEditor.Experimental.GraphView;

public class GUIItemBuy : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> itemInfoList = new List<TextMeshProUGUI>();
    public Image icon;
    public TextMeshProUGUI num;
    public Button m_buyButton;
    public enum E_ITEM_INFO { NAME, TYPE, DROP, EXPLAIN, NUM, PRICE, ICON, BUTTON }

    public void SettingItem(Item item)
    {
        itemInfoList[(int)E_ITEM_INFO.NAME].text = item.name;
        itemInfoList[(int)E_ITEM_INFO.TYPE].text = "분류 : " + (item.type == "consum" ? "소비품" : (item.type == "equip" ? "장비품" : "기타"));
        itemInfoList[(int)E_ITEM_INFO.DROP].text = "획득처 : " + item.dropLocation;
        itemInfoList[(int)E_ITEM_INFO.EXPLAIN].text = "설명 : " + item.explain;
        itemInfoList[(int)E_ITEM_INFO.NUM].text = "보유 수량 : " + FindItemNum(item);
        itemInfoList[(int)E_ITEM_INFO.PRICE].text = "구매가격 : " + item.money;
        //icon.sprite = Resources.Load<Sprite>("Images/RPG_inventory_icons/" + skill.icon);
        m_buyButton.onClick.AddListener(() => { TrytoBuyItem(item); });
    }

    [SerializeField] GridLayoutGroup m_gridContent;
    [SerializeField] List<GUIItemSell> m_guiItemList = new List<GUIItemSell>();
    public void SetConnentSize()
    {
        float height = m_guiItemList.Count * m_gridContent.cellSize.y;
        RectTransform rectTransform = m_gridContent.GetComponent<RectTransform>();
        //rectTransform.sizeDelta.y = height;
        Vector2 vSize = rectTransform.sizeDelta;
        vSize.y = height;
        rectTransform.sizeDelta = vSize;
    }

    //public void SetItem(Status player)
    //{
    //    Object prefabItemInfo = Resources.Load("Prefabs/BuyItemInfo");
    //    foreach (Item item in player.inventory)
    //    {
    //        GameObject objItemInfo = Instantiate(prefabItemInfo, m_gridContent.transform) as GameObject;
    //        GUIItemSell guiItemInfo = objItemInfo.GetComponent<GUIItemSell>();
    //        guiItemInfo.SettingItem(item);
    //        m_guiItemList.Add(guiItemInfo);
    //    }
    //    //Destroy(prefabItemButton);
    //    //SetConnentSize();
    //}

    //public void ResetItemList()
    //{
    //    foreach (var item in m_guiItemList)
    //    {
    //        Destroy(item.gameObject);
    //    }
    //    m_guiItemList.Clear();
    //}

    public void TrytoBuyItem(Item item)
    {
        if (item.money < GameManager.m_cInstance.playerStatus.money)
        {
            ChangeItemNum(item);
            //m_buyButton.gameObject.SetActive(false);
            GameManager.m_cInstance.playerStatus.money -= item.money;
            GameManager.m_cInstance.SetComment("구매했습니다.");
            num.text = "보유 수량 : " + FindItemNum(item);
        }
        else
        {
            GameManager.m_cInstance.SetComment("돈이 부족합니다.");
        }

        //GameManager.m_cInstance.RestoreStore();
    }

    public void ChangeItemNum(Item item)
    {
        bool findItem = false;
        for (int i = 0; i < GameManager.m_cInstance.playerStatus.inventory.Count; i++)
        {
            if (GameManager.m_cInstance.playerStatus.inventory[i].itemCode == item.itemCode)
            {
                GameManager.m_cInstance.playerStatus.inventory[i].num++;
                findItem = true;
                break;
            }
        }

        if (!findItem)
        {
            GameManager.m_cInstance.playerStatus.inventory.Add(item);
        }
    }

    public int FindItemNum(Item item)
    {
        int num = 0;

        for (int i = 0; i < GameManager.m_cInstance.playerStatus.inventory.Count; i++)
        {
            if (GameManager.m_cInstance.playerStatus.inventory[i].itemCode == item.itemCode)
            {
                num = GameManager.m_cInstance.playerStatus.inventory[i].num;
                break;
            }
        }

        return num;
    }
}
