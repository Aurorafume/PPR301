using UnityEngine;  
using System.Collections;  
using UnityEngine.EventSystems;  
using UnityEngine.UI;
using TMPro;

public class MenuButton : MonoBehaviour, IPointerEnterHandler {

    public TMP_Text theText;
    public Color colour1;
    public Color colour2;
    private RectTransform textRect;

    void Start()
    {
        textRect = theText.GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        theText.color = colour2;
    }

    void Update()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(textRect, Input.mousePosition))
        {
            theText.color = colour1;
        }
    }
}