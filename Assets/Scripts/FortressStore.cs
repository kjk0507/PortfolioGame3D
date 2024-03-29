using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FortressStore : MonoBehaviour
{
    public GameObject m_player;
    public bool isStore = false;
    bool isPress = false;
    bool isExit = false;

    private void Update()
    {
        if (m_player != null)
        {
            if (Input.GetKeyDown(KeyCode.A) || isPress)
            {
                //GameManager.m_cInstance.EventChageScene(5);
                GameManager.m_cInstance.VisitStore();
                GameManager.m_cInstance.HideControler();
                DontMove();
                isStore = true;
                isPress = false;
            }

            if (Input.GetKeyDown(KeyCode.Escape) || isExit)  //  || (Input.GetKeyDown(KeyCode.A) && isStore)
            {
                isExit = false;
                GameManager.m_cInstance.LeaveStore();
                GameManager.m_cInstance.ChangePlayerControler();
                CanMove();
                isStore = false;
                m_player.GetComponent<PlayerControl>().UpAttack();
            }
        }
    }

    public void DontMove()
    {
        m_player.GetComponent<PlayerControl>().isStop = true;
    }

    public void CanMove()
    {
        m_player.GetComponent<PlayerControl>().isStop = false;
    }

    public void PressGUIButton()
    {
        if (m_player != null)
        {
            isPress = true;
        }
    }
    public void ExitGUIButton()
    {
        isExit = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerHitBox"))
        {
            m_player = collision.transform.root.gameObject;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerHitBox"))
        {
            m_player = null;
        }
    }
}
