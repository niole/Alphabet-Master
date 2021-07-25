using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public int x, y;

    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);

    private SpriteRenderer render;

    public void InitXY(int newX, int newY) {
        x = newX;
        y = newY;
    }

    public void Switch(GameObject otherTile) {
        Sprite otherSprite = otherTile.GetComponent<SpriteRenderer>().sprite;
        Sprite thisSprite = GetComponent<SpriteRenderer>().sprite;

        GetComponent<SpriteRenderer>().sprite = otherSprite;
        otherTile.GetComponent<SpriteRenderer>().sprite = thisSprite;
    }

    public void Select() {
        render.color = selectedColor;
    }

    public void Deselect() {
        render.color = Color.white;
    }

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown() {
        Select();

        BoadScript.instance.SwitchWithNearby(x, y);
    }

}
