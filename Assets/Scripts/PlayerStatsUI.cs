using UnityEngine;
using TMPro; 

public class PlayerStatsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI heartText;
    [SerializeField] private TextMeshProUGUI laserText;

    private void OnEnable()
    {
        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.OnHeartUpdated += UpdateHeartDisplay;
            PlayerStatsManager.Instance.OnLaserUpdated += UpdateLaserDisplay;

            UpdateHeartDisplay(PlayerStatsManager.Instance.heart);
            UpdateLaserDisplay(PlayerStatsManager.Instance.laser);
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

    private void UpdateHeartDisplay(string heart)
    {
        if (heartText != null) heartText.text = heart;
    }
    private void UpdateLaserDisplay(string laser)
    {
        if (laserText != null) laserText.text = laser;
    }
}