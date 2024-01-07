using UnityEngine;
using RPGSetting;
using System.Collections;
using System.Runtime.InteropServices;

public class PlayerControl : MonoBehaviour
{
    // 멤버 변수
    Transform m_transform;
    Rigidbody m_rigidbody;
    Animator m_animator;

    // 스테이터스 -> 게임매니저에서 관리
    //public Status m_status = new Status();

    // 변환 변수
    public float m_moveSpeed = 5f;   // 이동속도
    public float m_jumpPower = 12f;  // 점프력
    public float jumpPosition = 1f;
    public float groundPosition = 1f;
    public float shotTime = 1f;

    public GameObject playerArrow;
    public GameObject shotPosition;
    public float shotPower = 20f;

    public GameObject CrossBow;

    public GameObject shieldPrefab;
    private GameObject currentShield;

    public int jumpCount;

    // 클래스 변수
    float inputHorizontal;

    // 가능 여부 변수
    bool isMove = true;  // 이동 가능 여부
    //bool isDash = false;  // 대시 가능 여부
    bool IsDoubleJump = false;  // 이단 점프 가능 여부
    bool isDamage = false;
    public bool isStop = false;
    bool isJump = false;
    bool isShot = false;
    public bool isPlatform = false; // 이건 플랫폼에서 변경

    // 입력 변수
    bool inputJump = false;  // 점프키 입력 여부
    bool inputAttack = false;  // 공격키 입력 여부

    // 입력 시간
    private float lastAttackTime;
    private float standAttackCooldown = 0.4f;
    private float runningAttackCooldown = 2f;

    // 플레이어 유한상태머신
    public enum E_Player_State { STANDING, RUNNING, STANDINGSHOT, RUNNINGSHOT, JUMP, DOUBLEJUMP, DAMAGE };
    public E_Player_State playerState;

    // 스킬 확인
    SkillManager m_skillManager = new SkillManager();

    void Start()
    {
        m_transform = GetComponent<Transform>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 피격 시 모든 움직임 정지
        if(!isDamage || !isStop)
        {
            // 좌우 이동 구현
            if (isMove)
            {
                inputHorizontal = Input.GetAxisRaw("Horizontal");

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
            if (Input.GetKeyDown(KeyCode.Space) && !inputJump && jumpCount < 2)
            {
                inputJump = true;
            }

            // (임시)더블 점프 구현
            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    IsDoubleJump = true;
            //}
            
            if(m_skillManager.FindSkillTrue(GameManager.m_cInstance.playerStatus, "SP_01"))
            {
                IsDoubleJump = true;
            }

            // 공격 구현
            if (Input.GetKeyDown(KeyCode.Z) && !inputAttack)
            {
                inputAttack = true;
                //ShotArrow();
                lastAttackTime = Time.time;
            }

            // 방어막 구현
            if (Input.GetKeyDown(KeyCode.B))
            {
                GenerateShield();
            }

            // 방어막이 생성되어 있고 플레이어가 존재한다면 플레이어를 따라다니게 함
            if (currentShield != null && m_transform != null)
            {
                UpdateShieldPosition();
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
                CrossBow.transform.localPosition = new Vector3(-0.01f, 0.08f, 0.08f);
                CrossBow.transform.localRotation = Quaternion.Euler(45f, 380f, 127f);
                break;
            case E_Player_State.DOUBLEJUMP:
                ProcessDoubleJump();
                CrossBow.transform.localPosition = new Vector3(-0.01f, 0.08f, 0.08f);
                CrossBow.transform.localRotation = Quaternion.Euler(45f, 380f, 127f);
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
        if (IsGrounded())
        {
            if (IsPlatform())
            {
                m_rigidbody.velocity = Vector3.zero;
            }
            else
            {
                m_rigidbody.velocity = Vector3.zero;
            }
        }

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
            jumpCount++;
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

        if (inputAttack)
        {
            ShotArrow();
        }

        Invoke("JumpMotionStop", 0.05f);
        //JumpMotionStop();
        IsEnemy();
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
            jumpCount++;
            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, 0);
            m_rigidbody.AddForce(Vector3.up * m_jumpPower, ForceMode.Impulse);
            IsDoubleJump = false;
        }

        inputJump = false;

        if (inputHorizontal != 0)
        {
            m_rigidbody.velocity = new Vector3(inputHorizontal * m_moveSpeed, m_rigidbody.velocity.y, 0);
        }

        if (inputAttack)
        {
            ShotArrow();
        }

        Invoke("JumpMotionStop", 0.05f);
        //JumpMotionStop();
        IsEnemy();
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

        if (!isShot)
        {
            isShot = true;
            GameObject objArrow = Instantiate(playerArrow, shotPosition.transform.position, rotation);
            objArrow.GetComponent<Rigidbody>().velocity = new Vector3(m_transform.localScale.z * shotPower, 0, 0);

            Invoke("ShotCoolTime",shotTime);
        }
    }

    void ShotCoolTime()
    {
        isShot = false;
    }

    bool IsGrounded()
    {
        int layerMask = ((1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Platform")));

        // 이 모델은 position 위치가 바닥이라 약간 위에서 확인가능
        Vector3 tempPosition = new Vector3(m_transform.position.x, m_transform.position.y + jumpPosition, m_transform.position.z);

        //bool isHit = Physics.Raycast(m_transform.position, Vector3.down, 1.1f, layerMask);
        bool isHit = Physics.Raycast(tempPosition, Vector3.down, groundPosition, layerMask);
        return isHit;
    }

    bool IsPlatform()
    {
        int layerMask = (1 << LayerMask.NameToLayer("Platform"));

        Vector3 tempPosition = new Vector3(m_transform.position.x, m_transform.position.y + jumpPosition, m_transform.position.z);

        bool isHit = Physics.Raycast(tempPosition, Vector3.down, groundPosition, layerMask);
        return isHit;
    }

    void IsEnemy()
    {
        //Debug.Log("IsEnemy");
        bool isEnemy = false;

        int layerMask = (1 << LayerMask.NameToLayer("EnemyHitBox"));
        Vector3 tempPosition = new Vector3(m_transform.position.x, m_transform.position.y + 0.5f, m_transform.position.z);
        isEnemy = Physics.Raycast(tempPosition, Vector3.down, 1.1f, layerMask);
        if(isEnemy)
        {
            //Debug.Log("Enemy");
            m_rigidbody.velocity = new Vector3(m_transform.localScale.z * 3f, 0, 0);
        }
        //return isEnemy;
    }

    void JumpMotionStop()
    {        
        if (IsGrounded())
        {
            m_animator.SetBool("isGround", true);
            isJump = false;
            jumpCount = 0;
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

            if (IsPlatform() && isPlatform)
            {
                isPlatform = false;
                Invoke("PlatformCollision", 1f);
            }
            else
            {
                playerState = E_Player_State.RUNNING;
                return;
            }
        }
    }

    void PlatformCollision()
    {
        Debug.Log("platform");        

        playerState = E_Player_State.RUNNING;
        return;
    }

    void ChangeStandingState()
    {
        playerState = E_Player_State.STANDING;
        return;
    }

    void GenerateShield()
    {
        // 이미 생성된 방어막이 있다면 제거
        if (currentShield != null)
        {
            Destroy(currentShield);
        }

        Vector3 shieldPosition = new Vector3(m_transform.position.x, m_transform.position.y + 1.5f, m_transform.position.z);

        // 방어막 생성 및 위치 설정
        currentShield = Instantiate(shieldPrefab, shieldPosition, Quaternion.identity);

        StartCoroutine(DestroyShield(currentShield, 3f));
    }
    void UpdateShieldPosition()
    {
        Vector3 shieldPosition = new Vector3(m_transform.position.x, m_transform.position.y + 1.5f, m_transform.position.z);
        // 플레이어 위치를 기준으로 방어막 위치 조정
        currentShield.transform.position = shieldPosition;
    }

    IEnumerator DestroyShield(GameObject currentShield, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentShield != null)
        {
            Destroy(currentShield);
        }
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

    //public void OnGUI()
    //{
    //    //오브젝트의 3d좌표를 2d좌표(스크린좌표)로 변환하여 GUI를 그린다.
    //    Vector3 vPos = this.transform.position;
    //    Vector3 vPosToScreen = Camera.main.WorldToScreenPoint(vPos); //월드좌표를 스크린좌표로 변환한다.
    //    vPosToScreen.y = Screen.height - vPosToScreen.y; //y좌표의 축이 하단을 기준으로 정렬되므로 상단으로 변환한다.
    //    int h = 40;
    //    int w = 100;
    //    //Rect rectGUI = new Rect(vPosToScreen.x, vPosToScreen.y, w, h);
    //    Rect rectGUI = new Rect(vPosToScreen.x - w / 2, vPosToScreen.y, w, h);
    //    //GUI.Box(rectGUI, "MoveBlock:" + isMoveBlock);
    //    GUI.Box(rectGUI, string.Format("HP:{0} / {1}", m_status.GetCurHp().ToString(), m_status.GetFinalHp().ToString()));
    //}
}
