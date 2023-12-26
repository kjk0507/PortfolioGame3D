using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public Collider m_collider;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(PassColliders());
    }

    IEnumerator PassColliders()
    {
        Collider[] playerColliders = Physics.OverlapSphere(transform.position, Mathf.Infinity, LayerMask.GetMask("PlayerHitBox"));

        foreach (Collider player in playerColliders)
        {
            if (player != null)
            {
                player.transform.root.GetComponent<PlayerControl>().isPlatform = true;
                Physics.IgnoreCollision(m_collider, player, true);
            }
        }

        yield return new WaitForSeconds(0.5f);

        foreach (Collider player in playerColliders)
        {
            if (player != null)
            {
                Debug.Log("exit");
                Physics.IgnoreCollision(m_collider, player, false);
            }
        }
    }
}
