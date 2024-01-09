using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RPGSetting;

public class GUIPanel : MonoBehaviour
{
    [SerializeField] Image m_itemImage;
    [SerializeField] TextMeshProUGUI m_textComment;

    public void SetPanelInfo(Item item)
    {
        //if (m_itemImage.sprite)
        //{
        //    Destroy(m_itemImage.sprite);
        //}

        m_itemImage.sprite = Resources.Load<Sprite>("Images/ItemImage/" + item.icon);
        m_textComment.text = item.explain;
    }
}
