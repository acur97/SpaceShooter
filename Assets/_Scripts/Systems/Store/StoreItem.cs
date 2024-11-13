using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    [SerializeField] private Image spriteImg;
    [SerializeField] private TextMeshProUGUI countTxt;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private TextMeshProUGUI priceTxt;
    [SerializeField] private Button selectBtn;

    private uint price;
    private StoreManager store;
    private PowerUpBase powerUp;

    public void Init(StoreManager _store, PowerUpBase _powerUp)
    {
        store = _store;
        powerUp = _powerUp;
        price = powerUp.cost;

        spriteImg.sprite = powerUp.sprite;
        titleTxt.text = powerUp.powerName;
        priceTxt.text = $"${price}";

        StoreManager.onRefresh += UpdateBtn;

        selectBtn.onClick.AddListener(Select);
    }

    private void UpdateBtn()
    {
        countTxt.text = powerUp.currentAmount.ToString();
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