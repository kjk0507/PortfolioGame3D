using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGSetting;

public class FortressControl : MonoBehaviour
{
    public GameObject m_fortress;
    public GameObject m_body;
    public GameObject m_startzone;
    //public GameObject virtualCamera;
    public GameObject m_spawnZone;
    public GameObject m_hitBox;
    public GameObject m_defaultFortress;
    public int m_maxEnemy;
    public int m_curEnemy;

    public GameObject m_player;

    public FortressStatus m_status = new FortressStatus();

    bool isWaving = false;

    public List<GameObject> m_enemyType = new List<GameObject>();
    public List<GameObject> m_enemyList = new List<GameObject>();
    public GameObject m_enemy;

    public GameObject shieldPrefab;
    private GameObject currentShield;

    bool isPress = false;
    bool isShield = false;

    // Start is called before the first frame update
    void Start()
    {
        //m_fortress = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_player != null && (Input.GetKeyDown(KeyCode.A) || isPress) && !isWaving)
        {
            isPress = false;

            m_enemyList = new List<GameObject>();

            m_body.SetActive(true);
            m_spawnZone.SetActive(true);

            m_startzone.SetActive(false);
            m_player.SetActive(false);

            m_curEnemy = m_maxEnemy;

            GameManager.m_cInstance.isPlayWave = true;
            GameManager.m_cInstance.activeFortress = m_fortress;
            GameManager.m_cInstance.ChangeDefenseControler();

            isWaving = true;

            StartCoroutine(SpawnEnemy(m_maxEnemy));
        }

        CheckEnemyNum();

        if (isWaving)
        {
            // 방어막 구현
            if ((Input.GetKeyDown(KeyCode.B) || isShield) && GameManager.m_cInstance.FindItemNum("IC_01") > 0 && GameManager.m_cInstance.FindSkillActive("SA_01"))
            {
                isShield = false;
                GenerateShield();
                GameManager.m_cInstance.UseItem("IC_01");
            }
        }
    }
    public void PressGUIButton()
    {
        if (m_player != null)
        {
            isPress = true;
        }
    }

    public void PressShieldButton()
    {
        if (m_player != null)
        {
            isShield = true;
        }
    }

    // 여기서 wave 전부 하기 -- 시간초랑 남은 애들은 어떻게 해야하려나 -> 여기서 gamemanager의 함수를 실행시키면?
    // EventWaveStart 실행 시켜서 ui 변경 및 시간초 갱신 + 남은 마리수는 update로 계속 전송
    // 끝난뒤 여기로 다시 뭔가를 던저서 끝내는거 보이기

    IEnumerator SpawnEnemy(int m_maxEnemy)
    {
        for(int i = 0; i < m_maxEnemy; i++)
        {
            yield return new WaitForSeconds(0.5f);

            // 생성 위치 부분에 위에서 만든 함수 Return_RandomPosition() 함수 대입
            // 이거 복사시 게임 오브젝트에 데이터 입력이 가능할까? -> 입력하고 소환한다면? -> 되네
            int randomNum = Random.Range(0,11);
            if(randomNum > 9)
            {
                m_enemy = m_enemyType[1];
            }
            else
            {
                m_enemy = m_enemyType[0];
                m_enemy.GetComponentInChildren<EnemyWeapon>().m_tag = "FortressHitBox";
            }

            m_enemy.GetComponent<EnemyControl_Wave>().nearestFortress = m_hitBox;
            GameObject instantEnemy = Instantiate(m_enemy, Return_RandomPosition(), Quaternion.identity);
            m_enemyList.Add(instantEnemy);
        }
    }

    Vector3 Return_RandomPosition()
    {
        Vector3 originPosition = m_spawnZone.transform.position;

        float range_X = m_spawnZone.GetComponent<BoxCollider>().bounds.size.x;
        float range_Z = m_spawnZone.GetComponent<BoxCollider>().bounds.size.z;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Z = Random.Range((range_Z / 2) * -1, range_Z / 2);
        Vector3 RandomPostion = new Vector3(range_X, 0f, range_Z);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }

    public void ChangeFortressStatus()
    {
        m_body.SetActive(false);
        m_spawnZone.SetActive(false);

        m_startzone.SetActive(true);
        m_player.SetActive(true);

        isWaving = false;

        if(m_enemyList.Count > 0)
        {
            foreach(GameObject enemy in m_enemyList)
            {
                Destroy(enemy);
            }
        }
    }

    public void CheckEnemyNum()
    {
        if (isWaving)
        {
            int count = 0;

            foreach (GameObject enemy in m_enemyList)
            {
                if (enemy != null && enemy.GetComponent<EnemyControl_Wave>() != null && !enemy.GetComponent<EnemyControl_Wave>().m_status.IsDeath())
                {
                    count++;
                }
            }

            m_curEnemy = count;
        }
    }

    void GenerateShield()
    {
        // 이미 생성된 방어막이 있다면 제거
        if (currentShield != null)
        {
            Destroy(currentShield);
        }

        Vector3 shieldPosition = new Vector3(m_defaultFortress.transform.position.x, m_defaultFortress.transform.position.y + 1.5f, m_defaultFortress.transform.position.z);
        shieldPrefab.transform.localScale = new Vector3(10,10,10);

        // 방어막 생성 및 위치 설정
        currentShield = Instantiate(shieldPrefab, shieldPosition, Quaternion.identity);

        StartCoroutine(DestroyShield(currentShield, 5f));
    }
    IEnumerator DestroyShield(GameObject currentShield, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentShield != null)
        {
            Destroy(currentShield);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerHitBox"))
        {
            m_player = collision.transform.root.gameObject;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerHitBox"))
        {
            m_player = null;
        }
    }


}
