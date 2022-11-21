using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class PromptTextUI : MonoBehaviour
{
    public static PromptTextUI Instance;
    
    public float offsetY = 25f;
    public TextMeshProUGUI text;
    [TextArea(5, 15)]
    public string tipsText;
    [TextArea(5, 15)]
    [OnValueChanged("OnFormatTextChanged")]
    public string formatText;
    [TextArea(5, 15)]
    public string onlyTipsFormatText;
    
    private RectTransform _rectTransform;
    private float _position;
    private int _tweenId;
    private bool _state;
    private bool _onlyTips;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        Instance = this;
        UpdatePosition();
    }

    private void Start()
    {
        SetPromptText(NewImageGenerator.Instance.GetPromptText());
    }

    public void SetOnlyTips(bool onlyTips)
    {
        _onlyTips = onlyTips;
        UpdateTipsText(tipsText);
    }

    public void UpdateTipsText(string tips)
    {
        tipsText = tips;
        
        if (_onlyTips)
        {
            text.text = string.Format(onlyTipsFormatText, tipsText);
            text.ForceMeshUpdate();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
            UpdatePosition();
        }
        else
        {
            SetPromptText(NewImageGenerator.Instance.GetPromptText());
        }
    }

    public void SetPromptText(string content)
    {
        text.text = string.Format(formatText, tipsText, content);
        text.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
        UpdatePosition();
    }

    public void Toggle()
    {
        if (_state)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        if (_state) return;

        if (!gameObject.activeSelf)
        {
            _state = true;
            _position = 1;
            UpdatePosition();
            return;
        }

        _state = true;
        LeanTween.cancel(_tweenId);
        _tweenId = LeanTween.value(gameObject, _position, 1, 0.6f)
            .setEase(LeanTweenType.easeInOutCubic)
            .setOnUpdate(UpdatePosValue)
            .id;
    }

    public void Hide()
    {
        if (!_state) return;

        _state = false;
        LeanTween.cancel(_tweenId);
        _tweenId = LeanTween.value(gameObject, _position, 0, 0.6f)
            .setEase(LeanTweenType.easeInOutCubic)
            .setOnUpdate(UpdatePosValue)
            .id;
    }

    public void HideImmediately()
    {
        if (!_state) return;

        _state = false;
        UpdatePosValue(0);
    }

    private void UpdatePosValue(float v)
    {
        _position = v;
        UpdatePosition();
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        var pos = _rectTransform.anchoredPosition;
        pos.y = (1 - _position) * (_rectTransform.rect.height + offsetY);
        _rectTransform.anchoredPosition = pos;
    }

    private void OnFormatTextChanged()
    {
        if (text != null) text.text = formatText;
    }
}
