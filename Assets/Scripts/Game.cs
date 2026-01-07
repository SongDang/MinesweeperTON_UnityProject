using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using UnitonConnect.Core;
using UnitonConnect.Core.Utils.Debugging;

public class Game : MonoBehaviour
{
    #region Singleton
    private static Game _instance;
    public static Game Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        else
            _instance = this;

        board = GetComponentInChildren<Board>();

        if (PlayerPrefs.HasKey(DATA_KEY))
        {
            string savedJSON = PlayerPrefs.GetString(DATA_KEY);
            Debug.Log("Luu: " + savedJSON);
            this._userDatas = JsonUtility.FromJson<UserDatas>(savedJSON);
        }
        else
        {
            this._userDatas = new UserDatas();
            Debug.Log("Chua co Data luu: ");
        }
    }

    #endregion Singleton

    public World _world;

    public int width = 10;
    public int height = 7;
    public int mineCount = 10;
    public int rockCount = 4;

    public Board board;
    public CellGrid grid;
    private bool gameover;
    public bool generated;

    public TextMeshProUGUI textgold;
    public TextMeshProUGUI textdiamond;
    public TextMeshProUGUI textlevel;
    public TextMeshProUGUI textcomplete;
    public TextMeshProUGUI tonText;

    public GameObject goldPrefab;
    public GameObject diamondPrefab;
    

    public RectTransform bagIcon;
    public Vector3 initCameraPosition;

    public GameObject sweepscreen;
    public bool isImageVisible = false;
    public int sweepmode = -1;

    public GameObject gameoverscreen;
    public GameObject gamewinscreen;
    public GameObject rewardPopup;
    public GameObject surePopup;

    public UserDatas _userDatas;
    public const string DATA_KEY = "DATA_KEY";

    public GameObject UIDirtExplodePrefab;
    public GameObject UIMineExplodePrefab;
    public GameObject UIDirtDigPrefab;
    private bool ishaveuidig = false;

    public GameObject Bagscreen;
    public GameObject OutOfHeartScreen;

    public GameObject bugPrefab;

    private int goldCount = 0;
    private int diamondCount = 0;
    private int floodingCount;
    private decimal tonCoin = 0m;
    private bool isFlooding = false;
    private float timeDig;
    private float probalitygold;
    private float probalitydiamond;

    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }


    public void Start()
    {
        this.textgold.text = goldCount.ToString();
        this.textdiamond.text = diamondCount.ToString();
        NewGame();
    }

    public void NewGame()
    {
        /*if (Game.Instance._userDatas.heart <= 0) 
        {
            gameoverscreen.SetActive(true);
            OutOfHeartScreen.SetActive(true);
        }*/          
            
        StopAllCoroutines();
        if (_world != null)
        {
            if (_world.levels[_userDatas.level] != null)
            {
                width = _world.levels[_userDatas.level].width;
                height = _world.levels[_userDatas.level].height;
                mineCount = _world.levels[_userDatas.level].mineCount;
                rockCount = _world.levels[_userDatas.level].rockCount;
            }
        }
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        initCameraPosition = Camera.main.transform.position;

        gameover = false;
        generated = false;
        
        holdTime = 0f;
        timeDig = 2 - PlayerStatsManager.Instance.levelDig * 0.5f;
        probalitygold = 15 + 10 + 5 * PlayerStatsManager.Instance.levelOre;
        if (PlayerStatsManager.Instance.levelOre > 1)
            probalitydiamond = 4 + 3; //+3%
        else
            probalitydiamond = 4;

        UnitonConnectLogger.Log($"timeDig: {timeDig}, probalitygold: {probalitygold}, probalitydiamond: {probalitydiamond}");

        grid = new CellGrid(width, height);
        board.InitiateDraw(grid, rockCount);

        CameraPanAndZoom cameraController = FindObjectOfType<CameraPanAndZoom>();
        if (cameraController != null)
        {
            cameraController.ResetCamera();
        }
        else
        {
            Debug.LogError("No CameraPanAndZoom script found in the scene!");
        }
    }

    public void SetLevel()
    {
        Debug.Log("Skip logic set level");
        /*if(_userDatas.level < 14)
        _userDatas.level++;
        SaveData();*/
    } 
    
        

    private bool isMouseButtonDown = false;
    private float holdTime = 0f;
    
    private void Update()
    {
        if (!gameover)
        {
            Sweep();
            if(sweepmode == -1)
                RevealAndFlag();
            
        }
        AdjustText();
        CheckReward();
    }

    public void RevealAndFlag()
    {
        if (isFlooding) return;
        if (rewardPopup.activeSelf) return;

        if(EventSystem.current.currentSelectedGameObject == null && Bagscreen.activeSelf == false)
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                isMouseButtonDown = true;
                
                //dig
                
            }

            
            if (TryGetCellAtMousePosition(out Cell cell) && isMouseButtonDown == true)
            {
                if(ishaveuidig == false && cell.revealed == false)
                {
                    AudioManager.Instance.DigSound();
                    Vector3 centerCellPositiona = new Vector3(cell.position.x + 0.5f, cell.position.y + 0.5f, cell.position.z);
                    GameObject a = Instantiate(UIDirtDigPrefab, centerCellPositiona, Quaternion.identity);
                    Destroy(a, 0.3f);
                    Invoke("DestroyUIdig", 0.3f);
                    ishaveuidig = true;
                }               
            }
            

            if (Input.GetMouseButtonUp(0))
            {

                if (holdTime < /*_userDatas.timedig*/timeDig)
                {
                    Flag();
                }

                isMouseButtonDown = false;
                holdTime = 0f;

                ishaveuidig = false;
            }

            if (isMouseButtonDown)
            {
                holdTime += Time.deltaTime;
                if (holdTime >= /*_userDatas.timedig*/timeDig)
                {
                    Reveal();
                }
            }
        }    
    }
    
    public void DestroyUIdig()
    {
        ishaveuidig = false;
    }
        
    public void AdjustText() 
    {
        textgold.text = goldCount.ToString();
        textdiamond.text = diamondCount.ToString();

        tonText.text = tonCoin.ToString();
        //textheart.text = heart.ToString();
        //textlevel.text = _userDatas.level.ToString();
    }
        
    private void Reveal()
    {
        if (TryGetCellAtMousePosition(out Cell cell))
        {
            if (!generated)
            {
                grid.GenerateMines(cell, mineCount);
                grid.GenerateNumbers();
                generated = true;
            }
            Reveal(cell);
            
        }
        
    }
    
    public void Reveal(Cell cell)
    {
        
        if (cell.revealed) return;
        if (cell.flagged) return;
        Vector3 centerCellPositiona = new Vector3(cell.position.x + 0.5f, cell.position.y + 0.5f, cell.position.z);
        GameObject a = Instantiate(UIDirtExplodePrefab, centerCellPositiona, Quaternion.identity);
        Destroy(a, 1.5f);

        AudioManager.Instance.RockSmashSound();

        switch (cell.type)
        {
            case Cell.Type.Mine:

                Explode(cell);
                break;

            case Cell.Type.Empty:
                StartCoroutine(Flood(cell));
                CollectOre(cell);
                CheckWinCondition();
                break;

            default:
                cell.revealed = true;
                CollectOre(cell);

                CheckWinCondition();
                break;
        }
        
        board.Draw(grid);
    }

    

    private IEnumerator Flood(Cell cell)
    {
        if (gameover) yield break;
        if (cell.revealed) yield break;
        if (cell.type == Cell.Type.Mine) yield break;
        if (cell.type == Cell.Type.Block) yield break;

        floodingCount++;
        isFlooding = true;

        cell.revealed = true;
        Vector3 centerCellPosition = new Vector3(cell.position.x + 0.5f, cell.position.y + 0.5f, cell.position.z);
        GameObject a = Instantiate(UIDirtExplodePrefab, centerCellPosition, Quaternion.identity);
        Destroy(a, 2f);

        AudioManager.Instance.RockSmashSound();

        board.Draw(grid);
        CollectOre(cell);

        yield return null;

        CheckWinCondition();

        if (cell.type == Cell.Type.Empty)
        {
            yield return new WaitForSeconds(0.3f);
            if (grid.TryGetCell(cell.position.x - 1, cell.position.y, out Cell left)) {
                StartCoroutine(Flood(left));
            }
            if (grid.TryGetCell(cell.position.x + 1, cell.position.y, out Cell right)) {
                StartCoroutine(Flood(right));
            }
            if (grid.TryGetCell(cell.position.x, cell.position.y - 1, out Cell down)) {
                StartCoroutine(Flood(down));
            }
            if (grid.TryGetCell(cell.position.x, cell.position.y + 1, out Cell up)) {
                StartCoroutine(Flood(up));
            }
            /*if (grid.TryGetCell(cell.position.x - 1, cell.position.y - 1, out Cell topLeft))
            {
                StartCoroutine(Flood(topLeft));
            }
            if (grid.TryGetCell(cell.position.x + 1, cell.position.y - 1, out Cell topRight))
            {
                StartCoroutine(Flood(topRight));
            }
            if (grid.TryGetCell(cell.position.x - 1, cell.position.y + 1, out Cell bottomLeft))
            {
                StartCoroutine(Flood(bottomLeft));
            }
            if (grid.TryGetCell(cell.position.x + 1, cell.position.y + 1, out Cell bottomRight))
            {
                StartCoroutine(Flood(bottomRight));
            }*/
        }

        floodingCount--;
        if (floodingCount <= 0)
        {
            floodingCount = 0;
            Invoke("CheckAfterFlooding", 0.5f);
        }
    }

    public void CheckAfterFlooding()
    {
        isFlooding = false;
    }

    public bool GetIsFlooding()
    {
        return isFlooding;
    }

    private void Flag()
    {
        if (!TryGetCellAtMousePosition(out Cell cell)) return;
        if (cell.revealed) return;
        cell.flagged = !cell.flagged;
        board.Draw(grid);
    }

    private void Explode(Cell cell)
    {
        Debug.Log("Game Over!");
        gameover = true;

        // Set the mine as exploded
        cell.exploded = true;
        cell.revealed = true;

       

        // Reveal all other mines
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = grid[x, y];

                if (cell.type == Cell.Type.Mine) {
                    cell.revealed = true;
                }

            }
        }
        
        Invoke("AllMineExplode", 1);
        Invoke("Lose", 1.5f);
    }

    public void AllMineExplode()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                if (cell.type == Cell.Type.Mine) {
                    cell.revealed = true;

                    Vector3 centerCellPositiona2 = new Vector3(cell.position.x + 0.5f, cell.position.y + 0.5f, cell.position.z);
                    GameObject b = Instantiate(UIMineExplodePrefab, centerCellPositiona2, Quaternion.identity);
                    Destroy(b, 1.5f);
                }

            }
        }
        AudioManager.Instance.ExplodeSound();
    }
        

    public void Lose()
    {
        float totalarea = 0;
        float sumarea = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                if (cell.revealed && cell.type != Cell.Type.Block &&  cell.type != Cell.Type.Mine)
                {
                    totalarea++;
                    sumarea++;                 
                }
                if (cell.revealed == false && cell.type != Cell.Type.Block && cell.type != Cell.Type.Mine)
                    sumarea++;
            }
        }

        if ((totalarea / sumarea * 100) >= 100)
            textcomplete.text = (totalarea / (totalarea + 2) * 100).ToString("F2") + "%";
        else
        textcomplete.text = (totalarea / sumarea * 100).ToString("F2") + "%";
        gameoverscreen.SetActive(true);
        //_userDatas.heart--;
        //SaveData();
        //Lives.Instance.LoseHeart();
    }
    
    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                // All non-mine cells must be revealed to have won
                if (cell.type != Cell.Type.Mine && !cell.revealed) {
                    return; // no win
                }
            }
        }

        Invoke("Win", 1);
    }
    
    public void Win()
    {

        Debug.Log("Winner!");

        // Flag all the mines
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                }
            }
        }

        gameover = true;
        AudioManager.Instance.WinSound();
        gamewinscreen.SetActive(true);
    }
    private bool TryGetCellAtMousePosition(out Cell cell)
    {
        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x < 0 || mousePos.x > Screen.width 
            ||  mousePos.y < 0 || mousePos.y > Screen.height)
        {
            cell = null;
            return false; //do nothing if the mouse is outside the game window
        }

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);

        return grid.TryGetCell(cellPosition.x, cellPosition.y, out cell); 
    }


    int probality = 100;
    private void CollectOre(Cell cell)
    {
        int bonusgold = 0, bonusdiamond = 0;
        int dice = Random.Range(1, probality);
        if (dice <= /*_userDatas.probalitygold*/probalitygold)
            bonusgold = Random.Range(0, 5);
        if (dice <= /*_userDatas.probalitydiamond*/probalitydiamond)
            bonusdiamond = Random.Range(0, 2);

        for(int i = 0; i < bonusgold; i++)
        {
            InstantiateAndMove(goldPrefab, cell.position);
            goldCount += 1;

            AudioManager.Instance.OreSound();
        }
        for (int i = 0; i < bonusdiamond; i++)
        {
            InstantiateAndMove(diamondPrefab, cell.position);
            diamondCount += 1;

            AudioManager.Instance.RareOreSound();
        }

        //if(Random.value > 0.98)        //ti le spawn bug
            //SpawnBug(cell);
        SaveData();
    }

    public void CheckReward()
    {
        if (isFlooding) return;
        if (rewardPopup.activeSelf) return;

        if (goldCount > 0 || diamondCount > 0)
        {
            tonCoin += goldCount * GameStatsManager.Instance.PriceGold + diamondCount*GameStatsManager.Instance.PriceDiamond;
            AdjustText();
            rewardPopup.SetActive(true);
        }
    }

    public void GetRewardOre()
    {
        Sell();

        goldCount = 0;
        diamondCount = 0;

        holdTime = 0f;

        rewardPopup.SetActive(false);
    }

    public void Sell()
    {
        long nanoAmount = (long)(tonCoin * 1_000_000_000m);
        Debug.Log($"Sell ore: {tonCoin} TON -> {nanoAmount} NanoTON");

        //{ } + ($) -> {{ }}
        string jsonParams = $"{{\"amountReward\": {nanoAmount}}}";
        UnitonConnectSDK.Instance.SendSmartContractTransaction(HandleClaimRewardResult, "claim_reward", jsonParams);
    }

    private void HandleClaimRewardResult(bool isSuccess)
    {
        if (isSuccess)
        {
            UnitonConnectSDK.Instance.LoadBalance();
            UnitonConnectLogger.Log("Claim reward success");
        }
        else
        {
            UnitonConnectLogger.Log("Claim reward failed");
        }
    }

    public void GetRewardWin() //get reward win and backhome
    {
        long nanoAmount = 100_000_000;
        Debug.Log($"Get reward win: {nanoAmount}");

        //{ } + ($) -> {{ }}
        string jsonParams = $"{{\"amountReward\": {nanoAmount}}}";
        UnitonConnectSDK.Instance.SendSmartContractTransaction(HandleGetRewardWinResult, "claim_reward", jsonParams);
    }
    private void HandleGetRewardWinResult(bool isSuccess)
    {
        if (isSuccess)
        {
            UnitonConnectSDK.Instance.LoadBalance();
            UnitonConnectLogger.Log("Get Reward Win success");

            BackToHome();
        }
        else
        {
            UnitonConnectLogger.Log("Get Reward Win failed");
        }
    }

    public void DropReward()
    {
        goldCount = 0;
        diamondCount = 0;
        holdTime = 0f;

        surePopup.SetActive(true);
    }

    public void OnclickYesSure()
    {
        if (rewardPopup.activeSelf)
            rewardPopup.SetActive(false); //drop dig
        else if(gamewinscreen.activeSelf)
        {
            //drop reward win and back home
            BackToHome();
        }
            
        surePopup.SetActive(false);
    }

    public void OnclickNoSure()
    {
        surePopup.SetActive(false);
    }

    private IEnumerator MoveAndHandleObject(GameObject obj)
    {
        float moveDuration = 1.0f;
        float elapsedTime = 0.0f;

        Vector3 startPosition = obj.transform.position;
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.z = 0; // Keep movement in the XY plane
        float randomDistance = Random.Range(1.5f, 3.0f);
        Vector3 moveVector = randomDirection.normalized * randomDistance;
        Vector3 endPosition = startPosition + moveVector;

        // Calculate the deceleration needed to stop at the end position
        float deceleration = (2 * randomDistance) / (moveDuration * moveDuration);

        while (elapsedTime < moveDuration)
        {
            // Update the position without using Lerp, instead applying deceleration
            float currentSpeed = Mathf.Clamp(moveVector.magnitude - deceleration * elapsedTime, 0, moveVector.magnitude);
            obj.transform.position += 3 * moveVector.normalized * currentSpeed * Time.deltaTime;

            // Check if the object has moved beyond the end position
            if ((obj.transform.position - startPosition).sqrMagnitude >= moveVector.sqrMagnitude)
            {
                obj.transform.position = endPosition; // Clamp to end position
                break; // Exit the loop
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(obj);
    }

    public void InstantiateAndMove(GameObject prefab, Vector3 position)
    {
        Vector3 centerCellPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, position.z);
        GameObject obj = Instantiate(prefab, centerCellPosition, Quaternion.identity);
        StartCoroutine(MoveAndHandleObject(obj));
    }

    private void Sweep() 
    {
        if (sweepmode == -1)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = grid[x, y];
                    if (cell.type == Cell.Type.Number && cell.revealed == true)
                    {
                        cell.type = Cell.Type.NumEmpty;
                        cell.numempty = true;
                    }
                }
            }
        }
        else if (sweepmode == 1)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = grid[x, y];
                    if (cell.type == Cell.Type.NumEmpty)
                    {
                        cell.type = Cell.Type.Number;
                        cell.numempty = false;
                    }
                }
            }
        }
        board.Draw(grid);
    }


    public void SaveData()
    {
        //JSON hóa data clas
        string dataJSON = JsonUtility.ToJson(this._userDatas);
        Debug.Log("DATA " + dataJSON);
        //save JSON string
        PlayerPrefs.SetString(DATA_KEY, dataJSON);

    }

    public void BackToHome()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
    }

    public void SpawnBug(Cell cell)
    {
        GameObject bug = Instantiate(bugPrefab, cell.position, Quaternion.identity);
    }
}
        



