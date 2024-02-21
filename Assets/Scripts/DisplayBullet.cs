using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBullet : MonoBehaviour
{
    public float m_speed = 20;
    public bool isMove = false;
    public float m_dist = 20;
    float m_curDist = 0;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isMove)
        {
            float fMove = m_speed * Time.deltaTime;
            transform.position += transform.up * fMove;
            m_curDist += fMove;
        }

        if(m_curDist >= m_dist)
        {
            Stop();
            if (DisplayTurret.queUsePool.Count > 0)
            {
                DisplayBullet bullet = DisplayTurret.queUsePool.Dequeue();
                DisplayTurret.queDisablePool.Enqueue(bullet);
            }
        }
    }

    public void Move()
    {
        isMove = true;
        m_curDist = 0;
        gameObject.SetActive(true);
    }

    public void Stop()
    {
        isMove = false;
        m_curDist = 0;
        gameObject.SetActive(false);
    }
}
