using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControl_org : MonoBehaviour
{
    // 멤버 변수
    Transform m_transform;
    Rigidbody m_rigidbody;
    Animator m_animator;

    // 변환 변수
    public float m_moveSpeed = 5f;   // 이동속도
    public float m_jumpPower = 12f;  // 점프력

    // 클래스 변수
    float inputHorizontal;

    // 가능 여부 변수
    bool isMove = true;  // 이동 가능 여부
    //bool isDash = false;  // 대시 가능 여부
    bool IsDoubleJump = false;  // 이단 점프 가능 여부

    // 입력 변수
    bool inputJump = false;  // 점프키 입력 여부
    bool inputAttack = false;  // 공격키 입력 여부

    // 플레이어 유한상태머신
    enum e_playerState { WAIT, MOVE, JUMP , ATTACK };
    e_playerState playerState;


    void Start()
    {
        m_transform = GetComponent<Transform>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }

    void Update()
    {
        
        // 좌우 이동 구현
        if (isMove)
        {
            inputHorizontal = Input.GetAxis("Horizontal");

            if (inputHorizontal > 0)
            {
                m_transform.localScale = new Vector3(2, 2, 2);
            }
            else if (inputHorizontal < 0)
            {
                m_transform.localScale = new Vector3(2, 2, -2);
            }
        }

        // 점프 구현
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inputJump = true;
        }

        // (임시)더블 점프 구현
        if (Input.GetKeyDown(KeyCode.Q))
        {
            IsDoubleJump = true;
        }

        // 공격 구현
        if (Input.GetKeyDown(KeyCode.Z))
        {
            inputAttack = true;
        }
    }

    private void FixedUpdate()
    {
        m_animator.SetInteger("playerState", (int)playerState);
        if (inputHorizontal != 0)
        {
            //m_transform.position += new Vector3(inputHorizontal, 0, 0) * Time.deltaTime * m_moveSpeed;
            m_rigidbody.velocity = new Vector3(inputHorizontal * m_moveSpeed, m_rigidbody.velocity.y, 0);
            //m_animator.SetBool("isWalk", true);
            if(playerState != e_playerState.JUMP)
            {
                playerState = e_playerState.MOVE;
            }
        }
        else
        {
            if (playerState != e_playerState.JUMP)
            {
                playerState = e_playerState.WAIT;
            }
        }

        if (inputJump)
        {
            //Debug.Log("isjump?");
            if (IsGrounded())
            {
                //Debug.Log("ground");
                playerState = e_playerState.JUMP;
                m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, 0);
                m_rigidbody.AddForce(Vector3.up * m_jumpPower, ForceMode.Impulse);
                //m_animator.SetBool("inputJump", inputJump);
                //StartCoroutine(JumpMotionStop());
            }
            else
            {
                //Debug.Log("noground");
                if (IsDoubleJump)
                {
                    m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, 0);
                    m_rigidbody.AddForce(Vector3.up * m_jumpPower, ForceMode.Impulse);
                    IsDoubleJump = false;
                }
            }

            inputJump = false;
            //m_Animator.SetBool("inputJump", inputJump);
        }

        if (playerState == e_playerState.JUMP)
        {
            StartCoroutine(JumpMotionStop());
        }

        if (inputAttack)
        {
            playerState = e_playerState.ATTACK;
        }
    }
    bool IsGrounded()
    {
        int layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Flatform"));

        // 이 모델은 position 위치가 바닥이라 약간 위에서 확인가능
        Vector3 tempPosition = new Vector3(m_transform.position.x, m_transform.position.y + 1, m_transform.position.z);

        //bool isHit = Physics.Raycast(m_transform.position, Vector3.down, 1.1f, layerMask);
        bool isHit = Physics.Raycast(tempPosition, Vector3.down, 1.1f, layerMask);
        return isHit;
    }

    IEnumerator JumpMotionStop()
    {
        yield return new WaitForSeconds(0.1f);
        //m_animator.SetBool("inputJump", false);
        if (IsGrounded())
        {
            if(inputHorizontal != 0)
            {
                playerState = e_playerState.MOVE;
            }
            else
            {
                playerState = e_playerState.WAIT;
            }            
        }
    }    

    //private void OnGUI()
    //{
    //    Vector3 tempPosition = new Vector3(m_transform.position.x, m_transform.position.y + 1, m_transform.position.z);
    //    Ray ray = new Ray(tempPosition, Vector3.down);
    //    float rayLength = 0.5f;
    //    Vector3 endPosition = ray.origin + ray.direction * rayLength;
    //    //Debug.DrawLine(ray.origin, ray.direction * rayLength, Color.blue);
    //    Debug.DrawLine(ray.origin, endPosition, Color.blue, 10);
    //}
}
