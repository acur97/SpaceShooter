using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [ReadOnly] public bool isOpen = false;

    [Header("Localization")]

    [Header("References")]
    [SerializeField] private GameObject root;
    [SerializeField] private LocalizeStringEvent text;

    [Space]
    [SerializeField] private Button leftButton;
    [SerializeField] private LocalizeStringEvent leftBtnText;

    [Space]
    [SerializeField] private Button rightButton;
    [SerializeField] private LocalizeStringEvent rightBtnText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Instance = null;
    }

    private void Awake()
    {
        Instance = this;

        DisableRoot();
        root.transform.localScale = Vector3.zero;
    }

    public void OpenPopUp(TableEntryReference _text, TableEntryReference _leftText = default, Action _leftAction = null, TableEntryReference _rightText = default, Action _rightAction = null, bool _autoClose = true)
    {
        isOpen = true;

        text.StringReference.SetReference("PopUps", _text);

        if (_leftText.Equals(default))
            _leftText = "accept";
        leftBtnText.StringReference.SetReference("PopUps", _leftText);
        leftButton.onClick.RemoveAllListeners();
        leftButton.onClick.AddListener(() => _leftAction?.Invoke());

        if (_rightText.Equals(default))
            _rightText = "cancel";
        rightBtnText.StringReference.SetReference("PopUps", _rightText);
        rightButton.onClick.RemoveAllListeners();
        rightButton.onClick.AddListener(() => _rightAction?.Invoke());

        if (_autoClose)
        {
            rightButton.onClick.AddListener(() => ClosePopUp());
            leftButton.onClick.AddListener(() => ClosePopUp());
        }

        LeanTween.scale(root, Vector3.one, 0.3f)
            .setEaseOutBack();

        root.SetActive(true);
    }

    public void ClosePopUp()
    {
        isOpen = false;

        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();

        LeanTween.scale(root, Vector3.zero, 0.2f)
            .setEaseInBack()
            .setOnComplete(DisableRoot);
    }

    private void DisableRoot()
    {
        root.SetActive(false);
    }

    public void UpdateText(TableEntryReference _text, string customVariable = null)
    {
        if (customVariable != null)
            (text.StringReference["custom"] as StringVariable).Value = customVariable;

        text.StringReference.SetReference("PopUps", _text);
    }

    public void UpdateLeftText(TableEntryReference _text, string customVariable = null)
    {
        if (customVariable != null)
            (leftBtnText.StringReference["custom"] as StringVariable).Value = customVariable;

        leftBtnText.StringReference.SetReference("PopUps", _text);
    }

    public void UpdateRightText(TableEntryReference _text, string customVariable = null)
    {
        if (customVariable != null)
            (rightBtnText.StringReference["custom"] as StringVariable).Value = customVariable;

        rightBtnText.StringReference.SetReference("PopUps", _text);
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