using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTurretMovement : MonoBehaviour
{
    public string enemyTag = "EnemyHitBox"; 
    public float rotationSpeed = 5f;
    public Transform m_transform;
    bool isShot = false;
    public GameObject bullet;
    public GameObject shotPosition;

    void Update()
    {
        GameObject nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            RotateTowards(nearestEnemy.transform.position);
        }
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        GameObject nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    void RotateTowards(Vector3 targetPosition)
    {
        //targetPosition.y = targetPosition.y + 1f;
        Vector3 direction = targetPosition - m_transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        float distance = Vector3.Distance(m_transform.position, targetPosition);
        if (distance < 20)
        {
            ShotBullet();
        }
    }

    void ShotBullet()
    {
        if (!isShot)
        {
            isShot = true;

            for (int i = 0; i < 4; i++)
            {
                Invoke("FireBullet", i * 0.1f);
            }

            Invoke("ResetShotFlag", 3f);
        }
    }

    void FireBullet()
    {
        //Quaternion rotation = Quaternion.identity;
        //Quaternion rotation = Quaternion.Euler(90f, 0f, 0f) * bullet.transform.rotation; ;
        Quaternion rotation = Quaternion.Euler(0f, 0f, -90f);
        GameObject objArrow = Instantiate(bullet, shotPosition.transform.position, rotation);
        //Vector3 direction = (shotPosition.transform.position - m_transform.position).normalized;
        Vector3 direction = m_transform.forward.normalized;
        //Vector3 direction = m_transform.right.normalized;
        //Vector3 direction = m_transform.up.normalized;
        objArrow.GetComponent<Rigidbody>().velocity = direction * 20f;
    }

    void ResetShotFlag()
    {
        isShot = false;
    }


}
