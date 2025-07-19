using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    public Texture2D cursorTextureDefault;
    public Vector2 clickPosition = Vector2.zero;
    //cursor prefab
    public GameObject cursorClickImagePrefab; // Prefab with UI Image
    public Canvas canvas; // Reference to the UI canvas
     public float fadeDuration = 1f;
    void Start()
    {
        Vector2 centerHotspot = new Vector2(cursorTextureDefault.width / 2, cursorTextureDefault.height / 2);
        Cursor.SetCursor(cursorTextureDefault, centerHotspot, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 spawnPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out spawnPos);

            GameObject clickImage = Instantiate(cursorClickImagePrefab, canvas.transform);
            RectTransform rect = clickImage.GetComponent<RectTransform>();
            rect.anchoredPosition = spawnPos;

            Image img = clickImage.GetComponent<Image>();
            StartCoroutine(FadeAndDestroy(img, fadeDuration));
        }
    }
    System.Collections.IEnumerator FadeAndDestroy(Image image, float duration)
    {
        float elapsed = 0f;
        Color originalColor = image.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Destroy(image.gameObject);
    }
}
