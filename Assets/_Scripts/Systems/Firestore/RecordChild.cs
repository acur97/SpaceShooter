using TMPro;
using UnityEngine;

public class RecordChild : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _position;
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _score;

    public void SetRecord(string position, string score, string playerName)
    {
        _position.text = position;
        _playerName.text = playerName;
        _score.text = score;
    }
}