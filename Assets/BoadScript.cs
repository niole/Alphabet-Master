using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoadScript : MonoBehaviour
{
    public int xCount, yCount;
    public GameObject tileBase;
    public List<Sprite> letters = new List<Sprite>();

    public static BoadScript instance;

    private GameObject[,] tiles;

    private (int, int)? selectedTile = null;

    void Awake() {
        instance = GetComponent<BoadScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateBoard();
    }

    public void SwitchWithNearby(int x, int y) {
        var (otherX, otherY) = selectedTile ?? (x, y);
        if (selectedTile != null && otherX == x && otherY == y) {
            return;
        } else if (selectedTile == null) {
            selectedTile = (x, y);
        } else {
            GameObject thisTile = tiles[x, y];
            GameObject otherTile = tiles[otherX, otherY];

            int xDiff = Mathf.Abs(otherX - x);
            int yDiff = Mathf.Abs(otherY - y);
            bool canSwitchHoriz = xDiff == 1 && yDiff == 0;
            bool canSwitchVert = yDiff == 1 && xDiff == 0;
            bool canSwitch = canSwitchHoriz || canSwitchVert;

            if (canSwitch) {
                thisTile.GetComponent<TileScript>().Switch(otherTile);
                thisTile.GetComponent<TileScript>().Deselect();
                selectedTile = null;
            } else {
                selectedTile = (x, y);
            }

            otherTile.GetComponent<TileScript>().Deselect();

            DestroyMatches();
        }
    }

    /**
     * Destroys all matches in the board
     *
     * if there is a 3 vertical or horizontal match, destroy
     *
     * At each tile, look back three horiz and vert, destroy if match found
     */
    void DestroyMatches() {
        for (int x = 0; x < xCount; x++) {
            for (int y = 0; y < yCount; y++) {

                Sprite thisSprite = tiles[x, y].GetComponent<SpriteRenderer>().sprite;

                List<GameObject> xDestroy = new List<GameObject>(new GameObject[] { tiles[x, y] });
                List<GameObject> yDestroy = new List<GameObject>(new GameObject[] { tiles[x, y] });

                for (int diff = 1; diff < 3; diff++) {
                    int yDiff = y-diff;
                    if (yDiff > -1) {
                        GameObject yGO = tiles[x, yDiff];
                        Sprite ySprite = yGO.GetComponent<SpriteRenderer>().sprite;
                        if (thisSprite == ySprite) {
                            yDestroy.Add(yGO);
                        }
                    }

                    int xDiff = x-diff;
                    if (xDiff > -1) {
                        GameObject xGO = tiles[xDiff, y];
                        Sprite xSprite = xGO.GetComponent<SpriteRenderer>().sprite;
                        if (thisSprite == xSprite) {
                            xDestroy.Add(xGO);
                        }
                    }

                }

                if (xDestroy.Count == 3) {
                    foreach (GameObject go in xDestroy) {
                        go.GetComponent<SpriteRenderer>().sprite = null;
                    }
                }

                if (yDestroy.Count== 3) {
                    foreach (GameObject go in yDestroy) {
                        go.GetComponent<SpriteRenderer>().sprite = null;
                    }
                }

            }
        }

        RefillBoard();
    }

    void RefillBoard() {
        // TODO
    }

    void CreateBoard() {
        float tileWidth = tileBase.GetComponent<SpriteRenderer>().bounds.size.x;
        float tileHeight = tileBase.GetComponent<SpriteRenderer>().bounds.size.y;

        tiles = new GameObject[xCount, yCount];

        float startX = transform.position.x;
        float startY = transform.position.y;

        for (int x = 0; x < xCount; x++) {
            for (int y = 0; y < yCount; y++) {
                Vector3 tileLocation = new Vector3(
                        startX + tileWidth * x,
                        startY + tileHeight * y,
                        0
                );

                GameObject newTile = Instantiate(tileBase, tileLocation, tileBase.transform.rotation);
                newTile.GetComponent<TileScript>().InitXY(x, y);
                newTile.GetComponent<SpriteRenderer>().sprite = GetRandomLetter();
                tiles[x, y] = newTile;
            }
        }
    }

    Sprite GetRandomLetter() {
        return letters[Random.Range(0, 3)];
    }
}
