using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyWeapon : MonoBehaviour
{
    public string m_tag = "PlayerHitBox";
    float m_fRange = 2;
    bool isHit = false;

    private void FixedUpdate()
    {
        Vector3 vPos = this.transform.position;
        Vector3 vDir = -1 * transform.up;  // 뭔가 축이 좀 틀어진거 같으므로 다른데서 쓰려면 수정 필요
        Vector3 vEnd = vPos + vDir * m_fRange;

        Debug.DrawLine(vPos, vEnd, Color.green);

        RaycastHit raycastHit;
        //bool isCheck = Physics.Raycast(vPos, vDir, out raycastHit, m_fRange, m_sHitLayer);
        bool isCheck = Physics.Raycast(vPos, vDir, out raycastHit, m_fRange) && raycastHit.collider.CompareTag(m_tag);



        if (isCheck)
        {
            if (!isHit)
            {
                //if(raycastHit.collider.transform.root.GetComponent<PlayerControl>() != null)
                //{
                //    raycastHit.collider.transform.root.GetComponent<PlayerControl>().m_status.Demeged(1);
                //} else if (raycastHit.collider.transform.root.GetComponent<FortressControl>() != null)
                //{
                //    raycastHit.collider.transform.root.GetComponent<FortressControl>().m_status.Demeged(1);
                //}


                GameManager.m_cInstance.playerStatus.Demeged(1);

                isHit = true;

                Invoke("CheckHitTime", 1f);
            }

            Debug.DrawLine(vPos, vEnd, Color.green);
        }
        else
        {
            Debug.DrawLine(vPos, vEnd, Color.red);
        }
    }

    void CheckHitTime()
    {
        isHit = false;
    }
}
