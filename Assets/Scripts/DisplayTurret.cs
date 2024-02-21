using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTurret : MonoBehaviour
{
    public GameObject m_firePosition;
    public int m_bulletNum;
    public List<DisplayBullet> listBullet;
    public static Queue<DisplayBullet> queUsePool = new Queue<DisplayBullet>();
    public static Queue<DisplayBullet> queDisablePool = new Queue<DisplayBullet>();
    public int m_bulletCount = 0;

    public bool isFire = false;
    public bool isReloading = false;
    public bool isFull = false;

    void Start()
    {
        listBullet = new List<DisplayBullet>(m_bulletNum);
        GameObject prefab = Resources.Load("Prefabs/Bullet_turret") as GameObject;
        for (int i = 0; i < m_bulletNum; i++)
        {
            GameObject objBullet = Instantiate(prefab);
            objBullet.transform.position = this.transform.position;
            DisplayBullet cBullet = objBullet.GetComponent<DisplayBullet>();
            listBullet.Add(cBullet);
            queDisablePool.Enqueue(cBullet);
        }
    }

    void Update()
    {
        if(m_bulletCount > 10 && !isFull)
        {
            isFull = true;
            isReloading = true;
            Invoke("Reloading", 2f);
        }

        if (!isFire)
        {
            Shot();
        }

    }

    public void Shot()
    {
        if (!isReloading)
        {
            isFire = true;

            if (queDisablePool.Count > 0)
            {
                for(int i = 0;i < queDisablePool.Count; i++)
                {
                    m_bulletCount++;
                    if (i > 0)
                    {
                        Invoke("DelayFire", i * 0.1f);
                    }

                    if(i == 0)
                    {
                        DelayFire();
                    }
                    
                    if(m_bulletCount > 10)
                    {
                        break;
                    }
                }
            }

            Invoke("ChangeFireState", 0.1f);
        }
    }

    public void ChangeFireState()
    {
        isFire = false;
    }

    public void Reloading()
    {
        isFull = false;
        isReloading = false;
        m_bulletCount = 0;
    }

    public void DelayFire()
    {
        DisplayBullet cBullet = queDisablePool.Dequeue();
        queUsePool.Enqueue(cBullet);
        cBullet.transform.position = m_firePosition.transform.position;
        cBullet.Move();                
    }
}
