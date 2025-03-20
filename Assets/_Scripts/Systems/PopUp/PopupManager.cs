using Cysharp.Text;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("References")]
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI text;

    [Space]
    [SerializeField] private Button leftButton;
    [SerializeField] private TextMeshProUGUI leftBtnText;

    [Space]
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI rightBtnText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Instance = null;
    }

    private void Awake()
    {
        Instance = this;

        root.SetActive(false);
        root.transform.localScale = Vector3.zero;
    }

    public void OpenPopUp(string _text, string _leftText = "Accept", Action _leftAction = null, string _rightText = "Cancel", Action _rightAction = null, bool _autoClose = true)
    {
        text.text = _text;

        leftBtnText.text = _leftText;
        leftButton.onClick.RemoveAllListeners();
        leftButton.onClick.AddListener(() => _leftAction?.Invoke());

        rightBtnText.text = _rightText;
        rightButton.onClick.RemoveAllListeners();
        rightButton.onClick.AddListener(() => _rightAction?.Invoke());

        if (_autoClose)
        {
            rightButton.onClick.AddListener(() => ClosePopUp());
            leftButton.onClick.AddListener(() => ClosePopUp());
        }

        LeanTween.scale(root, Vector3.one, 0.3f).setEaseOutBack();

        root.SetActive(true);
    }

    public void ClosePopUp()
    {
        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();

        LeanTween.scale(root, Vector3.zero, 0.2f).setEaseInBack().setOnComplete(() => root.SetActive(false));
    }

    public void UpdateText(string _text)
    {
        text.text = _text;
    }

    public void UpdateLeftText(string _text)
    {
        leftBtnText.text = _text;
    }

    public void UpdateRightText(string _text)
    {
        rightBtnText.text = _text;
    }

    public void UpdateLeftAction(Action _leftAction = null)
    {
        leftButton.onClick.RemoveAllListeners();
        leftButton.onClick.AddListener(() =>
        {
            _leftAction?.Invoke();
            ClosePopUp();
        });
    }

    public void UpdateRightAction(Action _rightAction = null)
    {
        rightButton.onClick.RemoveAllListeners();
        rightButton.onClick.AddListener(() =>
        {
            _rightAction?.Invoke();
            ClosePopUp();
        });
    }
}