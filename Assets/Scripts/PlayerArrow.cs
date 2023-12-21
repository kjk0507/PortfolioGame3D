using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    private void Start()
    {
        Invoke("DelayDestroy", 5f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("ishit?");
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyHitBox"))
        {
            Destroy(this.gameObject);
            if (collision.transform.parent.GetComponent<EnemyControl>() != null)
            {
                //Debug.Log("hit");
                collision.transform.parent.GetComponent<EnemyControl>().m_status.Demeged(1);
            }
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Invoke("DelayDestroy", 1f);
        }
    }

    void DelayDestroy()
    {
        Destroy(this.gameObject);
    }
}
