using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGSetting
{
    [Serializable]
    public class Status
    {
        public string name;
        // HP 관련
        public int maxHp;
        public int curHp;
        public int equipHp;
        // MP 관련 -> 총알 수로 하는건 어떨까?
        public int maxMp;
        public int curMp;
        public int equipMp;
        // 요새 관련
        public int maxFp;
        public int curFp;
        public int equipFp;
        // 공격 관련
        public int curAtk;
        public int equipAtk;
        // 인벤토리 관련
        public List<Item> inventory = new List<Item>();
        // 스킬 관련
        public List<Skill> skillList = new List<Skill>();
        // 소지금 관련
        public int money;
        // 상태이상 관련
        public bool isPain = true;  // 데미지 받는 여부

        // HP, MP, 요새 수치 불러오기
        public int GetFinalHp()
        {
            return maxHp + equipHp;
        }

        public int GetCurHp()
        {
            return curHp;
        }

        public int GetFinalMp()
        {
            return maxMp + equipMp;
        }

        public int GetCurMp()
        {
            return curMp;
        }

        public int GetFinalFp()
        {
            return maxFp + equipFp;
        }

        public int GetCurFp()
        {
            return curFp;
        }

        public int GetFinalAtk()
        {
            return curAtk + equipAtk;
        }

        // 생성자
        public Status(string name = "", int maxHp = 5, int curHp = 5, int curAtk = 1)
        {
            this.name = name;
            this.maxHp = maxHp;
            this.curHp = curHp;
            this.curAtk = curAtk;
        }

        // 체력 관련
        // 데미지 받는 경우
        public void Demeged(int demage)
        {
            if (isPain)
            {
                this.curHp -= demage;
                if (this.curHp < 0)
                {
                    this.curHp = 0;
                }
            }
        }

        // 회복하는 경우
        public void Healing(int healing)
        {
            int finalHp = GetFinalHp();
            this.curHp += healing;
            if (finalHp < curHp)
            {
                this.curHp = finalHp;
            }
        }

        public bool IsDeath()
        {
            if (this.curHp > 0)
                return false;
            else
                return true;
        }

        // 무적 상태 변환
        public void ChangePainTrue()
        {
            this.isPain = true;
        }

        public void ChangePainFalse()
        {
            this.isPain = false;
        }

        // 돈 증감
        public void GetMoney(int num)
        {
            this.money += num;
        }

        public void loseMoney(int num)
        {
            this.money -= num;
        }

        // 아이템 증감
        public void GetItem(Item getItem, int num)
        {
            bool ishave = false;

            foreach(Item item in this.inventory)
            {
                if(item.itemCode == getItem.itemCode)
                {
                    ishave = true;
                    item.num++;
                    break;
                }
            }

            if (!ishave)
            {
                this.inventory.Add(getItem);
            }
        }  
    }

    public class FortressStatus
    {
        // 체력 보너스 관련
        public int plusHP = 5;

        // 속도 관련
        public float rotationSpeed = 20f;
        public float fireSpeed;

        // 데미지 관련
        public float demage;

        // 터렛 관련
        public float turretSpeed;
        public float tureetFireNum;

        // 해금 정보
        public bool activeTurret1 = false;
        public bool activeTurret2 = false;
        public bool activeTurret3 = false;
        public bool activeTurret4 = false;

        // 클리어 여부
        public bool clearStage1 = false;
        public bool clearStage2 = false;
        public bool clearStage3 = false;
    }

    public class SkillInfo
    {
        public bool activeSkill1 = false; // 실드
        public bool activeSkill2 = false; // 회복
        public bool activeSkill3 = false; // 포격
    }

    [Serializable]
    public class Skill
    {
        public String type;
        public String modeType;
        public String skillCode;
        public String name;
        public String icon;
        public bool isHave;
        public String explain;
        public int money;
        public String dropLocation;
        public float coolDown;
        public String pressKey;
        public String Require;

        public Skill(string type, string modeType, string skillCode, string name, string icon, bool isHave, string explain, int money, string dropLocation, float coolDown, string pressKey, string Require)
        {
            this.type = type;
            this.modeType = modeType;
            this.skillCode = skillCode;
            this.name = name;
            this.icon = icon;
            this.isHave = isHave;
            this.explain = explain;
            this.money = money;
            this.dropLocation = dropLocation;
            this.coolDown = coolDown;
            this.pressKey = pressKey;
            this.Require = Require;
        }
    }

    public class SkillManager
    {
        List<Skill> m_listSkillManager = new List<Skill>();
        public void Init()
        {
            m_listSkillManager.Add(new Skill("Active", "both" , "SA_01", "실드", "a_barrier", false, "베리어 입니다.", 1000, "A", 5f, "B", "보석 1"));
            m_listSkillManager.Add(new Skill("Active", "both", "SA_02", "회복", "a_repair", false, "회복입니다.", 2000, "A", 5f, "C", "보석 1")); 
            m_listSkillManager.Add(new Skill("Active", "fortress","SA_03", "포격", "a_bomb", false, "저장해둔 포탄을 발사합니다.", 1000, "A", 5f, "X", "보석 2"));
            m_listSkillManager.Add(new Skill("Active", "fortress", "SA_04", "상점포격", "a_bombMoney", false, "상점에서 포탄을 발사합니다. 골드가 소모됩니다.", 2000, "A", 5f, "D", "보석 1"));
            m_listSkillManager.Add(new Skill("Pasive", "player", "SP_01", "이단점프", "p_dubbleJump", false, "이단 점프가 가능합니다.", 1000, "A", 0f, "없음", "보석 1"));
        }

        public void SetPlayerAllData(Status player)
        {
            player.skillList = m_listSkillManager;
        }

        public Skill FindSkill(Status player, string skillCode)
        {
            Skill findSkill = null;

            foreach(Skill skill in player.skillList)
            {
                if(skill.skillCode == skillCode)
                {
                    findSkill = skill;
                    break;
                }
            }

            return findSkill;
        }

        public bool FindSkillTrue(Status player, string skillCode)
        {
            Skill findSkill = null;

            foreach (Skill skill in player.skillList)
            {
                if (skill.skillCode == skillCode)
                {
                    findSkill = skill;
                    break;
                }
            }

            if(findSkill == null)
            {
                return false;
            }
            else
            {
                if(findSkill.isHave){
                    return true;
                }                
            }

            return false ;
        }
    }

    [Serializable]
    public class Item
    {
        public String type; // 소모품, 장비품, 기타 consum, equip, etc
        public String itemCode;
        public String name;
        public String icon;
        public int num; // 수량
        public String dropLocation;
        public String explain;
        public int money;
        public int plusAttack;
        public int plusDefence;

        public Item(string type, string itemCode, string name, string icon, int num, string dropLocation , string explain, int money)
        {
            this.type = type;
            this.itemCode = itemCode;
            this.name = name;
            this.icon = icon;
            this.num = num;
            this.dropLocation = dropLocation;
            this.explain = explain;
            this.money = money;
        }
    }

    public class ItemManager
    {
        public List<Item> m_listItemManager = new List<Item>();
        public List<Item> m_allItemList = new List<Item>();

        public void Init()
        {
            m_listItemManager.Add(new Item("consum", "IC_01", "실드 배터리", "c_battery", 1, "상점 구매","실드를 시용할 때 마다 하나씩 소모된다.", 500));
            m_listItemManager.Add(new Item("consum", "IC_02", "포탄", "c_bomb", 1, "상점 구매", "요새 모드에서 포탄을 사용할 때 마다 하나씩 소모된다.", 1000));
            m_listItemManager.Add(new Item("etc", "IE_01", "뼈", "e_bone", 1, "상점 구매", "스켈레톤을 쓰러뜨리고 얻은 물건이다.", 10));
            m_listItemManager.Add(new Item("etc", "IE_02", "붉은 보석", "e_redgem", 1, "상점 구매", "드래곤을 쓰러뜨리고 얻은 물건이다.", 2000));

            m_allItemList.Add(new Item("consum", "IC_01", "실드 배터리", "c_battery", 1, "상점 구매", "실드를 시용할 때 마다 하나씩 소모된다.", 500));
            m_allItemList.Add(new Item("consum", "IC_02", "포탄", "c_bomb", 1, "상점 구매", "요새 모드에서 포탄을 사용할 때 마다 하나씩 소모된다.", 1000));
            m_allItemList.Add(new Item("etc", "IE_01", "뼈", "e_bone", 1, "상점 구매", "스켈레톤을 쓰러뜨리고 얻은 물건이다.", 10));
            m_allItemList.Add(new Item("etc", "IE_02", "붉은 보석", "e_redgem", 1, "상점 구매", "드래곤을 쓰러뜨리고 얻은 물건이다.", 2000));

            //m_listItemManager.Add(new Item("etc", "IE_01", "test1", "axe", 1, "A", "test1", 10));
            //m_listItemManager.Add(new Item("etc", "IE_02", "test2", "bag", 1, "A", "test1", 10));
            //m_listItemManager.Add(new Item("etc", "IE_03", "test3", "boots", 1, "A", "test1", 10));
            //m_listItemManager.Add(new Item("etc", "IE_04", "test4", "cloaks", 1, "A", "test1", 10));

        }

        public void SetPlayerAllData(Status player)
        {
            player.inventory = m_listItemManager;
        }
    }
}
