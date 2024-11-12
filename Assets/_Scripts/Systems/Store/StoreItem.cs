using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    [SerializeField] private Image spriteImg;
    [SerializeField] private TextMeshProUGUI countTxt;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private TextMeshProUGUI priceTxt;
    [SerializeField] private Button buttonBtn;

    private uint price;
    private PowerUpBase powerUp;

    public void Init(PowerUpBase _powerUp)
    {
        powerUp = _powerUp;
        price = powerUp.cost;

        spriteImg.sprite = powerUp.sprite;
        countTxt.text = powerUp.currentAmount.ToString();
        titleTxt.text = powerUp.powerName;
        priceTxt.text = $"${price}";

        StoreManager.onRefresh += UpdateBtn;
    }

    private void UpdateBtn()
    {
        if (PlayerProgress.coins < price)
        {
            buttonBtn.interactable = false;
        }
    }

    public void Buy()
    {
        PlayerProgress.coins -= (int)price;
        powerUp.currentAmount++;

        countTxt.text = powerUp.currentAmount.ToString();
    }

    private void OnDestroy()
    {
        StoreManager.onRefresh -= UpdateBtn;
    }
}