using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HoverEffectText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text targetText; 

    public int normalFontSize = 80;
    public int hoverFontSize = 90;

    public string normalHex = "#96A85F";
    public string hoverHex = "#536322";

    private Color normalColor;
    private Color hoverColor;

    private void Start()
    {
        if (targetText == null)
            targetText = GetComponentInChildren<TMP_Text>();

        if (!ColorUtility.TryParseHtmlString(normalHex, out normalColor))
            normalColor = Color.white;

        if (!ColorUtility.TryParseHtmlString(hoverHex, out hoverColor))
            hoverColor = Color.yellow;

        ApplyNormal();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ApplyHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ApplyNormal();
    }

    void ApplyHover()
    {
        if (targetText != null)
        {
            targetText.fontSize = hoverFontSize;
            targetText.color = hoverColor;
        }
    }

    void ApplyNormal()
    {
        if (targetText != null)
        {
            targetText.fontSize = normalFontSize;
            targetText.color = normalColor;
        }
    }
}
