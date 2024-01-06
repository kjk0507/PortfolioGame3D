using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGSetting;
using static EnemyControl;

public class EnemyControl_Wave : MonoBehaviour
{
    string fortressTag = "FortressHitBox";
    Transform m_transform;
    Rigidbody m_rigidbody;
    Animator m_animator;

    public GameObject m_enemyHitBox;

    public Status m_status = new Status();

    public GameObject nearestFortress;

    bool isAttack = false;
    bool isDeath = false;

    public E_Enemy_Type e_enemyType;
    public E_AI_STATUS e_status = E_AI_STATUS.STANDING;

    public enum E_Enemy_Type
    {
        Skeleton,
        Dragon
    }
    public enum E_AI_STATUS
    {
        STANDING,  // 기본 동작(바로 달려감)
        MOVE,      // 요새방향으로 달리기
        ATTACK,    // 요새공격
        SKILL,     // 스킬 발동
        DEATH      // 죽는 모션
    }

    void Start()
    {
        m_transform = GetComponent<Transform>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        //nearestFortress = FindNearestFortress();  // 이거 가까운거보다 변수로 빼두고 부여하는 방식은 어떨까? 복사될때 부여가 가능할까?
        LookFortress(nearestFortress);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_status.IsDeath())
        {
            e_status = E_AI_STATUS.DEATH;
        }

        m_animator.SetInteger("e_status", (int)e_status);
    }

    private void FixedUpdate()
    {
        switch (e_status)
        {
            case E_AI_STATUS.STANDING:
                ProcessStanding(e_enemyType);
                break;
            case E_AI_STATUS.MOVE:
                ProcessMove(e_enemyType);
                break;
            case E_AI_STATUS.ATTACK:
                ProcessAttack(e_enemyType);
                break;
            case E_AI_STATUS.SKILL:
                ProcessSkill(e_enemyType);
                break;
            case E_AI_STATUS.DEATH:
                ProcessDeath(e_enemyType);
                break;
        }
    }

    void ProcessStanding(E_Enemy_Type e_enemyType)
    {
        if (e_enemyType == E_Enemy_Type.Skeleton)
        {
            float distance = Vector3.Distance(m_transform.position, nearestFortress.transform.position);
            //e_status = (distance > 3) ? E_AI_STATUS.MOVE : E_AI_STATUS.ATTACK;
            e_status = E_AI_STATUS.MOVE;
        }

        if(e_enemyType == E_Enemy_Type.Dragon)
        {
            float distance = Vector3.Distance(m_transform.position, nearestFortress.transform.position);
            e_status = E_AI_STATUS.MOVE;
        }
    }

    void ProcessMove(E_Enemy_Type e_enemyType)
    {
        if (e_enemyType == E_Enemy_Type.Skeleton)
        {
            //LookFortress(nearestFortress);

            Vector3 direction = new Vector3(m_transform.localScale.z, 0, 0);
            direction.Normalize();
            float distance = Vector3.Distance(m_transform.position, nearestFortress.transform.position);

            if (distance > 4)
            {
                m_rigidbody.velocity = new Vector3(direction.x * 5f, m_rigidbody.velocity.y, 0);
            }
            else
            {
                isAttack = true;
                e_status = E_AI_STATUS.ATTACK;
            }
        }

        if (e_enemyType == E_Enemy_Type.Dragon)
        {
            Vector3 direction = new Vector3(m_transform.localScale.z, 0, 0);
            direction.Normalize();
            float distance = Vector3.Distance(m_transform.position, nearestFortress.transform.position);

            if (distance > 10)
            {
                m_rigidbody.velocity = new Vector3(direction.x * 5f, m_rigidbody.velocity.y, 0);
            }
            else
            {
                isAttack = true;
                e_status = E_AI_STATUS.ATTACK;
            }
        }
    }

    void ProcessAttack(E_Enemy_Type e_enemyType)
    {
        if (e_enemyType == E_Enemy_Type.Skeleton )
        {
            if (isAttack)
            {
                isAttack = false;
                m_rigidbody.velocity = Vector3.zero;
                Invoke("ChangeStatus", 3f);
            }
        }

        if (e_enemyType == E_Enemy_Type.Dragon)
        {
            if (isAttack)
            {
                isAttack = false;
                m_rigidbody.velocity = Vector3.zero;
                Invoke("ChangeStatus", 3f);
            }
        }
    }

    void ProcessSkill(E_Enemy_Type e_enemyType)
    {
        if (e_enemyType == E_Enemy_Type.Skeleton)
        {

        }
    }

    void ProcessDeath(E_Enemy_Type e_enemyType)
    {
        if (e_enemyType == E_Enemy_Type.Skeleton && !isDeath)
        {
            StartCoroutine(EnemyDeath());
        }

        if (e_enemyType == E_Enemy_Type.Dragon && !isDeath)
        {
            StartCoroutine(EnemyDeath());
        }
    }    

    IEnumerator EnemyDeath()
    {
        m_rigidbody.isKinematic = true;
        m_enemyHitBox.SetActive(false);

        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }

    void ChangeStatus()
    {
        e_status = E_AI_STATUS.STANDING;
    }

    void LookFortress(GameObject fortress)
    {
        float direction;
        float scale;

        if (e_enemyType == E_Enemy_Type.Skeleton)
        {
            direction = (nearestFortress.GetComponent<Transform>().position.x < transform.position.x ? -1 : 1);
            scale = transform.localScale.y;
            transform.localScale = new Vector3(scale, scale, direction * scale);
            transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        }

        if (e_enemyType == E_Enemy_Type.Dragon)
        {
            direction = (nearestFortress.GetComponent<Transform>().position.x < transform.position.x ? -1 : 1);
            scale = transform.localScale.y;
            transform.localScale = new Vector3(scale, scale, direction * scale);
            transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        }
    }

    GameObject FindNearestFortress()
    {
        GameObject[] fortressList = GameObject.FindGameObjectsWithTag(fortressTag);

        GameObject nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (GameObject fortress in fortressList)
        {
            float distance = Vector3.Distance(transform.position, fortress.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = fortress;
            }
        }

        return nearestEnemy;
    }


}
