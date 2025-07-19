using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D cursorTextureDefault;
    public Vector2 clickPosition = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        Vector2 centerHotspot = new Vector2(cursorTextureDefault.width / 2, cursorTextureDefault.height / 2);
        Cursor.SetCursor(cursorTextureDefault, centerHotspot, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
