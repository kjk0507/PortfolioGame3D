using RPGSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager m_cInstance;
    public GameObject m_comment;
    public Coroutine fadeOutCoroutine;

    public List<GameObject> m_hpList; // hp 스테이터스 리스트
    public List<GameObject> m_hpList2; // 요세 HP 스테이터스 리스트
    public List<GameObject> m_listGUIScenes; // GUI 리스트
    public GameObject m_inventory;  // 인벤토리 GUI
    public GUIInventory m_guiInventory;  // 인벤토리 하위 레이어
    public GUISkill m_guiSkillList;
    public GameObject m_store; // store 상위 GUI
    public GUIStoreBuy m_guiStoreBuy;
    public GUIItemSell m_guiItemSell;


    public enum E_GUI_STATE { TITLE, PLAY, GAMEOVER, THEEND, PLAY_WAVE, STORE }  // GUI 상태
    public E_GUI_STATE m_curGUIState;
    bool isDeath = false;

    // 소모품 관련(play)
    public TextMeshProUGUI m_batteryNum;
    public TextMeshProUGUI m_bombNum;
    public TextMeshProUGUI m_goldNum;

    // 소모품 관련(play wave)
    public TextMeshProUGUI m_wave_batteryNum;
    public TextMeshProUGUI m_wave_bombNum;
    public TextMeshProUGUI m_wave_goldNum;

    // 소모품 관련(play store)
    public TextMeshProUGUI m_store_batteryNum;
    public TextMeshProUGUI m_store_bombNum;
    public TextMeshProUGUI m_store_goldNum;

    // wave 관련
    public bool isPlayWave = false;  // wave 상태여부
    public GameObject activeFortress; // 활성화된 요새
    public GameObject activePlayer;   // 활성화된 플레이어
    bool isTimerStart = false;
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI enemyNum;
    public float countdownDuration = 60f;  // wave 시간
    private float currentTime;
    public bool isAttackCheck = false;
    public bool isDragonAttak = false;
    private bool isWaveCoroutineRun = false;

    // 상태창, 인벤토리, 스킬
    [SerializeField] ItemManager m_cItemManager = new ItemManager();  // 모든 아이템 리스트가 포함
    [SerializeField] SkillManager m_skillManager = new SkillManager();  // 모든 스킬 리스트
    public Status playerStatus = new Status();
    public SkillInfo playerSkill = new SkillInfo();
    public FortressStatus playerFortessStatus = new FortressStatus();


    public List<GameObject> popupGUIList = new List<GameObject>();
    public enum E_GUI_POPUP { STATUS, INVENTORY, SKILL}
    public GUIStatus m_guiStatus;

    public List<GameObject> StoreGUIList = new List<GameObject>();
    public enum E_GUI_STORE { BUY, SELL}

    private void Awake()
    {
        m_cInstance = this;
        m_cItemManager.Init();
        m_cItemManager.SetPlayerAllData(playerStatus);
        m_skillManager.Init();
        m_skillManager.SetPlayerAllData(playerStatus);
        playerStatus.name = "Player";
        playerStatus.money = 10000;
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
        CheckDragonAttack();
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
        CheckConsumNum();
    }

    public void CheckConsumNum()
    {
        m_batteryNum.text = FindItemNum("IC_01").ToString();
        m_bombNum.text = FindItemNum("IC_02").ToString();
        m_goldNum.text = playerStatus.money.ToString();

        m_wave_batteryNum.text = FindItemNum("IC_01").ToString();
        m_wave_bombNum.text = FindItemNum("IC_02").ToString();
        m_wave_goldNum.text = playerStatus.money.ToString();

        m_store_batteryNum.text = FindItemNum("IC_01").ToString();
        m_store_bombNum.text = FindItemNum("IC_02").ToString();
        m_store_goldNum.text = playerStatus.money.ToString();
    }

    public int FindItemNum(string itemCode)
    {
        int num = 0;

        if(playerStatus.inventory != null)
        {
            for(int i = 0; i < playerStatus.inventory.Count; i++)
            {
                if (playerStatus.inventory[i].itemCode == itemCode)
                {
                    num = playerStatus.inventory[i].num;
                    break;
                }
            }
        }

        return num;
    }

    public void UseItem(string itemCode)
    {
        if (playerStatus.inventory != null)
        {
            for (int i = 0; i < playerStatus.inventory.Count; i++)
            {
                if (playerStatus.inventory[i].itemCode == itemCode)
                {
                    playerStatus.inventory[i].num--;
                    break;
                }
            }
        }
    }

    public bool FindSkillActive(string skillCode)
    {
        bool result = false;

        if(playerStatus.skillList != null)
        {
            for(int i = 0; i < playerStatus.skillList.Count; i++)
            {
                if (playerStatus.skillList[i].skillCode == skillCode)
                {
                    if (playerStatus.skillList[i].isHave)
                    {
                        result = true;
                    }
                }
            }
        }
        return result;
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
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    foreach (GameObject popup in popupGUIList)
                    {
                        popup.SetActive(false);
                    }
                    m_guiInventory.ResetIventoryButton();
                    m_guiSkillList.ResetSkillList();
                }
                break;
            case E_GUI_STATE.STORE:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    LeaveStore();
                }
                break;
        }
    }

    public void VisitStore()
    {
        // 다른 스테이터스로 변경하여 아예 다른 버튼 안눌리게 변경
        m_curGUIState = E_GUI_STATE.STORE;
        m_store.SetActive(true);
        foreach(GameObject store in StoreGUIList)
        {
            store.SetActive(false);
        }
        StoreGUIList[(int)E_GUI_STORE.BUY].SetActive(true);
        RestoreStore();
    }

    public void LeaveStore()
    {
        m_curGUIState = E_GUI_STATE.PLAY;
        m_store.SetActive(false);
    }

    public void RestoreStore()
    {
        m_guiStoreBuy.ResetStoreButton(); // 구매 버튼 초기화
        m_guiStoreBuy.ResetStoreInfo();   // 구매 정보 초기화
        m_guiStoreBuy.SetSkillStore(playerStatus); // 스킬 판매 버튼생성
        m_guiStoreBuy.SetItemStore(m_cItemManager); // 아이템 판매 버튼생성
        //Debug.Log("전체 : " + m_cItemManager.m_allItemList.Count);   //  이거 왜 주소를 공유함?
        //Debug.Log("플레이어 : " + playerStatus.inventory.Count);
        m_guiItemSell.ResetItemList();
        m_guiItemSell.SetItem(playerStatus);

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
                Time.timeScale = 1;
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
            m_guiSkillList.SetSkill(playerStatus);
            m_guiStatus.SettingStatus(playerStatus, playerFortessStatus);
            foreach (GameObject popup in popupGUIList)
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
            m_guiSkillList.ResetSkillList();
        }
    }

    public void PopupStatus()
    {
        if (popupGUIList[(int)E_GUI_POPUP.STATUS].activeSelf == false)
        {
            m_guiInventory.SetIventory(playerStatus);
            m_guiSkillList.SetSkill(playerStatus);
            m_guiStatus.SettingStatus(playerStatus, playerFortessStatus);
            foreach (GameObject popup in popupGUIList)
            {
                popup.SetActive(false);
            }
            popupGUIList[(int)E_GUI_POPUP.STATUS].SetActive(true);
        }
        else
        {
            foreach (GameObject popup in popupGUIList)
            {
                popup.SetActive(false);
            }
            m_guiInventory.ResetIventoryButton();
            m_guiSkillList.ResetSkillList();
        }
    }

    public void PopupSkill()
    {
        if (popupGUIList[(int)E_GUI_POPUP.SKILL].activeSelf == false)
        {
            m_guiInventory.SetIventory(playerStatus);
            m_guiSkillList.SetSkill(playerStatus);
            m_guiStatus.SettingStatus(playerStatus, playerFortessStatus);
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
            m_guiInventory.ResetIventoryButton();
            m_guiSkillList.ResetSkillList();
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
        // 포폴용 게임 엔딩으로 임시 변경
        //m_curGUIState = E_GUI_STATE.PLAY;
        //activeFortress.GetComponent<FortressControl>().ChangeFortressStatus();
        //playerStatus.maxHp = playerStatus.maxHp - playerFortessStatus.plusHP;
        //playerStatus.curHp = playerStatus.maxHp;
        m_curGUIState = E_GUI_STATE.THEEND;
        Invoke("LoadTitleScene", 2f);
    }

    public void WaveStatus()
    {
        if (isTimerStart)
        {
            int maxNum = activeFortress.GetComponent<FortressControl>().m_maxEnemy;
            int curNum = activeFortress.GetComponent<FortressControl>().m_curEnemy;
            string enemyCountText = curNum.ToString() + " / " + maxNum.ToString();
            enemyNum.text = enemyCountText;

            if (curNum == 0 && !isWaveCoroutineRun)
            {
                StartCoroutine(CheckEnemyNum());
            }
        }        
    }

    public IEnumerator CheckEnemyNum()
    {
        isWaveCoroutineRun = true;
        yield return new WaitForSeconds(2f);
        isWaveCoroutineRun = false;

        int curNum = activeFortress.GetComponent<FortressControl>().m_curEnemy;

        if (curNum == 0)
        {
            isTimerStart = false;
            ChangeFortressMode();
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

    public void SetComment(string comment)
    {
        m_comment.gameObject.SetActive(true);
        TextMeshProUGUI textMeshPro = m_comment.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.text = comment;

        if (fadeOutCoroutine != null)
        {
            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 1f);
            StopCoroutine(fadeOutCoroutine);
        }

        fadeOutCoroutine = StartCoroutine(FadeOutText(textMeshPro));
    }

    private IEnumerator FadeOutText(TextMeshProUGUI textMeshPro)
    {
        float fadeOutTime = 1f;

        float startAlpha = textMeshPro.color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeOutTime)
        {
            Color newColor = textMeshPro.color;
            newColor.a = Mathf.Lerp(startAlpha, 0.0f, t);
            textMeshPro.color = newColor;
            yield return null;
        }
        m_comment.gameObject.SetActive(false);
    }

    private void CheckDragonAttack()
    {
        if(isDragonAttak && !isAttackCheck)
        {
            isAttackCheck = true;
            Invoke("ChangeDragonAttack", 1f);
        }
    }

    private void ChangeDragonAttack()
    {
        isDragonAttak = false;
        isAttackCheck = false;
    }
}
