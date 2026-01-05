using UnityEngine;
using TMPro; 

public class PlayerStatsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI heartText;
    [SerializeField] private TextMeshProUGUI laserText;

    private void Start()
    {
        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.OnHeartUpdated += UpdateHeartDisplay;
            PlayerStatsManager.Instance.OnLaserUpdated += UpdateLaserDisplay;

            UpdateHeartDisplay(PlayerStatsManager.Instance.heart);
            UpdateLaserDisplay(PlayerStatsManager.Instance.laser);
        }
        else
        {
            Debug.Log("PlayerStatsManager null");
        }    
    }

    private void OnDisable()
    {
        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.OnHeartUpdated -= UpdateHeartDisplay;
            PlayerStatsManager.Instance.OnLaserUpdated -= UpdateLaserDisplay;
        }
    }

    private void UpdateHeartDisplay(int heart)
    {        
        heartText.text = heart.ToString();
        Debug.Log("update heart: " + heart);
    }
    private void UpdateLaserDisplay(int laser)
    {
        laserText.text = laser.ToString();
        Debug.Log("update laser: " + laser);
    }
}