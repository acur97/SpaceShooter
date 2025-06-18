using Cysharp.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    [SerializeField] private Image spriteImg;
    [SerializeField] private TextMeshProUGUI countTxt;
    [SerializeField] private Color haveColor;
    [SerializeField] private Color dontColor;
    [SerializeField] private LocalizeStringEvent titleTxt;
    [SerializeField] private TextMeshProUGUI priceTxt;
    [SerializeField] private Button selectBtn;
    [SerializeField] private Image selected;

    private StoreManager store;
    private PowerUpBase powerUp;

    public void Init(StoreManager _store, PowerUpBase _powerUp)
    {
        store = _store;
        powerUp = _powerUp;

        spriteImg.sprite = _powerUp.sprite;
        titleTxt.StringReference.SetReference("PowerUps", _powerUp.powerTitleKey);

        if (_powerUp.cost == 0)
        {
            priceTxt.text = UiCommonTexts.Free;
        }
        else
        {
            priceTxt.SetTextFormat(UiCommonTexts.PriceFormat, _powerUp.cost);
        }

        StoreManager.onRefresh += UpdateBtn;

        selectBtn.onClick.AddListener(Select);
    }

    private void UpdateBtn()
    {
        countTxt.SetTextFormat(UiCommonTexts.CountFormat, powerUp.currentAmount);
        countTxt.color = powerUp.currentAmount > 0 ? haveColor : dontColor;

        selected.enabled = GameManager.Instance.gameplayScriptable.selectedPowerUp == powerUp;
    }

    private void Select()
    {
        store.SelectPowerUp(powerUp);
    }

    private void OnDestroy()
    {
        StoreManager.onRefresh -= UpdateBtn;
    }
}