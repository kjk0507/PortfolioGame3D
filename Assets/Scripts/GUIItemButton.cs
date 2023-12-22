using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RPGSetting;

public class GUIItemButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_textName;
    [SerializeField] Button m_btnButton;

    public void SetItemInfo(GUIPanel guiPanel, Item item)
    {
        m_textName.text = item.name;
        m_btnButton.onClick.AddListener(() => { guiPanel.SetPanelInfo(item); });
    }
}
