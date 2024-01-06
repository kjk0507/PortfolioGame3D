using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class PopupButton : MonoBehaviour
{
    List<GameObject> popupList = new List<GameObject>();

    public enum E_Button_Type
    {
        statusGUI,
        InventoryGUI,
        SkillGUI
    }

    public E_Button_Type e_button_type;

    public void ChangePopupActive()
    {
        popupList = GameManager.m_cInstance.popupGUIList;
        for (int i = 0; i < popupList.Count; i++)
        {
            if ((E_Button_Type)i == e_button_type)
                popupList[i].SetActive(true);
            else
                popupList[i].SetActive(false);
        }
    }
}
