using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    bool hasTriggered = false;
    bool isCollision = false;

    private void Start()
    {
        Invoke("DelayDestroy", 5f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(!hasTriggered)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyHitBox"))
            {
                hasTriggered = true;
                Destroy(this.gameObject);
                if (collision.transform.root.GetComponent<EnemyControl>() != null)
                {
                    collision.transform.root.GetComponent<EnemyControl>().m_status.Demeged(1);
                } else if (collision.transform.root.GetComponent<EnemyControl_Wave>() != null)
                {
                    collision.transform.root.GetComponent<EnemyControl_Wave>().m_status.Demeged(1);
                }
                isCollision = true;
            }

            if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                hasTriggered = true;
                this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                Invoke("DelayDestroy", 1f);
                isCollision = true;
            }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        isCollision = false;
    }

    void DelayDestroy()
    {
        Destroy(this.gameObject);
    }
}
