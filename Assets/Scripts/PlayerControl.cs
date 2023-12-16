using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // 멤버 변수
    Transform m_transform;
    Rigidbody m_Rigidbody;

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



    void Start()
    {
        m_transform = GetComponent<Transform>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 좌우 이동 구현
        if (isMove)
        {
            inputHorizontal = Input.GetAxis("Horizontal");

            if (inputHorizontal > 0)
            {
                m_transform.localScale = new Vector3(1, 1, 1);
            }
            else if (inputHorizontal < 0)
            {
                m_transform.localScale = new Vector3(-1, 1, 1);
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
    }

    private void FixedUpdate()
    {
        if(inputHorizontal != 0)
        {
            //m_transform.position += new Vector3(inputHorizontal, 0, 0) * Time.deltaTime * m_moveSpeed;
            m_Rigidbody.velocity = new Vector3(inputHorizontal * m_moveSpeed, m_Rigidbody.velocity.y, 0);
        }

        if (inputJump)
        {
            if (IsGrounded())
            {
                m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, 0);
                m_Rigidbody.AddForce(Vector3.up * m_jumpPower, ForceMode.Impulse);
            }
            else
            {
                if (IsDoubleJump)
                {
                    m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, 0);
                    m_Rigidbody.AddForce(Vector3.up * m_jumpPower, ForceMode.Impulse);
                    IsDoubleJump = false;
                }
            }

            inputJump = false;
        }
    }
    bool IsGrounded()
    {
        int layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Flatform"));

        bool isHit = Physics.Raycast(m_transform.position, Vector3.down, 1.2f, layerMask);
        return isHit;
    }
}
