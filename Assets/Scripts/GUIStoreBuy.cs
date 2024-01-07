using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPGSetting;
using UnityEditor.Experimental.GraphView;

public class GUIStoreBuy : MonoBehaviour
{
    [SerializeField] List<GUIStoreButton> m_guiButtonList = new List<GUIStoreButton>();
    [SerializeField] GUIPanel m_guiPanel; // 아이콘과 제목, 설명 표시 ui
    [SerializeField] GUISkill m_guiSkill;
    [SerializeField] GridLayoutGroup m_gridContent;     // 버튼부분(왼쪽)
    [SerializeField] GridLayoutGroup m_gridInfoContent; // 설명부분(오른쪽)

    public void SetConnentSize()
    {
        float height = m_guiButtonList.Count * m_gridContent.cellSize.y;
        RectTransform rectTransform = m_gridContent.GetComponent<RectTransform>();
        //rectTransform.sizeDelta.y = height;
        Vector2 vSize = rectTransform.sizeDelta;
        vSize.y = height;
        rectTransform.sizeDelta = vSize;
    }

    public void SetItemStore(ItemManager itemManager)
    {
        Object prefabItemButton = Resources.Load("Prefabs/StoreButton");
        GameObject objButton = Instantiate(prefabItemButton, m_gridContent.transform) as GameObject;
        GUIStoreButton guiButton = objButton.GetComponent<GUIStoreButton>();
        guiButton.SetItemInfo(itemManager, m_gridInfoContent);
        m_guiButtonList.Add(guiButton);
    }
    public void SetSkillStore(Status player)
    {
        // 여기서 정보를 넘겨줘야 뒤에서 작업 가능
        Object prefabItemButton = Resources.Load("Prefabs/StoreButton");
        GameObject objButton = Instantiate(prefabItemButton, m_gridContent.transform) as GameObject;
        GUIStoreButton guiButton = objButton.GetComponent<GUIStoreButton>();
        guiButton.SetSkillInfo(player, m_gridInfoContent);
        m_guiButtonList.Add(guiButton);
    }

    public void ResetStoreButton()
    {
        foreach (var item in m_guiButtonList)
        {
            Destroy(item.gameObject);
        }
        m_guiButtonList.Clear();
    }

    public void ResetStoreInfo()
    {
        for (int i = m_gridInfoContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(m_gridInfoContent.transform.GetChild(i).gameObject);
        }
    }
}
