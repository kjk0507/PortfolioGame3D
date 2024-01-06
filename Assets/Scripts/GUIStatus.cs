using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPGSetting;
using Unity.VisualScripting;

public class GUIStatus : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> statusInfoList = new List<TextMeshProUGUI>();

    public enum E_STATUS_INFO { NAME, LEVEL, HP, FORTRESS, ATK, MONEY, STAGECLEAR1, STAGECLEAR2, STAGECLEAR3 }

    public void SettingStatus(Status player, FortressStatus fortress)
    {
        statusInfoList[(int)E_STATUS_INFO.NAME].text = player.name;
        statusInfoList[(int)E_STATUS_INFO.LEVEL].text = "LV : 임시";
        statusInfoList[(int)E_STATUS_INFO.HP].text = "체력 : " + player.curHp + " / " + player.maxHp;
        statusInfoList[(int)E_STATUS_INFO.FORTRESS].text = "요세 추가 체력 : " + fortress.plusHP;
        statusInfoList[(int)E_STATUS_INFO.ATK].text = "무기 공격력 : 임시";
        statusInfoList[(int)E_STATUS_INFO.MONEY].text = "소지금 : " + player.money;
        statusInfoList[(int)E_STATUS_INFO.STAGECLEAR1].text = "점령지1 : " + (fortress.clearStage1 ? "점령(10초당 10골드 획득)" : "미점령");
        statusInfoList[(int)E_STATUS_INFO.STAGECLEAR2].text = "점령지2 : " + (fortress.clearStage1 ? "점령(10초당 20골드 획득)" : "미점령");
        statusInfoList[(int)E_STATUS_INFO.STAGECLEAR3].text = "점령지3 : " + (fortress.clearStage1 ? "점령(10초당 30골드 획득)" : "미점령");
    }
}
