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
        level = 3;
        gold = 0;
        diamond = 0;
        maxheart = 2;
        heart = 2;
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
