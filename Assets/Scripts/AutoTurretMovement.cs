using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTurretMovement : MonoBehaviour
{
    public string enemyTag = "Enemy"; // 태그를 적절한 값으로 변경하세요.
    public float rotationSpeed = 5f;
    public Transform m_transform;

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
        //Debug.Log("x : " + targetPosition.x + "/ y : " + targetPosition.y + "/ z : " + targetPosition.z);

        //float tempX = targetPosition.x;
        //float tempY = targetPosition.y;
        //float tempZ = targetPosition.z;

        //targetPosition.x = tempZ;
        //targetPosition.y = tempX;
        //targetPosition.z = tempY;

        //float tempX2 = m_transform.position.x;
        //float tempY2 = m_transform.position.y;
        //float tempZ2 = m_transform.position.z;



        Vector3 direction = targetPosition - m_transform.position;

        //direction.y = 0;

        // 타겟의 y 값 만큼 x가 움직, 타켓의 z 값만큼 y가 움직임, 타겟의 x 만큼 z가 움직임

        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}
