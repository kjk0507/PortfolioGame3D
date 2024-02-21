using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MiniGameControl : MonoBehaviour
{
    public GameObject m_player;
    public bool isMiniGame = false;
    bool isPress = false;
    bool isExit = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_player != null)
        {
            if (Input.GetKeyDown(KeyCode.A) || isPress)
            {                
                GameManager.m_cInstance.PlayMiniGame();
                GameManager.m_cInstance.HideControler();
                isPress = false;
                DontMove();
                isMiniGame = true;
            }

            if (Input.GetKeyDown(KeyCode.Escape) || isExit)  //  || (Input.GetKeyDown(KeyCode.A) && isStore)
            {
                isExit = false;
                GameManager.m_cInstance.LeaveMiniGame();
                GameManager.m_cInstance.ChangePlayerControler();
                CanMove();
                isMiniGame = false;
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
        if (m_player != null)
        {
            isExit = true;
        }        
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
