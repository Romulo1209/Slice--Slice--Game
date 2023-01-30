using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
struct FinishWindowReferences
{
    public TMP_Text YourPointsText;
    public TMP_Text MultiplierText;
    public TMP_Text FinalResultText;
}

public class GameController : MonoBehaviour
{
    [Header("Player")]
    public bool GameStarted = false;
    [Space]
    [SerializeField] int currentPoints;
    [SerializeField] int accountBalance;
    [SerializeField] Transform spawnPosition;
    [SerializeField] KnifeController actualWeapon;

    [Header("Map Generator")]
    [SerializeField] int actualLevel = 1;
    [SerializeField] [Range(1, 10)] int chunksCount = 2;
    [SerializeField] List<GameObject> levelChunks;

    [Header("Shop")]
    public List<ShopItem> allShopItem;

    [SerializeField] GameObject shopIconPrefab;
    [SerializeField] Transform shopContentFather;
    [SerializeField] TMP_Text shopBalanceText;

    [Header("References")]
    public WeaponsHolderScriptable allWeapons;
    public AllChunksScriptable allChunks;
    public CameraController cameraController;
    public WindowController windowController;
    public TMP_Text LevelPointsText;
    [SerializeField] FinishWindowReferences levelFinishResult;

    public static GameController instance;

    #region Getters & Setters

    public int CurrentPoints { get { return currentPoints; } set { currentPoints += value; LevelPointsText.text = currentPoints.ToString("00000"); } }
    public int Balance { get { return accountBalance; } set { accountBalance += value; } }

    #endregion

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
        SetupFirstEntryGame();

        SetupGame();
    }

    //Controlador para primeira entrada do jogador no jogo
    void SetupFirstEntryGame() {
        if (PlayerPrefs.GetInt("First") == 0) {
            PlayerPrefs.SetInt("Level", 1);
            int i = 0;
            foreach (WeaponScriptable weapon in allWeapons.AllWeapons) {
                if (i == 0) {
                    PlayerPrefs.SetInt(weapon.WeaponName, 1);
                    i++;
                    continue;
                }
                PlayerPrefs.SetInt(weapon.WeaponName, 0);
                i++;
            }
            PlayerPrefs.SetInt("Balance", 0);
            PlayerPrefs.SetInt("Equipped", 0);
            PlayerPrefs.SetInt("First", 1);
        }
    }

    #region Game

    //Remove a tela de menu quando executa o clique e inicia o jogo
    public void GameStart() {
        GameStarted = true;
        windowController.OpenWindow(windowController.AllWindows[1]);
    }

    //Faz setup do Game carrega as informações, instancia a arma selecionada e instancia o mapa
    public void SetupGame()
    {
        actualLevel = PlayerPrefs.GetInt("Level");
        accountBalance = PlayerPrefs.GetInt("Balance");

        GameStarted = false;
        windowController.OpenWindow(windowController.AllWindows[0]);
        currentPoints = 0;
        CurrentPoints = 0;

        SetupPlayer();
        SpawnMap();
    }

    //Instancia a arma que foi selecionada
    public void SetupPlayer()
    {
        if (actualWeapon != null)
            Destroy(actualWeapon.gameObject);

        int weaponID = PlayerPrefs.GetInt("Equipped");
        GameObject Weapon = Instantiate(allWeapons.AllWeapons[weaponID].WeaponPrefab, spawnPosition.position, Quaternion.Euler(45, 0, 0));
        Weapon.GetComponent<KnifeController>().gameController = this;
        cameraController.KnifePos = Weapon.transform;
        cameraController.Setup();
        actualWeapon = Weapon.GetComponent<KnifeController>();
    }

    //Faz o setup do mapa, verifica qual level o player está e gera uma quantidade de chunks dependendo do level que ele está
    void SpawnMap()
    {
        chunksCount = GetChunkCount();

        if (levelChunks.Count > 0)
            RemoveSpawnedChunks();

        float maxDistance = 0;
        for(int i = 0; i < chunksCount; i++) {
            Vector3 spawnPostition = transform.position;
            spawnPostition.y -= 6.5f;
            if (i != 0) {
                spawnPostition.z += 70 * i;
                maxDistance = spawnPostition.z;
            }
            GameObject chunk = Instantiate(allChunks.MapChunks[Random.Range(0, allChunks.MapChunks.Count)], spawnPostition, Quaternion.identity);
            levelChunks.Add(chunk);
        }
        Vector3 FinishLinePosition = new Vector3(0, 17, maxDistance + 60);
        GameObject finish = Instantiate(allChunks.FinishLine, FinishLinePosition, Quaternion.identity);
        levelChunks.Add(finish);
    }
    //Retorna a quantidade de chunks dependendo de qual level o player está
    int GetChunkCount() {
        if (actualLevel <= 10)
            return 1;
        else if (actualLevel <= 20)
            return 2;
        else if (actualLevel <= 30)
            return 3;
        else if (actualLevel <= 40)
            return 4;
        else if (actualLevel <= 50)
            return 5;
        else if (actualLevel <= 60)
            return 6;
        else
            return 7;
    }

    //Remove os chunks que já estão spawnados
    void RemoveSpawnedChunks()
    {
        for(int i = 0; i < levelChunks.Count; i++) {
            Destroy(levelChunks[i].gameObject);
        }
        levelChunks.Clear();
    }

    #endregion

    #region Shop

    //Atualiza o balanço na loja
    public void RefreshShopBalance() { shopBalanceText.text = accountBalance.ToString() + " $"; }

    //Inicia a loja do jogo, e caso ela não esteja spawnada ele spawna ela e caso já esteja instanciada ele apenas atualiza
    public void ShopSetup()
    {
        if (shopContentFather.childCount > 0) {
            RefreshShop();
            return;
        }

        RefreshShopBalance();
        foreach (WeaponScriptable weapon in allWeapons.AllWeapons)
        {
            GameObject icon = Instantiate(shopIconPrefab, shopContentFather);
            var shopItem = icon.GetComponent<ShopItem>();
            shopItem.WeaponScriptable = weapon;
            shopItem.Setup(weapon.WeaponIcon, weapon.WeaponValue);
            allShopItem.Add(shopItem);
        }
    }

    //Atualização da loja
    public void RefreshShop()
    {
        foreach (ShopItem weapon in allShopItem)
        {
            weapon.Setup(weapon.Weapon.WeaponIcon, weapon.Weapon.WeaponValue);
        }
    }

    #endregion

    #region Finish Level

    //Finaliza o Level quando o trigger é ativado
    public void FinishLevel(int multiplier) {
        var totalPoints = currentPoints * multiplier;
        Balance = totalPoints;

        FinishLevelResult(totalPoints, multiplier);
        SavePlayerPrefs();

        windowController.OpenWindow(windowController.AllWindows[3]);
        GameStarted = false;
    }

    //Atualiza os valores na UI
    public void FinishLevelResult(int finalReward, int multiplier)
    {
        levelFinishResult.YourPointsText.text = currentPoints.ToString("00000");
        levelFinishResult.MultiplierText.text = multiplier.ToString("") + "x";
        levelFinishResult.FinalResultText.text = finalReward.ToString("00000");
    }

    //Salva os PlayerPrefs
    void SavePlayerPrefs()
    {
        PlayerPrefs.SetInt("Balance", Balance);
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
    }

    #endregion
}