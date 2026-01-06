using UnityEngine;
using TMPro;

public class UpdateLiveText : MonoBehaviour
{
    public TextMeshProUGUI heartText;
    private void Start()
    {
        UpdateHeartText(PlayerStatsManager.Instance.heart);
        PlayerStatsManager.Instance.OnHeartUpdated += UpdateHeartText;
    }
    public void UpdateHeartText(int count)
    {
        heartText.text = count.ToString();
    }
    private void OnDestroy()
    {
        PlayerStatsManager.Instance.OnHeartUpdated -= UpdateHeartText;
    }
}
