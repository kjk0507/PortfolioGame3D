using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreButton : MonoBehaviour
{
    List<GameObject> StoreList = new List<GameObject>();

    public enum E_Button_Type
    {
        BuyGUI,
        SellGUI
    }

    public E_Button_Type e_button_type;

    public void ChangeStoreActive()
    {
        StoreList = GameManager.m_cInstance.StoreGUIList;
        for (int i = 0; i < StoreList.Count; i++)
        {
            if ((E_Button_Type)i == e_button_type)
                StoreList[i].SetActive(true);
            else
                StoreList[i].SetActive(false);
        }

        GameManager.m_cInstance.RestoreStore();
    }
}
