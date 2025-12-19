using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
{
    public GameObject bagscreen;
    
    public void OpenBag()
    {
        bagscreen.SetActive(true);
    }    

    public void CloseBag()
    {
        bagscreen.SetActive(false);
    }    


}
