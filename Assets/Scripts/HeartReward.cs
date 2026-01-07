using UnityEngine;
using UnitonConnect.Core;
using UnitonConnect.Core.Utils.Debugging;
using TMPro;
using UnityEngine.UI;
using System; 

public class HeartReward : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public Button getButton;

    // 24h (24 * 60 * 60)
    private const long COOLDOWN_SECONDS = 86400;

    private void OnEnable()
    {
        UpdateUIState();
    }

    private void UpdateUIState()
    {
        long lastTime = PlayerStatsManager.Instance.lastTimeReceiveHeart;
        long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();

        long nextClaimTime = lastTime + COOLDOWN_SECONDS;

        if (lastTime == 0 || currentTime >= nextClaimTime)
        {
            string heartReward = (2 + PlayerStatsManager.Instance.levelHeart).ToString();
            timeText.text = "Your gift is ready! +" + heartReward;
            getButton.interactable = true;
        }
        else
        {
            getButton.interactable = false;

            DateTime nextDate = DateTimeOffset.FromUnixTimeSeconds(nextClaimTime).LocalDateTime;

            timeText.text = $"Come back at\n{nextDate.ToString("HH:mm dd/MM/yyyy")}";
        }
    }

    public void OpenHeartReward()
    {
        long lastTime = PlayerStatsManager.Instance.lastTimeReceiveHeart;
        long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();

        if (lastTime != 0 && currentTime < lastTime + COOLDOWN_SECONDS)
        {
            Debug.LogWarning("cooldown!");
            UpdateUIState();
            return;
        }

        GetHeartReward();
    }

    public void GetHeartReward()
    {
        Debug.Log("Sending Transaction...");
        getButton.interactable = false; 

        string jsonParams = "{\"amountHeart\": 1}"; //dummy, contract auto give heart based on level
        UnitonConnectSDK.Instance.SendSmartContractTransaction(HandleGetHeartRewardResult, "heart_reward", jsonParams);
    }

    private void HandleGetHeartRewardResult(bool isSuccess)
    {
        if (isSuccess)
        {
            int heartCount = 2 + PlayerStatsManager.Instance.levelHeart;
            PlayerStatsManager.Instance.AddHeart(heartCount);

            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            PlayerStatsManager.Instance.SetlastTimeReceiveHeart((int)now);

            UnitonConnectLogger.Log($"Success! Next claim allowed at: {now + COOLDOWN_SECONDS}");

            UpdateUIState();
        }
        else
        {
            UnitonConnectLogger.Log("Heart Reward Result Failed.");
            UpdateUIState();
        }
    }
}