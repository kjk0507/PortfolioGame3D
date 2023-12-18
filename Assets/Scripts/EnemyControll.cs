using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGSetting;
using UnityEngine.Playables;
using static EnemyControl;

public class EnemyControl : MonoBehaviour
{
    Transform m_transform;
    Rigidbody m_rigidbody;
    Animator m_animator;

    bool isDetected = false;  // 플레이어 발견
    GameObject m_player = null;
    int randomDirection = 1;  // 초기 적 이동방향(오른쪽)
    bool isAttack = false;

    // 인스펙터 수정 가능 항목
    public float m_moveSpeed = 2;

    // Tracking 관련 시간 조절
    private float lastDirectionChangeTime;
    private float directionChangeInterval = 5f;

    // Thinking 관련 시간 조절
    float thinkingDuration = 2.5f;
    float thinkingStartTime;


    // 기본 선언
    public E_Enemy_Type e_enemyType;
    public E_AI_STATUS e_status = E_AI_STATUS.STANDING;
    public Status m_status = new Status();

    public enum E_Enemy_Type
    {
        Skeleton
    }

    public enum E_AI_STATUS
    {
        STANDING,  // 가만히 서있기
        TRACKING,  // 좌 우로 이동
        THINKING,  // 낭떠러지 도착시 멈춤
        MOVE,      // 플레이어 인식 후 이동
        ATTACK,    // 플레이어 공격
        SKILL      // 스킬 발동
    }

    void Start()
    {
        m_transform = GetComponent<Transform>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (m_status.IsDeath())
        {
            Destroy(this.gameObject);
        }

        m_animator.SetInteger("e_status", (int)e_status);
    }

    private void FixedUpdate()
    {
        //Debug.Log("e_status" + e_status.ToString());
        switch (e_status)
        {
            case E_AI_STATUS.STANDING:
                ProcessStanding(e_enemyType);
                break;
            case E_AI_STATUS.TRACKING:
                ProcessTracking(e_enemyType);
                break;
            case E_AI_STATUS.THINKING:
                ProcessThinking(e_enemyType);
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
        }
    }

    // 그냥 서있는 단계
    void ProcessStanding(E_Enemy_Type e_enemyType)
    {
        if (e_enemyType == E_Enemy_Type.Skeleton)
        {
            // 플레이어 발견
            if (m_player != null && isDetected == true)
            {
                // 거리 재고 move 또는 attack으로
                e_status = E_AI_STATUS.MOVE;
                return;
            }
            else
            {
                e_status = E_AI_STATUS.TRACKING;
                return;
            }
        }
    }

    // 좌 우로 움직이는 단계
    void ProcessTracking(E_Enemy_Type e_enemyType)
    {
        if(e_enemyType == E_Enemy_Type.Skeleton)
        {
            // 플레이어 발견시 바로 전환
            if (m_player != null && isDetected == true)
            {
                float distance = Vector3.Distance(transform.position, m_player.transform.position);
                e_status = (distance > 3) ? E_AI_STATUS.MOVE : E_AI_STATUS.ATTACK;
                return;
            }

            float scale = transform.localScale.x;
            if(randomDirection != 0)
            {
                transform.localScale = new Vector3(scale, scale, randomDirection * scale);
            }
            Vector3 direction = new Vector3(randomDirection * m_moveSpeed, 0, 0);
            m_rigidbody.velocity = direction;

            if (Time.time - lastDirectionChangeTime >= directionChangeInterval)
            {
                ChangeDirection();
                lastDirectionChangeTime = Time.time;
            }

            // 절벽이면 thinking으로
            if(IsCliff())
            {
                e_status = E_AI_STATUS.THINKING;
                thinkingStartTime = Time.time;
            }
        }
    }

    void ProcessThinking(E_Enemy_Type e_enemyType)
    {
        if (e_enemyType == E_Enemy_Type.Skeleton)
        {
            // 플레이어 발견시 바로 전환
            if (m_player != null && isDetected == true)
            {
                e_status = E_AI_STATUS.MOVE;
                return;
            }

            if (Time.time - thinkingStartTime >= thinkingDuration)
            {
                randomDirection = -1 * randomDirection;
                e_status = E_AI_STATUS.STANDING;
            }
        }

    }

    // 플레이어 발견하고 다가가는 단계
    void ProcessMove(E_Enemy_Type e_enemyType)
    {
        if (e_enemyType == E_Enemy_Type.Skeleton)
        {
            // 절벽이면 thinking으로 
            if (IsCliff())
            {
                e_status = E_AI_STATUS.THINKING;
                thinkingStartTime = Time.time;
            }

            if(isDetected && m_player != null)
            {
                
                // 플레이어를 바라봄
                LookPlayer(m_player);

                // 플레이어 방향을 확인(90도 회전한 상태라 방향이 z)
                Vector3 direction = new Vector3(m_transform.localScale.z, 0, 0);
                direction.Normalize();
                float distance = Vector3.Distance(transform.position, m_player.transform.position);

                Debug.Log("still Moving");
                if (distance > 3)
                {
                    m_rigidbody.velocity = new Vector3(direction.x * 5f, m_rigidbody.velocity.y, 0);
                }
                else
                {
                    isAttack = true;
                    e_status = E_AI_STATUS.ATTACK;
                }
            }
            else
            {
                e_status = E_AI_STATUS.STANDING;
            }

        }
    }

    // 플레이어 발견하고 공격하는 단계
    void ProcessAttack(E_Enemy_Type e_enemyType)
    {
        if (isAttack)
        {
            isAttack = false;
            //e_status = E_AI_STATUS.STANDING;
            m_rigidbody.velocity = Vector3.zero;
            Invoke("ChangeStatus", 3f);
        }
    }

    // 때때로 스킬 발동
    void ProcessSkill(E_Enemy_Type e_enemyType)
    {

    }

    void ChangeDirection()
    {
        int randomInt = Random.Range(1, 3);// 1이면 왼쪽, 2이면 오른쪽
        if (randomInt == 1)
        {
            randomDirection = -1;
        }
        else
        {
            randomDirection = 1;
        }
    }

    bool IsCliff()
    {
        int layerMask = (1 << LayerMask.NameToLayer("Ground"));
        Vector3 tempPosition = new Vector3(m_transform.position.x + randomDirection * 1.2f, m_transform.position.y, m_transform.position.z);
        //Debug.DrawRay(tempPosition, Vector3.down, new Color(0, 1, 0));
        bool isHit = Physics.Raycast(tempPosition, Vector3.down, 1.1f, layerMask);
        return !isHit;
    }

    void LookPlayer(GameObject player)
    {
        float direction = (player.GetComponent<Transform>().position.x < transform.position.x ? -1 : 1);
        float scale = transform.localScale.x;
        transform.localScale = new Vector3(scale, scale, direction * scale);
    }

    void ChangeStatus()
    {
        e_status = E_AI_STATUS.STANDING;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("PlayerHitBox"))
        {
            isDetected = true;
            m_player = collision.transform.parent.gameObject;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerHitBox"))
        {
            isDetected = false;
            m_player = null;
        }
    }

    //private void OnGUI()
    //{
    //    Vector3 tempPosition = new Vector3(m_transform.position.x + randomDirection * 1.2f, m_transform.position.y, m_transform.position.z);
    //    Ray ray = new Ray(tempPosition, Vector3.down);
    //    float rayLength = 0.5f;
    //    Vector3 endPosition = ray.origin + ray.direction * rayLength;
    //    Debug.DrawLine(ray.origin, endPosition, Color.blue, 10);
    //}
}
