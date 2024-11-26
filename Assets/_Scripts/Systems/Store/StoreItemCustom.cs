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
        priceTxt.text = _ship.owned ? Types.ui_Owned : _ship.cost == 0 ? Types.ui_Free : $"${_ship.cost}";

        StoreManager.onRefresh += UpdateBtn;

        selectBtn.onClick.AddListener(Select);
    }

    private void UpdateBtn()
    {
        priceTxt.text = ship.owned ? Types.ui_Owned : Types.ui_Free;
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