using UnityEngine;


public class LevelSelectionManager : MonoBehaviour
{
    public void StartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");
    }    
}
