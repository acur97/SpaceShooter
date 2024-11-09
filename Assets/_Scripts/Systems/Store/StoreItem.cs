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

    private int price;
    private PowerUpBase powerUp;

    public void Init(PowerUpBase _powerUp, Sprite _sprite, string _title, int _price)
    {
        powerUp = _powerUp;
        price = _price;

        spriteImg.sprite = _sprite;
        countTxt.text = powerUp.currentAmount.ToString();
        titleTxt.text = _title;
        priceTxt.text = $"${price}";

        if (PlayerProgress.coins < price)
        {
            buttonBtn.interactable = false;
        }
    }

    public void Buy()
    {
        PlayerProgress.coins -= price;
        powerUp.currentAmount++;

        countTxt.text = powerUp.currentAmount.ToString();
    }
}