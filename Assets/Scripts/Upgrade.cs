using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    public TextMeshProUGUI textgold;
    public TextMeshProUGUI textdiamond;
    public TextMeshProUGUI textinfor;
    private int whattodo = 0;
    public int costgold = 0;
    public int costdiamond = 0;
    public Button dig2;
    public Button heart2;
    public Button ore2;
    public Button dig1;
    public Button heart1;
    public Button ore1;

    public void ConfirmUpgrade()
    {
        
        AdjustCost(whattodo);
        if (Game.Instance._userDatas.gold >= costgold && Game.Instance._userDatas.diamond >= costdiamond)
        {
            AudioManager.Instance.WinSound();
            DoUpgrade(whattodo);
            Game.Instance._userDatas.gold -= costgold;
            Game.Instance._userDatas.diamond -= costdiamond;            
            whattodo = 0;
            Game.Instance.SaveData();
        }     
    }    


    public void AdjustCost(int i)
    {
        if(whattodo == 0)
        {
            costgold = 0;
            costdiamond = 0;
        }    
        if (whattodo == 1)
        {
            costgold = 100;
            costdiamond = 0;
        }
        if (whattodo == 2)
        {
            costgold = 200;
            costdiamond = 5;
        }
        if (whattodo == 3)
        {
            costgold = 500;
            costdiamond = 10;
        }
        if (whattodo == 4)
        {
            costgold = 1000;
            costdiamond = 20;
        }
        if (whattodo == 5)
        {
            costgold = 500;
            costdiamond = 10;
        }
        if (whattodo == 6)
        {
            costgold = 1000;
            costdiamond = 20;
        }
    }    

    public void DoUpgrade(int i)
    {   
        if(whattodo == 1)
        {
            costgold = 100;
            costdiamond = 0;
            Game.Instance._userDatas.timedig -= 0.5f;
            dig2.interactable = true;
            dig1.interactable = false;
            Game.Instance._userDatas.skill1 = false;
            Game.Instance._userDatas.skill2 = true;
        }
        if (whattodo == 2)
        {
            costgold = 200;
            costdiamond = 5;
            Game.Instance._userDatas.timedig -= 0.5f;
            dig2.interactable = false;
            Game.Instance._userDatas.skill2 = false;
        }
        if (whattodo == 3)
        {
            costgold = 500;
            costdiamond = 10;
            Game.Instance._userDatas.maxheart += 1;
            Game.Instance._userDatas.heart += 1;
            heart2.interactable = true;
            heart1.interactable = false;
            Game.Instance._userDatas.skill3 = false;
            Game.Instance._userDatas.skill4 = true;
        }
        if (whattodo == 4)
        {
            costgold = 1000;
            costdiamond = 20;
            Game.Instance._userDatas.maxheart += 1;
            Game.Instance._userDatas.heart += 1;
            heart2.interactable = false;
            Game.Instance._userDatas.skill4 = false;
        }
        if (whattodo == 5)
        {
            costgold = 500;
            costdiamond = 10;
            Game.Instance._userDatas.probalitygold += 10;
            ore2.interactable = true;
            ore1.interactable = false;
            Game.Instance._userDatas.skill5 = false;
            Game.Instance._userDatas.skill6 = true;
        }
        if (whattodo == 6)
        {
            costgold = 1000;
            costdiamond = 20;
            Game.Instance._userDatas.probalitygold += 5;
            Game.Instance._userDatas.probalitydiamond += 3;
            ore2.interactable = false;
            Game.Instance._userDatas.skill6 = false;
        }

    }

    public void Choose(int x)
    {
        whattodo = x;
    }

    public void AdjustInfor(int z)
    {
        
            if (textinfor != null)
        {
            if(z == 0)
            {
                textinfor.text = "Upgrade successful";
            }    
            if (z == 1)
            {
                textinfor.text = "Digging time: 2s -> 1.5s";
            }
            if (z == 2)
            {
                textinfor.text = "Digging time: 1.5s -> 1s";
            }
            if (z == 3)
            {
                textinfor.text = "Max hearts: 3 -> 4";
            }
            if (z == 4)
            {
                textinfor.text = "Max hearts: 4 -> 5";
            }
            if (z == 5)
            {
                textinfor.text = "Chance to collect gold +10%";
            }
            if (z == 6)
            {
                textinfor.text = "Chance to collect gold +5%, diamond +3%";
            }
        }
           
    }

    void Start()
    {
        this.textgold.text = Game.Instance._userDatas.gold.ToString();
        this.textdiamond.text = Game.Instance._userDatas.diamond.ToString();
        if(dig1 != null)
        { 
            dig1.interactable = Game.Instance._userDatas.skill1;
            dig2.interactable = Game.Instance._userDatas.skill2;
            heart1.interactable = Game.Instance._userDatas.skill3;
            heart2.interactable = Game.Instance._userDatas.skill4;
            ore1.interactable = Game.Instance._userDatas.skill5;
            ore2.interactable = Game.Instance._userDatas.skill6;
        }          
    }
    
    void Update()
    {
        textgold.text = Game.Instance._userDatas.gold.ToString();
        textdiamond.text = Game.Instance._userDatas.diamond.ToString();
        AdjustInfor(whattodo);
    }

}
