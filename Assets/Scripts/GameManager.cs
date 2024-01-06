using RPGSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager m_cInstance;

    public List<GameObject> m_hpList; // hp 스테이터스 리스트
    public List<GameObject> m_hpList2; // 요세 HP 스테이터스 리스트
    public List<GameObject> m_listGUIScenes; // GUI 리스트
    public GameObject m_inventory;  // 인벤토리 GUI
    public GUIInventory m_guiInventory;  // 인벤토리 하위 레이어

    public enum E_GUI_STATE { TITLE, PLAY, GAMEOVER, THEEND, PLAY_WAVE }  // GUI 상태
    public E_GUI_STATE m_curGUIState;
    bool isDeath = false;

    // wave 관련
    public bool isPlayWave = false;  // wave 상태여부
    public GameObject activeFortress; // 활성화된 요새
    public GameObject activePlayer;   // 활성화된 플레이어
    bool isTimerStart = false;
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI enemyNum;
    public float countdownDuration = 60f;  // wave 시간
    private float currentTime;

    // 상태창, 인벤토리, 스킬
    [SerializeField] ItemManager m_cItemManager = new ItemManager();  // 모든 아이템 리스트가 포함
    public Status playerStatus = new Status();
    public SkillInfo playerSkill = new SkillInfo();
    public FortressStatus playerFortessStatus = new FortressStatus();

    public List<GameObject> popupGUIList = new List<GameObject>();
    public enum E_GUI_POPUP { STATUS, INVENTORY, SKILL}
    public GUIStatus m_guiStatus;

    private void Awake()
    {
        m_cInstance = this;
        m_cItemManager.Init();
        m_cItemManager.SetPlayerAllData(playerStatus);
        playerStatus.name = "Player";
    }

    void Start()
    {
        EventUpdateStatus();
        SetGUIScene(m_curGUIState);
    }

    void Update()
    {
        UpdateState();
        EventUpdateStatus();
        SetGUIScene(m_curGUIState);
        WaveTimer();
        WaveStatus();
        IsPain();
    }

    void EventUpdateStatus()
    {
        if(m_curGUIState == E_GUI_STATE.PLAY || m_curGUIState == E_GUI_STATE.PLAY_WAVE)
        SetHPBar(playerStatus.GetCurHp(), playerStatus.GetFinalHp());
        if (!isDeath && playerStatus.IsDeath())
        {
            isDeath = true;
            SetGUIScene(E_GUI_STATE.GAMEOVER);

            Invoke("LoadTitleScene", 2f);
        }
    }

    void SetHPBar(float cur, float max)
    {
        List<GameObject> list = new List<GameObject>();

        if(m_curGUIState == E_GUI_STATE.PLAY)
        {
            list = m_hpList;
        } else if(m_curGUIState == E_GUI_STATE.PLAY_WAVE)
        {
            list = m_hpList2;
        }

        for (int i = 0; i < cur; i++)
        {
            list[i].SetActive(true);
        }

        for (int j = (int)cur; j < list.Count; j++)
        {
            list[j].SetActive(false);
        }
    }

    void UpdateState()
    {
        if (isPlayWave)
        {
            m_curGUIState = E_GUI_STATE.PLAY_WAVE;
        }

        switch (m_curGUIState)
        {
            case E_GUI_STATE.TITLE:
                break;
            case E_GUI_STATE.THEEND:
                break;
            case E_GUI_STATE.GAMEOVER:
                break;
            case E_GUI_STATE.PLAY_WAVE:
                EventWaveStart();
                break;
            case E_GUI_STATE.PLAY:
                EventUpdateStatus();
                if(Input.GetKeyDown(KeyCode.S)) {
                    PopupStatus();
                }
                if (Input.GetKeyDown(KeyCode.I))
                {
                    PopupIventroy();
                }
                if (Input.GetKeyDown(KeyCode.K))
                {
                    PopupSkill();
                }
                break;
        }
    }

    public void EventWaveStart()
    {
        if(m_curGUIState == E_GUI_STATE.PLAY_WAVE && isPlayWave)
        {
            isPlayWave = false;
            playerStatus.curHp = playerStatus.maxHp + playerFortessStatus.plusHP;
            playerStatus.maxHp = playerStatus.maxHp + playerFortessStatus.plusHP;
            SpawnEnemy();
            StartTimer();
        }
    }

    void SpawnEnemy()
    {

    }

    void StartTimer()
    {
        isTimerStart = true;
        currentTime = countdownDuration;
    }

    public void EventChageScene(int stateNumber)
    {
        SetGUIScene((E_GUI_STATE)stateNumber);
    }


    void SetGUIScene(E_GUI_STATE state)
    {
        switch (state)
        {
            case E_GUI_STATE.TITLE:
                Time.timeScale = 0;
                break;
            case E_GUI_STATE.THEEND:
                Time.timeScale = 0;
                break;
            case E_GUI_STATE.GAMEOVER:
                Time.timeScale = 1;
                break;
            case E_GUI_STATE.PLAY:
                Time.timeScale = 1;
                break;
        }
        ShowScenec(state);
        m_curGUIState = state;
    }

    public void ShowScenec(E_GUI_STATE state)
    {
        for (int i = 0; i < m_listGUIScenes.Count; i++)
        {
            if ((E_GUI_STATE)i == state)
                m_listGUIScenes[i].SetActive(true);
            else
                m_listGUIScenes[i].SetActive(false);
        }
    }

    void LoadTitleScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void PopupIventroy()
    {
        if (m_inventory.activeSelf == false)
        {
            m_guiInventory.SetIventory(playerStatus);
            foreach(GameObject popup in popupGUIList)
            {
                popup.SetActive(false);
            }
            m_inventory.SetActive(true);
        }
        else
        {
            m_inventory.SetActive(false);
            foreach (GameObject popup in popupGUIList)
            {
                popup.SetActive(false);
            }
            m_guiInventory.ResetIventoryButton();
        }
    }

    public void PopupStatus()
    {
        if (popupGUIList[(int)E_GUI_POPUP.STATUS].activeSelf == false)
        {
            foreach (GameObject popup in popupGUIList)
            {
                popup.SetActive(false);
            }
            popupGUIList[(int)E_GUI_POPUP.STATUS].SetActive(true);
            m_guiStatus.SettingStatus(playerStatus, playerFortessStatus);
        }
        else
        {
            foreach (GameObject popup in popupGUIList)
            {
                popup.SetActive(false);
            }
        }
    }

    public void PopupSkill()
    {
        if (popupGUIList[(int)E_GUI_POPUP.SKILL].activeSelf == false)
        {
            foreach (GameObject popup in popupGUIList)
            {
                popup.SetActive(false);
            }
            popupGUIList[(int)E_GUI_POPUP.SKILL].SetActive(true);
        }
        else
        {
            foreach (GameObject popup in popupGUIList)
            {
                popup.SetActive(false);
            }
        }
    }

    public void WaveTimer()
    {
        if (isTimerStart)
        {
            if (currentTime > 0f)
            {
                Timer.text = Mathf.FloorToInt(currentTime).ToString();
                currentTime -= Time.deltaTime;

                // 타이머가 만료되면 원하는 작업 수행
                if (currentTime <= 0f)
                {
                    currentTime = 0f;
                    Debug.Log("타이머 만료!");
                    // 원하는 작업을 여기에 추가
                }
            }

            if (currentTime <= 0f)
            {
                isTimerStart = false;
                ChangeFortressMode();                
            }
        }
    }

    public void ChangeFortressMode()
    {
        m_curGUIState = E_GUI_STATE.PLAY;
        activeFortress.GetComponent<FortressControl>().ChangeFortressStatus();
        playerStatus.maxHp = playerStatus.maxHp - playerFortessStatus.plusHP;
        playerStatus.curHp = playerStatus.maxHp;
    }

    public void WaveStatus()
    {
        if (isTimerStart)
        {
            int maxNum = activeFortress.GetComponent<FortressControl>().m_maxEnemy;
            int curNum = activeFortress.GetComponent<FortressControl>().m_curEnemy;
            string enemyCountText = curNum.ToString() + " / " + maxNum.ToString();
            enemyNum.text = enemyCountText;
        }
        
    }

    public void IsPain()
    {
        GameObject[] shields = GameObject.FindGameObjectsWithTag("Shield");
        if (shields.Length != 0)
        {
            playerStatus.ChangePainFalse();
        } else
        {
            playerStatus.ChangePainTrue();
        }
    }
}
