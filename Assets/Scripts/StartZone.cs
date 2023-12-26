using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    public GameObject m_body;
    public GameObject m_startzone;
    public GameObject virtualCamera;

    GameObject m_player; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_player != null && Input.GetKeyDown(KeyCode.A))
        {
            m_body.SetActive(true);

            m_startzone.SetActive(false);
            m_player.SetActive(false);
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
