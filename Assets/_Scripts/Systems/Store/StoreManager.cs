using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static Action onRefresh;

    [SerializeField] private GameplayScriptable gameplayScriptable;

    [Header("Store")]
    [SerializeField] private GameObject prefabPowerUps;
    [SerializeField] private GameObject prefabCustoms;
    [SerializeField] private Transform content;

    [Header("UI")]
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform panel1;
    [SerializeField] private RectTransform panel2;

    [Space]
    [SerializeField] private Toggle customTgl;
    [SerializeField] private Toggle powersTgl;

    [Header("Info Panel")]
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Toggle selectTgl;
    [SerializeField] private GameObject onTgl;
    [SerializeField] private GameObject offTgl;

    private bool powerUpsMode = true;
    private PowerUpBase showingPowerUp;
    private ShipScriptable showingShip;

    [Space]
    [SerializeField] private TextMeshProUGUI coinsTxt;

    private const float limit = 0.746988f;
    private float cameraAspect = 1.777778f;

    private void Awake()
    {
        onRefresh += UpdateUi;

        customTgl.onValueChanged.AddListener((on) =>
        {
            if (on)
            {
                powerUpsMode = false;
                InitPrefabs();
            }
        });
        powersTgl.onValueChanged.AddListener((on) =>
        {
            if (on)
            {
                powerUpsMode = true;
                InitPrefabs();
            }
        });

        InitPrefabs();

        selectTgl.onValueChanged.AddListener(UpdateToggle);

        SelectPowerUp(null);
    }

    private void InitPrefabs()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        if (powerUpsMode)
        {
            for (int i = 0; i < gameplayScriptable.powerUps.Count; i++)
            {
                Instantiate(prefabPowerUps, content).GetComponent<StoreItem>().Init(this, gameplayScriptable.powerUps[i]);
            }
        }
        else
        {
            for (int i = 0; i < gameplayScriptable.customs.Count; i++)
            {
                Instantiate(prefabCustoms, content).GetComponent<StoreItemCustom>().Init(this, gameplayScriptable.customs[i]);
            }
        }

        onRefresh?.Invoke();
    }

    private void UpdateToggle(bool on)
    {
        onTgl.SetActive(on);
        offTgl.SetActive(!on);

        if (powerUpsMode)
        {
            if (on)
            {
                gameplayScriptable.selectedPowerUp = showingPowerUp;
            }
            else
            {
                gameplayScriptable.selectedPowerUp = null;
            }
        }
        else
        {
            if (on)
            {
                GameManager.Instance.SetCustoms(showingShip);
            }
            else
            {
                selectTgl.isOn = true;
            }
        }

        onRefresh?.Invoke();
    }

    private void Start()
    {
        onRefresh?.Invoke();
    }

    private void UpdateUi()
    {
        coinsTxt.text = $"Coins: {PlayerProgress.GetCoins()}";
    }

    public void SetUi(bool on)
    {
        if (on)
        {
            UiManager.Instance.SetUi(UiType.Store, true, 0.5f, () => UiManager.Instance.SetUi(UiType.Select, false));
        }
        else
        {
            UiManager.Instance.SetUi(UiType.Select, true);
            UiManager.Instance.SetUi(UiType.Store, false, 0.25f);

            PlayerProgress.SavePowerUps(false);
            PlayerProgress.SaveCustoms(true);
        }
    }

    public void SelectPowerUp(PowerUpBase powerUp)
    {
        if (powerUp == null)
        {
            titleTxt.text = string.Empty;
            descriptionTxt.text = string.Empty;

            buyBtn.interactable = false;
            selectTgl.interactable = false;

            selectTgl.SetIsOnWithoutNotify(false);
            onTgl.SetActive(false);
            offTgl.SetActive(true);

            return;
        }

        showingPowerUp = powerUp;

        titleTxt.text = showingPowerUp.powerName;
        descriptionTxt.text = showingPowerUp.description;

        buyBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.AddListener(() => BuyPowerUp());
        buyBtn.interactable = PlayerProgress.GetCoins() >= showingPowerUp.cost;

        selectTgl.SetIsOnWithoutNotify(gameplayScriptable.selectedPowerUp == showingPowerUp);
        onTgl.SetActive(selectTgl.isOn);
        offTgl.SetActive(!selectTgl.isOn);
        selectTgl.interactable = showingPowerUp.currentAmount > 0;
    }

    private void BuyPowerUp()
    {
        PlayerProgress.UpCoins(-(int)showingPowerUp.cost);
        showingPowerUp.currentAmount++;
        buyBtn.interactable = PlayerProgress.GetCoins() >= showingPowerUp.cost;
        selectTgl.interactable = showingPowerUp.currentAmount > 0;

        onRefresh?.Invoke();
    }

    public void SelectCustom(ShipScriptable ship)
    {
        if (ship == null)
        {
            titleTxt.text = string.Empty;
            descriptionTxt.text = string.Empty;

            buyBtn.interactable = false;
            selectTgl.interactable = false;

            selectTgl.SetIsOnWithoutNotify(false);
            onTgl.SetActive(false);
            offTgl.SetActive(true);

            return;
        }

        showingShip = ship;

        titleTxt.text = showingShip.name;
        descriptionTxt.text = showingShip.description;

        buyBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.AddListener(() => BuyCustom());
        buyBtn.interactable = PlayerProgress.GetCoins() >= showingShip.cost && !showingShip.owned;

        selectTgl.SetIsOnWithoutNotify(gameplayScriptable.selectedCustoms == showingShip);
        onTgl.SetActive(selectTgl.isOn);
        offTgl.SetActive(!selectTgl.isOn);
        selectTgl.interactable = showingShip.owned;
    }

    public void BuyCustom()
    {
        PlayerProgress.UpCoins(-(int)showingShip.cost);
        showingShip.owned = true;
        buyBtn.interactable = !showingShip.owned;
        selectTgl.interactable = showingShip.owned;

        onRefresh?.Invoke();
    }

    private void Update()
    {
        cameraAspect = cam.aspect;

        if (cameraAspect >= limit)
        {
            // horizontal
            panel1.anchorMin = new Vector2(0.04f, 0.06f);
            panel1.anchorMax = new Vector2(0.49f, 0.9f);

            panel2.anchorMin = new Vector2(0.51f, 0.06f);
            panel2.anchorMax = new Vector2(0.96f, 0.9f);
        }
        else
        {
            // vertical
            panel1.anchorMin = new Vector2(0.04f, 0.4f);
            panel1.anchorMax = new Vector2(0.96f, 0.9f);

            panel2.anchorMin = new Vector2(0.04f, 0.06f);
            panel2.anchorMax = new Vector2(0.96f, 0.4f);
        }
    }
}