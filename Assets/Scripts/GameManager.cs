using RPGSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject m_player;
    public List<GameObject> m_hpList; // hp 스테이터스 리스트
    public List<GameObject> m_listGUIScenes; // GUI 리스트
    public GameObject m_inventory;
    public enum E_GUI_STATE { TITLE, PLAY, GAMEOVER, THEEND }

    [SerializeField] ItemManager m_cItemManager = new ItemManager();
    public ItemManager ItemManager { get { return m_cItemManager; } }

    public E_GUI_STATE m_curGUIState;
    static GameManager m_cInstance;

    bool isDeath = false;
    bool isActiveInventory = false;

    public GUIInventory m_guiInventory;

    private void Awake()
    {
        m_cInstance = this;
        m_cItemManager.Init();
        m_cItemManager.SetPlayerAllData(m_player.GetComponent<PlayerControl>().m_status);
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
    }

    void EventUpdateStatus()
    {        
        if (m_player != null)
        {
            SetHPBar(m_player.GetComponent<PlayerControl>().m_status.GetCurHp(), m_player.GetComponent<PlayerControl>().m_status.GetFinalHp());
            if (!isDeath && m_player.GetComponent<PlayerControl>().m_status.IsDeath())
            {
                isDeath = true;
                SetGUIScene(E_GUI_STATE.GAMEOVER);

                Invoke("LoadTitleScene", 2f);
            }
        }
    }

    void SetHPBar(float cur, float max)
    {
        for (int i = 0; i < cur; i++)
        {
            m_hpList[i].SetActive(true);
        }

        for (int j = (int)cur; j < m_hpList.Count; j++)
        {
            m_hpList[j].SetActive(false);
        }
    }

    void UpdateState()
    {
        switch (m_curGUIState)
        {
            case E_GUI_STATE.TITLE:
                break;
            case E_GUI_STATE.THEEND:
                break;
            case E_GUI_STATE.GAMEOVER:
                break;
            case E_GUI_STATE.PLAY:
                EventUpdateStatus();
                if (Input.GetKeyDown(KeyCode.I))
                {
                    PopupIventroy();
                }

                break;
        }
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
            m_guiInventory.SetIventory(m_player.GetComponent<PlayerControl>().m_status);
            m_inventory.SetActive(true);
        }
        else
        {
            m_inventory.SetActive(false);
            m_guiInventory.ResetIventoryButton();
        }
    }
}
