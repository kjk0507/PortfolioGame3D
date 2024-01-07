using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortressStore : MonoBehaviour
{
    public GameObject m_player;
    public bool isStore = false;

    private void Update()
    {
        if (m_player != null)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                GameManager.m_cInstance.VisitStore();
                DontMove();
                isStore = true;
            }

            if (Input.GetKeyDown(KeyCode.Escape))  //  || (Input.GetKeyDown(KeyCode.A) && isStore)
            {
                GameManager.m_cInstance.LeaveStore();
                CanMove();
                isStore = false;
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
