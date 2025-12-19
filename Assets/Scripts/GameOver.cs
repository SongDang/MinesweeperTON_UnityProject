using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public void PlayAgain()
    {
        if(Game.Instance.OutOfHeartScreen.activeSelf == false)
        {
            if (Game.Instance._userDatas.heart > 0)
            {
                Game.Instance.NewGame();
                gameObject.SetActive(false);
            }
            else
            {
                Game.Instance.OutOfHeartScreen.SetActive(true);
            }
        }    
        else
        {
            if(Game.Instance._userDatas.diamond >= 1)
            {
                Game.Instance.NewGame();
                Game.Instance.OutOfHeartScreen.SetActive(false);
                gameObject.SetActive(false);
                Game.Instance._userDatas.diamond--;
                Game.Instance._userDatas.heart++;
                Game.Instance.SaveData();
            }   
        }    
    }
    public void NextLevel()
    {
        Game.Instance.SetLevel();
        Game.Instance.NewGame();
        gameObject.SetActive(false);
    }    


}
