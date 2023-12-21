﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField]
    LayerMask m_sHitLayer;
    [SerializeField]
    float m_fRange = 1;
    bool isHit = false;

    private void FixedUpdate()
    {
        Vector3 vPos = this.transform.position;
        Vector3 vDir = -1 * transform.up;  // 뭔가 축이 좀 틀려진거 같으므로 다른데서 쓰려면 수정 필요
        Vector3 vEnd = vPos + vDir * m_fRange;

        Debug.DrawLine(vPos, vEnd, Color.green);

        RaycastHit raycastHit;
        bool isCheck = Physics.Raycast(vPos, vDir, out raycastHit, m_fRange, m_sHitLayer);

        if (isCheck)
        {
            if (!isHit)
            {
                raycastHit.collider.transform.parent.GetComponent<PlayerControl>().m_status.Demeged(1);
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