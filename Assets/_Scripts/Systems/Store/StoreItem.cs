using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    [SerializeField] private Image spriteImg;
    [SerializeField] private TextMeshProUGUI countTxt;
    [SerializeField] private Color haveColor;
    [SerializeField] private Color dontColor;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private TextMeshProUGUI priceTxt;
    [SerializeField] private Button selectBtn;
    [SerializeField] private Image selected;

    private StoreManager store;
    private PowerUpBase powerUp;

    private const string _free = "free";

    public void Init(StoreManager _store, PowerUpBase _powerUp)
    {
        store = _store;
        powerUp = _powerUp;

        spriteImg.sprite = powerUp.sprite;
        titleTxt.text = powerUp.powerName;
        priceTxt.text = powerUp.cost == 0 ? _free : $"${powerUp.cost}";

        StoreManager.onRefresh += UpdateBtn;

        selectBtn.onClick.AddListener(Select);
    }

    private void UpdateBtn()
    {
        countTxt.text = powerUp.currentAmount.ToString();
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