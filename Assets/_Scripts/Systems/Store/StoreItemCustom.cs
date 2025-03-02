using Cysharp.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemCustom : MonoBehaviour
{
    [SerializeField] private Image spriteImg;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private TextMeshProUGUI priceTxt;
    [SerializeField] private Button selectBtn;
    [SerializeField] private Image selected;

    private StoreManager store;
    private ShipScriptable ship;

    public void Init(StoreManager _store, ShipScriptable _ship)
    {
        store = _store;
        ship = _ship;

        spriteImg.sprite = _ship.sprite;
        titleTxt.text = _ship.name;

        if (_ship.owned)
        {
            priceTxt.text = UiCommonTexts.Owned;
        }
        else if (_ship.cost == 0)
        {
            priceTxt.text = UiCommonTexts.Free;
        }
        else
        {
            priceTxt.SetTextFormat(UiCommonTexts.PriceFormat, _ship.cost);
        }

        StoreManager.onRefresh += UpdateBtn;

        selectBtn.onClick.AddListener(Select);
    }

    private void UpdateBtn()
    {
        priceTxt.text = ship.owned ? UiCommonTexts.Owned : UiCommonTexts.Free;
        selected.enabled = GameManager.Instance.gameplayScriptable.selectedCustoms == ship;
    }

    private void Select()
    {
        store.SelectCustom(ship);
    }

    public void SetStatus(bool on)
    {
        selected.enabled = on;
    }

    private void OnDestroy()
    {
        StoreManager.onRefresh -= UpdateBtn;
    }
}