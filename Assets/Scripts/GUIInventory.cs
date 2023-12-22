using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGSetting;
using UnityEngine.UI;

public class GUIInventory : MonoBehaviour
{
    [SerializeField] List<GUIItemButton> m_guiButtonList = new List<GUIItemButton>();
    [SerializeField] GUIPanel m_guiPanel; // 아이콘과 제목, 설명 표시 ui
    [SerializeField] GridLayoutGroup m_gridContent; // 뷰포인트 밑에 버튼이 생겨날 위치

    public void SetConnentSize()
    {
        float height = m_guiButtonList.Count * m_gridContent.cellSize.y;
        RectTransform rectTransform = m_gridContent.GetComponent<RectTransform>();
        //rectTransform.sizeDelta.y = height;
        Vector2 vSize = rectTransform.sizeDelta;
        vSize.y = height;
        rectTransform.sizeDelta = vSize;
    }

    public void SetIventory(Status player)
    {
        Object prefabItemButton = Resources.Load("Prefabs/ItemButton");
        foreach (var item in player.inventory)
        {
            GameObject objButton = Instantiate(prefabItemButton, m_gridContent.transform) as GameObject;
            GUIItemButton guiButton = objButton.GetComponent<GUIItemButton>();
            guiButton.SetItemInfo(m_guiPanel, item);
            m_guiButtonList.Add(guiButton);
        }
        //Destroy(prefabItemButton);
        //SetConnentSize();
    }

    public void ResetIventoryButton()
    {
        foreach (var item in m_guiButtonList)
        {
            Destroy(item.gameObject);
        }
        m_guiButtonList.Clear();
    }

}
