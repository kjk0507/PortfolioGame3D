using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGSetting;

public class FortressControl : MonoBehaviour
{
    public GameObject m_body;
    public GameObject m_startzone;
    //public GameObject virtualCamera;
    public GameObject m_spawnZone;
    public int m_maxEnemy;
    public int m_curEnemy;

    public GameObject m_player;

    public FortressStatus m_status = new FortressStatus();

    bool isWaving = false;

    public GameObject m_enemy;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_player != null && Input.GetKeyDown(KeyCode.A) && !isWaving)
        {
            m_body.SetActive(true);
            m_spawnZone.SetActive(true);

            m_startzone.SetActive(false);
            m_player.SetActive(false);

            GameManager.m_cInstance.isPlayWave = true;
            isWaving = true;

            StartCoroutine(SpawnEnemy(m_maxEnemy));
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
            GameObject instantCapsul = Instantiate(m_enemy, Return_RandomPosition(), Quaternion.identity);
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
