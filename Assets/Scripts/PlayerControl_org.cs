using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControl_org : MonoBehaviour
{
    // ��� ����
    Transform m_transform;
    Rigidbody m_rigidbody;
    Animator m_animator;

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
    bool inputAttack = false;  // ����Ű �Է� ����

    // �÷��̾� ���ѻ��¸ӽ�
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
        
        // �¿� �̵� ����
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

        // ���� ����
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

        // �� ���� position ��ġ�� �ٴ��̶� �ణ ������ Ȯ�ΰ���
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
