using RPGSetting;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIItemSell : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> itemInfoList = new List<TextMeshProUGUI>();
    public Image icon;
    public Button m_buyButton;
    public enum E_ITEM_INFO { NAME, TYPE, DROP, EXPLAIN, NUM, PRICE, ICON, BUTTON }

    public void SettingItem(Item item)
    {
        itemInfoList[(int)E_ITEM_INFO.NAME].text = item.name;
        itemInfoList[(int)E_ITEM_INFO.TYPE].text = "분류 : " + (item.type == "consum" ? "소비품" : (item.type == "equip" ? "장비품" : "기타"));
        itemInfoList[(int)E_ITEM_INFO.DROP].text = "획득처 : " + item.dropLocation;
        itemInfoList[(int)E_ITEM_INFO.EXPLAIN].text = "설명 : " + item.explain;
        itemInfoList[(int)E_ITEM_INFO.NUM].text = "보유 수량 : " + item.num;
        itemInfoList[(int)E_ITEM_INFO.PRICE].text = "판매가격 : " + item.money;
        icon.sprite = Resources.Load<Sprite>("Images/ItemImage/" + item.icon);
        m_buyButton.onClick.AddListener(() => { TrytoSellItem(item); });
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

    public void SetItem(Status player)
    {
        Object prefabItemInfo = Resources.Load("Prefabs/SellItemInfo");
        foreach (Item item in player.inventory)
        {
            GameObject objItemInfo = Instantiate(prefabItemInfo, m_gridContent.transform) as GameObject;
            GUIItemSell guiItemInfo = objItemInfo.GetComponent<GUIItemSell>();
            guiItemInfo.SettingItem(item);
            m_guiItemList.Add(guiItemInfo);
        }
        //Destroy(prefabItemButton);
        //SetConnentSize();
    }

    public void ResetItemList()
    {
        foreach (var item in m_guiItemList)
        {
            Destroy(item.gameObject);
        }
        m_guiItemList.Clear();
    }

    public void TrytoSellItem(Item item)
    {
        ChangeItemNum(item);
        GameManager.m_cInstance.playerStatus.money += item.money;
        GameManager.m_cInstance.SetComment(item.name + "을(를) " + item.money +"골드를 받고 판매했습니다.");
        //GameManager.m_cInstance.RestoreStore();
    }

    public void ChangeItemNum(Item item)
    {
        for (int i = 0; i < GameManager.m_cInstance.playerStatus.inventory.Count; i++)
        {
            if (GameManager.m_cInstance.playerStatus.inventory[i].itemCode == item.itemCode)
            {
                GameManager.m_cInstance.playerStatus.inventory[i].num--;
                if(GameManager.m_cInstance.playerStatus.inventory[i].num == 0)
                {
                    GameManager.m_cInstance.playerStatus.inventory.RemoveAt(i);
                }

                GameManager.m_cInstance.RestoreStore();

                break;
            }
        }
    }
}
