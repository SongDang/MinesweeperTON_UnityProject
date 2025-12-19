using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDatas 
{
    public int level;
    public int gold;
    public int diamond;
    public int maxheart;
    public int heart;
    public int timeheart;
    public float timedig;
    public int probalitygold;
    public int probalitydiamond;

    public bool skill1;
    public bool skill2;
    public bool skill3;
    public bool skill4;
    public bool skill5;
    public bool skill6;
    public UserDatas()
    {
        Init();
    }

    public void Init()
    {
        level = 5;
        gold = 9999;
        diamond = 9999;
        maxheart = 3;
        heart = 3;
        timedig = 1.5f;
        probalitygold = 15;
        probalitydiamond = 4;
        skill1 = true;
        skill2 = false;
        skill3 = true;
        skill4 = false;
        skill5 = true;
        skill6 = false;
    }
}
