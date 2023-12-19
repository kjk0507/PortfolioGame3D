using UnityEngine;
using RPGSetting;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    // 멤버 변수
    Transform m_transform;
    Rigidbody m_rigidbody;
    Animator m_animator;

    // 스테이터스
    Status m_status = new Status();

    // 변환 변수
    public float m_moveSpeed = 5f;   // 이동속도
    public float m_jumpPower = 12f;  // 점프력
    public float jumpPosition = 1f;
    public float groundPosition = 1f;

    public GameObject playerArrow;
    public GameObject shotPosition;
    public float shotPower = 20f;

    public GameObject CrossBow;

    // 클래스 변수
    float inputHorizontal;

    // 가능 여부 변수
    bool isMove = true;  // 이동 가능 여부
    //bool isDash = false;  // 대시 가능 여부
    bool IsDoubleJump = false;  // 이단 점프 가능 여부
    bool isDamage = false;
    bool isJump = false;

    // 입력 변수
    bool inputJump = false;  // 점프키 입력 여부
    bool inputAttack = false;  // 공격키 입력 여부

    // 입력 시간
    private float lastAttackTime;
    private float standAttackCooldown = 0.4f;
    private float runningAttackCooldown = 2f;

    // 플레이어 유한상태머신
    enum E_Player_State { STANDING, RUNNING, STANDINGSHOT, RUNNINGSHOT, JUMP, DOUBLEJUMP, DAMAGE };
    E_Player_State playerState;


    void Start()
    {
        m_transform = GetComponent<Transform>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 피격 시 모든 움직임 정지
        if(!isDamage)
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
                //ShotArrow();
                lastAttackTime = Time.time;
            }
        }

        m_animator.SetInteger("playerState", (int)playerState);
        //Debug.Log("PlayerState : " + playerState.ToString());
    }

    private void FixedUpdate()
    {
        switch (playerState)
        {
            case E_Player_State.STANDING:
                ProcessStanding();
                CrossBow.transform.localPosition = new Vector3(0.02f, -0.14f, -0.03f);
                CrossBow.transform.localRotation = Quaternion.Euler(-8f, 172f, -34f);
                break;
            case E_Player_State.RUNNING:
                ProcessRunningShot();
                CrossBow.transform.localPosition = new Vector3(0.07f, -0.08f, 0.02f);
                CrossBow.transform.localRotation = Quaternion.Euler(-8f, 172f, -58f);
                ProcessRunning();
                break;
            case E_Player_State.STANDINGSHOT:
                ProcessStandingShout();
                CrossBow.transform.localPosition = new Vector3(0.06f, -0.13f, 0f);
                CrossBow.transform.localRotation = Quaternion.Euler(4f, 175f, -40f);
                break;
            case E_Player_State.RUNNINGSHOT:
                ProcessRunningShot();
                CrossBow.transform.localPosition = new Vector3(0.07f, -0.08f, 0.02f);
                CrossBow.transform.localRotation = Quaternion.Euler(-8f, 172f, -58f);
                break;
            case E_Player_State.JUMP:
                ProcessJump();
                CrossBow.transform.localPosition = new Vector3(0.02f, -0.14f, -0.03f);
                CrossBow.transform.localRotation = Quaternion.Euler(-8f, 172f, -34f);
                break;
            case E_Player_State.DOUBLEJUMP:
                ProcessDoubleJump();
                CrossBow.transform.localPosition = new Vector3(0.02f, -0.14f, -0.03f);
                CrossBow.transform.localRotation = Quaternion.Euler(-8f, 172f, -34f);
                break;
            case E_Player_State.DAMAGE:
                ProcessDamage();
                break;
        }
    }

    // 상태 함수
    void ProcessStanding()
    {
        //m_rigidbody.velocity = new Vector3(0, m_rigidbody.transform.position.y, 0);
        m_rigidbody.velocity = Vector3.zero;

        // 가능한 동작 : Running, StandingShot, Jump, Throw, Damage
        // 윗 코드는 동작 전달, 밑 코드는 그 함수에서 해야할 동작
        // 피격 받을 시 멈춤
        if (isDamage)
        {
            playerState = E_Player_State.DAMAGE;
            return;
        }

        // 입력값이 있다면 running으로 전환
        if (inputHorizontal != 0)
        {
            playerState = E_Player_State.RUNNING;
            return;
        }

        if (inputJump && IsGrounded())
        {
            playerState = E_Player_State.JUMP;
            isJump = true;
            return;
        }

        if (inputAttack)
        {
            playerState = E_Player_State.STANDINGSHOT;
            return;
        }        
    }

    void ProcessRunning()
    {
        // 가능한 동작 : Standing, RunningShot, Jump, Damage
        if (isDamage)
        {
            playerState = E_Player_State.DAMAGE;
            return;
        }

        if( inputHorizontal == 0)
        {
            playerState = E_Player_State.STANDING;
            return;
        }

        if(inputHorizontal != 0 && inputAttack)
        {
            playerState = E_Player_State.RUNNINGSHOT;
            return;
        }

        if(inputJump && IsGrounded())
        {
            playerState = E_Player_State.JUMP;
            isJump = true;
            return;
        }

        // 달리기 구현
        m_rigidbody.velocity = new Vector3(inputHorizontal * m_moveSpeed, m_rigidbody.velocity.y, 0);
    }

    void ProcessStandingShout()
    {
        // 가능한 동작 : Standing, Running, Jump, Damage
        if (isDamage)
        {
            playerState = E_Player_State.DAMAGE;
            return;
        }

        if ((Time.time - lastAttackTime) >= standAttackCooldown)
        {
            playerState = E_Player_State.STANDING;
            return;
        }

        if(inputHorizontal != 0)
        {
            playerState = E_Player_State.RUNNINGSHOT;
            return;
        }

        if (inputJump && IsGrounded())
        {
            playerState = E_Player_State.JUMP;
            isJump = true;
            return;
        }

        if (inputAttack)
        {
            //m_rigidbody.velocity = new Vector3(0, m_rigidbody.transform.position.y, 0);
            m_rigidbody.velocity = Vector3.zero;
            ShotArrow();
        }
    }

    void ProcessRunningShot()
    {
        // 가능한 동작 : Standing, Running, Jump, Damage
        if (isDamage)
        {
            playerState = E_Player_State.DAMAGE;
            return;
        }

        if (inputHorizontal == 0)
        {
            playerState = E_Player_State.STANDINGSHOT;
            return;
        }

        if (inputHorizontal != 0 && (Time.time - lastAttackTime) >= runningAttackCooldown)
        {
            playerState = E_Player_State.RUNNING;
            return;
        }

        if (inputJump && IsGrounded())
        {
            playerState = E_Player_State.JUMP;
            isJump = true;
            return;
        }

        if (inputAttack)
        {
            ShotArrow();
        }
        m_rigidbody.velocity = new Vector3(inputHorizontal * m_moveSpeed, m_rigidbody.velocity.y, 0);
    }

    void ProcessJump()
    {
        m_animator.SetBool("isGround", false);

        // 가능한 동작 : Standing, DoubleJump, Damage
        if (isDamage)
        {
            playerState = E_Player_State.DAMAGE;
            return;
        }

        // 점프 한번만
        if(inputJump && IsGrounded() && isJump)
        {
            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, 0);
            m_rigidbody.AddForce(Vector3.up * m_jumpPower, ForceMode.Impulse);
            //isJump = false;
        }

        inputJump = false;

        if(inputHorizontal != 0)
        {
            m_rigidbody.velocity = new Vector3(inputHorizontal * m_moveSpeed, m_rigidbody.velocity.y, 0);
        }

        if (IsDoubleJump)
        {
            playerState = E_Player_State.DOUBLEJUMP;
            return;
        }

        Invoke("JumpMotionStop", 0.05f);
        //JumpMotionStop();
    }

    void ProcessDoubleJump()
    {
        m_animator.SetBool("isGround", false);
        if (isDamage)
        {
            playerState = E_Player_State.DAMAGE;
            return;
        }

        if(inputJump)
        {
            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, 0);
            m_rigidbody.AddForce(Vector3.up * m_jumpPower, ForceMode.Impulse);
            IsDoubleJump = false;
        }

        inputJump = false;

        if (inputHorizontal != 0)
        {
            m_rigidbody.velocity = new Vector3(inputHorizontal * m_moveSpeed, m_rigidbody.velocity.y, 0);
        }

        Invoke("JumpMotionStop", 0.05f);
        //JumpMotionStop();
    }

    void ProcessDamage()
    {

    }

    void ShotArrow()
    {
        inputAttack = false;
        Quaternion rotation = new Quaternion();
        if(m_transform.localScale.z > 0f)
        {
            rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else
        {
            rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        GameObject objArrow = Instantiate(playerArrow, shotPosition.transform.position, rotation);
        objArrow.GetComponent<Rigidbody>().velocity = new Vector3(m_transform.localScale.z * shotPower, 0, 0);

        StartCoroutine(DestroyArrow(objArrow, 5f));
    }

    IEnumerator DestroyArrow(GameObject objArrow, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (objArrow != null)
        {
            Destroy(objArrow);
        }
    }

    bool IsGrounded()
    {
        int layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Flatform"));

        // 이 모델은 position 위치가 바닥이라 약간 위에서 확인가능
        Vector3 tempPosition = new Vector3(m_transform.position.x, m_transform.position.y + jumpPosition, m_transform.position.z);

        //bool isHit = Physics.Raycast(m_transform.position, Vector3.down, 1.1f, layerMask);
        bool isHit = Physics.Raycast(tempPosition, Vector3.down, groundPosition, layerMask);
        return isHit;
    }

    void JumpMotionStop()
    {        
        if (IsGrounded())
        {
            m_animator.SetBool("isGround", true);
            isJump = false;
            //m_rigidbody.velocity = new Vector3(0, m_rigidbody.transform.position.y, 0);

            //if (inputHorizontal != 0)
            //{
            //    playerState = E_Player_State.RUNNING;
            //    return;
            //}
            //else
            //{
            //    playerState = E_Player_State.STANDING;
            //    return;
            //    //Invoke("ChangeStandingState", 0.1f);
            //}

            playerState = E_Player_State.RUNNING;
            return;
        }
    }

    void ChangeStandingState()
    {
        playerState = E_Player_State.STANDING;
        return;
    }

    //private void OnGUI()
    //{
    //    Vector3 tempPosition = new Vector3(m_transform.position.x, m_transform.position.y + 1, m_transform.position.z);
    //    Ray ray = new Ray(tempPosition, Vector3.down);
    //    float rayLength = 0.5f;
    //    Vector3 endPosition = ray.origin + ray.direction * rayLength;
    //    Debug.DrawLine(ray.origin, ray.direction * rayLength, Color.blue);
    //    Debug.DrawLine(ray.origin, endPosition, Color.blue, 10);
    //}
}
