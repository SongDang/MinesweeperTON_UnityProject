using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnitonConnect.Core.Utils.Debugging;
using UnitonConnect.Core;

public class Booster : MonoBehaviour
{
    public GameObject laserPrefab;
    public TextMeshProUGUI resultText;

    public void AutoLaser()
    {
        /*if (Game.Instance._userDatas.diamond >= 5)
        {
            Game.Instance._userDatas.diamond -= 5;
        }
        else
        {
            resultText.text = "Not enough diamond";
            resultText.gameObject.SetActive(true);
            Invoke("removeResult", 1f);
            return;
        }*/

        if (Game.Instance.GetIsFlooding())
            return;

        int.TryParse(PlayerStatsManager.Instance.laser, out int currentLaser);
        if (currentLaser <= 0)
        {
            UnitonConnectLogger.Log("Not enough laser");
            //pop up
            return;
        }

        //loading

        UnitonConnectSDK.Instance.OnTonTransactionSended += OnTxSuccess;
        UnitonConnectSDK.Instance.OnTonTransactionSendFailed += OnTxFailed;

        string jsonParams = "{\"qty\": 1}"; //1 item
        UnitonConnectSDK.Instance.SendSmartContractTransaction("use_laser", jsonParams);
    }

    private void OnTxFailed(string errorMsg)
    {
        UnitonConnectLogger.LogError("Use laser failed: " + errorMsg);

        Cleanup();
    }

    private void OnTxSuccess(string transactionHash)
    {
        UnitonConnectLogger.Log("Use laser success, hash: " + transactionHash);

        Cleanup();

        //PlayerStatsManager.Instance.heart--; 
        ActiveLaser();
    }

    public void removeResult()
    {
        resultText.gameObject.SetActive(false);
    }

    private void Cleanup()
    {
        //cancel loading pop up;

        if (UnitonConnectSDK.Instance != null)
        {
            UnitonConnectSDK.Instance.OnTonTransactionSended -= OnTxSuccess;
            UnitonConnectSDK.Instance.OnTonTransactionSendFailed -= OnTxFailed;
        }
    }
    private void OnDestroy()
    {
        Cleanup();
    }

    private void ActiveLaser()
    {
        resultText.text = "Laser On!";
        resultText.gameObject.SetActive(true);
        Invoke("removeResult", 1f);

        int width = Game.Instance.width;
        int height = Game.Instance.height;

        List<Cell> listCanDigCell = new List<Cell>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = Game.Instance.grid.cells[x, y];

                if (cell.type != Cell.Type.Mine && cell.type != Cell.Type.Block && cell.revealed == false)
                {
                    listCanDigCell.Add(cell);
                }
            }
        }
        if (listCanDigCell.Count == 0) return;

        Cell randomCell = listCanDigCell[Random.Range(0, listCanDigCell.Count)];
        if (!Game.Instance.generated)
        {
            Game.Instance.grid.GenerateMines(randomCell, Game.Instance.mineCount);
            Game.Instance.grid.GenerateNumbers();
            Game.Instance.generated = true;
        }
        GameObject laserEffect = Instantiate(laserPrefab, randomCell.position + new Vector3(0.5f, 0.5f), Quaternion.identity);

        //Kiem tra neu hieu ung kich hoat ben ngoai camera thi dich chuyen toi do
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(laserEffect.transform.position);
        if (viewportPosition.x <= 0f || viewportPosition.x >= 1f || viewportPosition.y <= 0f || viewportPosition.y >= 0.9f)
            Camera.main.transform.position = new Vector3(laserEffect.transform.position.x, laserEffect.transform.position.y, Camera.main.transform.position.z);

        Destroy(laserEffect, 0.5f);
        Game.Instance.Reveal(randomCell);

        Game.Instance.SaveData();
    }
}
