using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static Action onRefresh;

    [Header("Store")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform content;

    [Header("UI")]
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform panel1;
    [SerializeField] private RectTransform panel2;

    [Header("Info Panel")]
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Toggle selectTgl;
    [SerializeField] private GameObject onTgl;
    [SerializeField] private GameObject offTgl;

    private PowerUpBase showingPowerUp;

    [Space]
    [SerializeField] private TextMeshProUGUI coinsTxt;

    private const float limit = 0.746988f;
    private float cameraAspect = 1.777778f;

    private void Awake()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        for (int i = 0; i < PowerUpsManager.Instance.powerUps.Count; i++)
        {
            Instantiate(prefab, content).GetComponent<StoreItem>().Init(this, PowerUpsManager.Instance.powerUps[i]);
        }

        onRefresh += UpdateUi;

        selectTgl.onValueChanged.AddListener(UpdateToggle);
        selectTgl.onValueChanged.AddListener(SetToggle);

        SelectPowerUp(null);
    }

    private void UpdateToggle(bool on)
    {
        onTgl.SetActive(on);
        offTgl.SetActive(!on);

        if (on)
        {
            PowerUpsManager.Instance.selectedPowerUps = showingPowerUp;
        }
        else
        {
            PowerUpsManager.Instance.selectedPowerUps = null;
        }
    }

    private void SetToggle(bool on)
    {
        if (on)
        {
            PowerUpsManager.Instance.selectedPowerUps = showingPowerUp;
        }
        else
        {
            PowerUpsManager.Instance.selectedPowerUps = null;
        }
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

        selectTgl.SetIsOnWithoutNotify(PowerUpsManager.Instance.selectedPowerUps == showingPowerUp);
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