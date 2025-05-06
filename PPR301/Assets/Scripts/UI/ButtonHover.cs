using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // An array of all dull images
    // e.g 0 - Resume, 1 - Settings, 2 - Leave, 3 - Back.
    public GameObject[] dullImages;

    // The index corresponding to this button's dull image.
    public int dullImageIndex;

    // When the pointer enters the button, disable its corresponding dull image.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (dullImages != null && dullImageIndex >= 0 && dullImageIndex < dullImages.Length)
        {
            dullImages[dullImageIndex].SetActive(false);
        }
    }

    // When the pointer exits the button, re-enable its corresponding dull image.
    public void OnPointerExit(PointerEventData eventData)
    {
        if (dullImages != null && dullImageIndex >= 0 && dullImageIndex < dullImages.Length)
        {
            dullImages[dullImageIndex].SetActive(true);
        }
    }
}
