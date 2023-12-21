using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // 정령 관련
        public int maxFp;
        public int curFp;
        public int equipFp;
        // 공격 관련
        public int curAtk;
        public int equipAtk;


        // HP, MP, 정령 수치 불러오기
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

        // 데미지 받는 경우
        public void Demeged(int demage)
        {
            this.curHp -= demage;
            if (this.curHp < 0)
            {
                this.curHp = 0;
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
    }

    [Serializable]
    public class Item
    {
        public String type; // 소모품, 장비품, 기타
        public String itemCode;
        public String name;
        public String icon;
        public int num; // 수량
        public String explain;
        public int money;
        public int plusAttack;
        public int plusDefence;

        public Item(string type, string itemCode, string name, int num, string explain, int money)
        {
            this.type = type;
            this.itemCode = itemCode;
            this.name = name;
            this.num = num;
            this.explain = explain;
            this.money = money;
        }
    }
}
