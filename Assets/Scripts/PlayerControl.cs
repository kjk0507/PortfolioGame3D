using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // ��� ����
    Transform m_transform;
    Rigidbody m_Rigidbody;

    // ��ȯ ����
    public float m_moveSpeed = 5f;   // �̵��ӵ�
    public float m_jumpPower = 12f;  // ������

    // Ŭ���� ����
    float inputHorizontal;

    // ���� ���� ����
    bool isMove = true;  // �̵� ���� ����
    //bool isDash = false;  // ��� ���� ����
    bool IsDoubleJump = false;  // �̴� ���� ���� ����

    // �Է� ����
    bool inputJump = false;  // ����Ű �Է� ����



    void Start()
    {
        m_transform = GetComponent<Transform>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // �¿� �̵� ����
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

        // ���� ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inputJump = true;
        }

        // (�ӽ�)���� ���� ����
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
