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

    private const string _free = "free";
    private const string _owned = "owned";

    public void Init(StoreManager _store, ShipScriptable _ship)
    {
        store = _store;
        ship = _ship;

        spriteImg.sprite = _ship.sprite;
        titleTxt.text = _ship.name;
        priceTxt.text = _ship.owned ? _owned : _ship.cost == 0 ? _free : $"${_ship.cost}";

        StoreManager.onRefresh += UpdateBtn;

        selectBtn.onClick.AddListener(Select);
    }

    private void UpdateBtn()
    {
        priceTxt.text = ship.owned ? _owned : _free;
        selected.enabled = GameManager.Instance.selectedCustoms == ship;
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