using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortressHitBox : MonoBehaviour
{
    public FortressControl m_fortressControl;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerHitBox"))
        {
            m_fortressControl.m_player = collision.transform.root.gameObject;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerHitBox"))
        {
            m_fortressControl.m_player = null;
        }
    }
}
