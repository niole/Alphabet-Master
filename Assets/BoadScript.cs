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
        bool destroyedThings = false;

        for (int x = 0; x < xCount; x++) {
            for (int y = 0; y < yCount; y++) {

                Sprite thisSprite = tiles[x, y].GetComponent<SpriteRenderer>().sprite;

                if (thisSprite != null) {

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
                        destroyedThings = true;
                        foreach (GameObject go in xDestroy) {
                            go.GetComponent<SpriteRenderer>().sprite = null;
                        }
                    }

                    if (yDestroy.Count== 3) {
                        destroyedThings = true;
                        foreach (GameObject go in yDestroy) {
                            go.GetComponent<SpriteRenderer>().sprite = null;
                        }
                    }

                }

            }
        }

        ShiftDown();
        RefillBoard();

        if (destroyedThings) {
            DestroyMatches();
        }
    }

    /**
     * Shifts the tiles down in the board in order to fill holes in the middle of the board
     *
     * Starts at the bottom, goes column by column, if it finds a hole, it goes up until it sees a tile
     * that has a sprite, and then swaps with empty spot
     *
     * two indexes: the next empty one, the next full one, if no next full one, because all empty
     * or we hit the top, we move on to the next column
     */
    void ShiftDown() {
        for (int col = 0; col < xCount; col++) {
            int emptyIndex = 0;
            int fullIndex = 0;
            while (emptyIndex < (yCount-1) && fullIndex < yCount) {
                // only continue if emptyIndex can swap with fullIndex

                while(emptyIndex < (yCount-1) && tiles[col, emptyIndex].GetComponent<SpriteRenderer>().sprite != null) {
                    // look for an empty tile
                    // look until you find one or run off the board
                    emptyIndex++;
                }

                fullIndex = emptyIndex + 1;
                while (fullIndex < (yCount-1) && tiles[col, fullIndex].GetComponent<SpriteRenderer>().sprite == null) {
                    // look for a full tile that is higher up than empty tile
                    // look until you find one or run off the board
                    fullIndex++;
                }

                if (fullIndex < yCount && emptyIndex < yCount) {
                    // if neither indexes have run off of the board,
                    // then we can do a shift swap

                    if (tiles[col, emptyIndex].GetComponent<SpriteRenderer>().sprite == null && tiles[col, fullIndex].GetComponent<SpriteRenderer>().sprite != null) {
                        // switch
                        tiles[col, emptyIndex].GetComponent<SpriteRenderer>().sprite = tiles[col, fullIndex].GetComponent<SpriteRenderer>().sprite;
                        tiles[col, fullIndex].GetComponent<SpriteRenderer>().sprite = null;
                    }

                }

                emptyIndex++;
            }
        }
    }

    /**
     * Refills the holes in the top of the board
     */
    void RefillBoard() {
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
        return letters[Random.Range(0, letters.Count - 1)];
    }
}
