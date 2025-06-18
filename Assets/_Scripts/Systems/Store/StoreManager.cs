using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static Action onRefresh;

    [SerializeField] private GameplayScriptable gameplayScriptable;

    [Header("Store")]
    [SerializeField] private GameObject prefabPowerUps;
    [SerializeField] private GameObject prefabCustoms;
    [SerializeField] private ScrollRect scrollRect;

    private GameObject instenciatedPrefab;
    private readonly List<GameObject> instanciatedPrefabs = new();
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Transform first;
    [SerializeField] private Transform top;
    [SerializeField] private Transform bottom;
    [SerializeField] private Transform last;
    private bool canMoveFromSelected = true;

    [Header("UI")]
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform panel1;
    [SerializeField] private RectTransform panel2;
    [SerializeField] private RectTransform tgl1;
    [SerializeField] private RectTransform tgl2;

    [Space]
    [SerializeField] private Toggle powersTgl;
    [SerializeField] private Toggle customTgl;

    [Header("Info Panel")]
    [SerializeField] private TextMeshProUGUI titleTmp;
    [SerializeField] private LocalizeStringEvent titleTxt;
    [SerializeField] private TextMeshProUGUI descriptionTmp;
    [SerializeField] private LocalizeStringEvent descriptionTxt;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Toggle selectTgl;
    [SerializeField] private GameObject onTgl;
    [SerializeField] private GameObject offTgl;

    private bool powerUpsMode = true;
    private PowerUpBase showingPowerUp;
    private ShipScriptable showingShip;

    private const float limit = 1.050505f;
    private float cameraAspect = 1.777778f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        onRefresh = null;
    }

    private void Awake()
    {
        //onRefresh += UpdateUi;

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

    private void OnDestroy()
    {
        //onRefresh -= UpdateUi;
#if Platform_Mobile
        AdsManager.OnRewardedAdLoaded -= OnAdLoaded;
        AdsManager.OnRewardedAdCompleted -= OnAdViewed;
#endif
    }

    private void InitPrefabs()
    {
        instanciatedPrefabs.Clear();
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            Destroy(scrollRect.content.GetChild(i).gameObject);
        }

        if (powerUpsMode)
        {
            for (int i = 0; i < gameplayScriptable.powerUps.Count; i++)
            {
                instenciatedPrefab = Instantiate(prefabPowerUps, scrollRect.content);
                instenciatedPrefab.GetComponent<StoreItem>().Init(this, gameplayScriptable.powerUps[i]);
                instanciatedPrefabs.Add(instenciatedPrefab);
            }
        }
        else
        {
            for (int i = 0; i < gameplayScriptable.customs.Count; i++)
            {
                instenciatedPrefab = Instantiate(prefabCustoms, scrollRect.content);
                instenciatedPrefab.GetComponent<StoreItemCustom>().Init(this, gameplayScriptable.customs[i]);
                instanciatedPrefabs.Add(instenciatedPrefab);
            }
        }

        instenciatedPrefab = null;

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
                PowerUpsManager.Instance.SelectPowerUp(showingPowerUp);
            }
            else
            {
                PowerUpsManager.Instance.SelectPowerUp(null);
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

    //private void UpdateUi()
    //{
    //}

    public void SetUi(bool on = true)
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
            DisableComponents();
            return;
        }

        showingPowerUp = powerUp;

        titleTxt.StringReference.SetReference("PowerUps", showingPowerUp.powerTitleKey);
        descriptionTxt.StringReference.SetReference("PowerUps", showingPowerUp.powerDescriptionKey);

        buyBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.AddListener(() => BuyPowerUp());
#if !Platform_Mobile
        buyBtn.interactable = PlayerProgress.GetCoins() >= showingPowerUp.cost;
#else
        buyBtn.interactable = true;
#endif

        selectTgl.SetIsOnWithoutNotify(gameplayScriptable.selectedPowerUp == showingPowerUp);
        onTgl.SetActive(selectTgl.isOn);
        offTgl.SetActive(!selectTgl.isOn);
        selectTgl.interactable = showingPowerUp.currentAmount > 0;
    }

    public void DisableComponents()
    {
        titleTxt.StringReference.SetReference("PowerUps", "_empty");
        descriptionTxt.StringReference.SetReference("PowerUps", "_empty");

        buyBtn.interactable = false;
        selectTgl.interactable = false;

        selectTgl.SetIsOnWithoutNotify(false);
        onTgl.SetActive(false);
        offTgl.SetActive(true);
    }

    private void BuyPowerUp()
    {
        if (PlayerProgress.GetCoins() < showingPowerUp.cost)
        {
            PopupManager.Instance.OpenPopUp(
                "watchAdFor_title1",
                "close",
                () =>
                {
                    buyBtn.Select();

#if Platform_Mobile
                    AdsManager.OnRewardedAdLoaded -= OnAdLoaded;
#endif
                    PopupManager.Instance.ClosePopUp();
                },
                "loading",
                null,
                false);

#if Platform_Mobile
            AdsManager.ResetRewardedRetrys();
            AdsManager.OnRewardedAdLoaded += OnAdLoaded;
            AdsManager.PrepareAd(AdsManager.AdType.Rewarded_Coins);
#endif
            return;
        }

        PlayerProgress.SetCoins(-(int)showingPowerUp.cost);
        showingPowerUp.currentAmount++;
        //buyBtn.interactable = PlayerProgress.GetCoins() >= showingPowerUp.cost;
        buyBtn.interactable = true;
        selectTgl.interactable = showingPowerUp.currentAmount > 0;

        onRefresh?.Invoke();

        AnalyticsManager.Log_BuyPowerUp(showingPowerUp.type, showingPowerUp.cost);
    }

    private void OnAdLoaded(bool success, double amount)
    {
#if Platform_Mobile
        AdsManager.OnRewardedAdLoaded -= OnAdLoaded;

        if (success)
        {
            PopupManager.Instance.UpdateText("watchAdFor_title2", amount.ToString());
            PopupManager.Instance.UpdateRightText("watchAdFor_watch");
            PopupManager.Instance.UpdateRightAction(() =>
            {
                AdsManager.OnRewardedAdCompleted += OnAdViewed;
                AdsManager.ShowRewarded();

                buyBtn.Select();
                PopupManager.Instance.ClosePopUp();
            });
        }
        else
        {
            PopupManager.Instance.UpdateText("noAds");
            PopupManager.Instance.UpdateRightText("tryLater");
            PopupManager.Instance.UpdateRightAction(() =>
            {
                buyBtn.Select();
                PopupManager.Instance.ClosePopUp();
            });
        }
#endif
    }

    private void OnAdViewed(bool rewarded)
    {
#if Platform_Mobile
        AdsManager.OnRewardedAdCompleted -= OnAdViewed;
#endif

        if (rewarded)
        {
            GameManager.Instance.UpCoins(10);
        }
    }

    public void SelectCustom(ShipScriptable ship)
    {
        if (ship == null)
        {
            DisableComponents();
            return;
        }

        showingShip = ship;

        titleTmp.text = showingShip.name;
        descriptionTmp.text = showingShip.description;

        buyBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.AddListener(() => BuyCustom());
        //buyBtn.interactable = PlayerProgress.GetCoins() >= showingShip.cost && !showingShip.owned;
        buyBtn.interactable = true;

        selectTgl.SetIsOnWithoutNotify(gameplayScriptable.selectedCustoms == showingShip);
        onTgl.SetActive(selectTgl.isOn);
        offTgl.SetActive(!selectTgl.isOn);
        selectTgl.interactable = showingShip.owned;
    }

    public void BuyCustom()
    {
        PlayerProgress.SetCoins(-(int)showingShip.cost);
        showingShip.owned = true;
        buyBtn.interactable = !showingShip.owned;
        selectTgl.interactable = showingShip.owned;

        onRefresh?.Invoke();
    }

    public void SetMoveFromSelected(bool on)
    {
        canMoveFromSelected = on;
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

            tgl1.offsetMin = new Vector2(8f, 16f);
            tgl1.offsetMax = new Vector2(-8f, -16f);
            tgl2.offsetMin = new Vector2(8f, 16f);
            tgl2.offsetMax = new Vector2(-8f, -16f);
        }
        else
        {
            // vertical
            panel1.anchorMin = new Vector2(0.04f, 0.41f);
            panel1.anchorMax = new Vector2(0.96f, 0.9f);

            panel2.anchorMin = new Vector2(0.04f, 0f);
            panel2.anchorMax = new Vector2(0.96f, 0.41f);

            tgl1.offsetMin = new Vector2(8f, 8f);
            tgl1.offsetMax = new Vector2(-8f, -8f);
            tgl2.offsetMin = new Vector2(8f, 8f);
            tgl2.offsetMax = new Vector2(-8f, -8f);
        }

        if (ControlsManager.hasTouch ||
            !canMoveFromSelected ||
            top.position.y <= bottom.position.y ||
            instanciatedPrefabs.Count <= 4)
        {
            return;
        }

        if (eventSystem.currentSelectedGameObject == instanciatedPrefabs[0])
        {
            if (instanciatedPrefabs[0].transform.position.y > first.position.y)
            {
                scrollRect.StopMovement();
                scrollRect.content.localPosition -= 1000f * Time.deltaTime * Vector3.up;
            }
        }
        else if (eventSystem.currentSelectedGameObject == instanciatedPrefabs[^1])
        {
            if (instanciatedPrefabs[^1].transform.position.y < last.position.y)
            {
                scrollRect.StopMovement();
                scrollRect.content.localPosition += 1000f * Time.deltaTime * Vector3.up;
            }
        }
        else
        {
            for (int i = 1; i < instanciatedPrefabs.Count - 1; i++)
            {
                if (eventSystem.currentSelectedGameObject == instanciatedPrefabs[i])
                {
                    if (instanciatedPrefabs[i].transform.position.y > top.position.y)
                    {
                        scrollRect.StopMovement();
                        scrollRect.content.localPosition -= 1000f * Time.deltaTime * Vector3.up;
                    }
                    else if (instanciatedPrefabs[i].transform.position.y < bottom.position.y)
                    {
                        scrollRect.StopMovement();
                        scrollRect.content.localPosition += 1000f * Time.deltaTime * Vector3.up;
                    }
                }
            }
        }
    }
}