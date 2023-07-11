using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardMode : MonoBehaviour
{
    private Tile[,] tiles;

    private string[,] tileStrings =
    {
        {"A1", "A2", "A3", "A4", "A5", "A6"},
        {"B1", "B2", "B3", "B4", "B5", "B6"},
        {"C1", "C2", "C3", "C4", "C5", "C6"},
        {"D1", "D2", "D3", "D4", "D5", "D6"},
        {"E1", "E2", "E3", "E4", "E5", "E6"},
        {"F1", "F2", "F3", "F4", "F5", "F6"},
        {"G1", "G2", "G3", "G4", "G5", "G6"},
        {"H1", "H2", "H3", "H4", "H5", "H6"},
    };

    [SerializeField] InputReader inputReader;
    [SerializeField] GameObject knight;
    [SerializeField] GameObject highlight;

    private Tile currentMovingTile;
    private bool isSelectingMove;

    private List<Vector2Int> currentPossibleMoves = new List<Vector2Int>();

    void Start()
    {
        inputReader.ClickEvent += HandleClick;
        inputReader.RightClickEvent += HandleRightClick;

        tiles = new Tile[6, 8];
        for(int i = 0; i < 6; i++)
            for(int j = 0; j < 8; j++)
                tiles[i, j] = new Tile(i, j, GameObject.Find(tileStrings[j,i]));

        tiles[0, 0].SetUp(Tile.Type.Rook, Tile.Team.White);
        tiles[1, 0].SetUp(Tile.Type.Knight, Tile.Team.White);
        tiles[2, 0].SetUp(Tile.Type.Bishop, Tile.Team.White);
        tiles[3, 0].SetUp(Tile.Type.Bishop, Tile.Team.White);
        tiles[4, 0].SetUp(Tile.Type.Knight, Tile.Team.White);
        tiles[5, 0].SetUp(Tile.Type.Rook, Tile.Team.White);

        tiles[0, 7].SetUp(Tile.Type.Rook, Tile.Team.Black);
        tiles[1, 7].SetUp(Tile.Type.Knight, Tile.Team.Black);
        tiles[2, 7].SetUp(Tile.Type.Bishop, Tile.Team.Black);
        tiles[3, 7].SetUp(Tile.Type.Bishop, Tile.Team.Black);
        tiles[4, 7].SetUp(Tile.Type.Knight, Tile.Team.Black);
        tiles[5, 7].SetUp(Tile.Type.Rook, Tile.Team.Black);

        for(int i = 0; i < 6; i++)
        {
            tiles[i, 1].SetUp(Tile.Type.Pawn, Tile.Team.White);
            tiles[i, 6].SetUp(Tile.Type.Pawn, Tile.Team.Black);
        }

        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                InstantiatePiece(i, j);
            }
        }
    }

    void Update()
    {
        //Debug.Log(isSelectingMove);
    }

    private void HandleClick()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit))
        {
            if(hit.collider != null)
            {
                if(hit.collider.tag == "Tile")
                {
                    Vector2Int tilePosition = GetTilePosition(hit.collider.name);
                    Tile tile = tiles[tilePosition.x, tilePosition.y];
                    
                    if(isSelectingMove)
                    {
                        if(currentPossibleMoves.Contains(tile.coordinates))
                        {
                            // capture
                            if(IsMoveACapture(currentMovingTile, tile))
                            {
                                CapturePiece(currentMovingTile, tile);
                            }
                            // move
                            else
                            {
                                MovePiece(currentMovingTile, tile);
                            }

                        } 
                        else
                        {
                            print("doesnt' contain");
                        }
                        
                        isSelectingMove = false;
                        ClearHighlights();
                        currentPossibleMoves.Clear();
                        currentMovingTile = null;
                    }
                    else
                    {
                        if(tile.type != Tile.Type.None)
                        {
                            currentMovingTile = tile;
                            currentPossibleMoves = GetPossibleMoves(tile);
                            HighlightPossibleMoves();
                            isSelectingMove = true;
                        }
                    }
                    
                }
            }
        }
        else
        {
            ClearHighlights();
            isSelectingMove = false;
            currentPossibleMoves.Clear();
            currentMovingTile = null;
        }
    }

    private void HandleRightClick()
    {

    }

    private Vector2Int GetTilePosition(string tileName)
    {
        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if(tileStrings[j, i] == tileName)
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        return new Vector2Int(0,0);
    }

    public void InstantiatePiece(int i, int j)
    {
        Tile tile = tiles[i,j];

        if(tile.team == Tile.Team.None) return;

        tile.piece = Instantiate(knight, tile.model.transform.position, Quaternion.identity);
    }

    public List<Vector2Int> GetPossibleMoves(Tile tile)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        if(tile.type == Tile.Type.Rook)
        {
            // up
            for(int i = tile.coordinates.y + 1; i < tiles.GetLength(1); i++)
            {
                Tile nextTile = tiles[tile.coordinates.x, i];
                if(nextTile.type == Tile.Type.None)
                {
                    possibleMoves.Add(nextTile.coordinates);
                    //highlightTile(nextTile.coordinates);
                } else if(nextTile.team == tile.team)
                {
                    break;
                } else if(nextTile.team != tile.team)
                {
                    possibleMoves.Add(nextTile.coordinates);
                    break;
                }
            }

            // down
            for(int i = tile.coordinates.y - 1; i >= 0; i--)
            {
                Tile nextTile = tiles[tile.coordinates.x, i];
                if(nextTile.type == Tile.Type.None)
                {
                    possibleMoves.Add(nextTile.coordinates);
                    //highlightTile(nextTile.coordinates);
                } else if(nextTile.team == tile.team)
                {
                    break;
                } else if(nextTile.team != tile.team)
                {
                    possibleMoves.Add(nextTile.coordinates);
                    break;
                }
            }

            // left
            for(int i = tile.coordinates.x - 1; i >= 0; i--)
            {
                Tile nextTile = tiles[i, tile.coordinates.y];
                if(nextTile.type == Tile.Type.None)
                {
                    possibleMoves.Add(nextTile.coordinates);
                    //highlightTile(nextTile.coordinates);
                } else if(nextTile.team == tile.team)
                {
                    break;
                } else if(nextTile.team != tile.team)
                {
                    possibleMoves.Add(nextTile.coordinates);
                    break;
                }
            }

            // right
            for(int i = tile.coordinates.x + 1; i < tiles.GetLength(0); i++)
            {
                Tile nextTile = tiles[i, tile.coordinates.y];
                if(nextTile.type == Tile.Type.None)
                {
                    possibleMoves.Add(nextTile.coordinates);
                    //highlightTile(nextTile.coordinates);
                }
                else if(nextTile.team == tile.team)
                {
                    break;
                }
                else if(nextTile.team != tile.team)
                {
                    possibleMoves.Add(nextTile.coordinates);
                    break;
                }
            }

        }
        else if(tile.type == Tile.Type.Knight)
        {
            int iLeftOnce = tile.coordinates.x - 1;
            int iLeftTwice = tile.coordinates.x - 2;
            int iRightOnce = tile.coordinates.x + 1;
            int iRightTwice = tile.coordinates.x + 2;

            int jUpOnce = tile.coordinates.y + 1;
            int jUpTwice = tile.coordinates.y + 2;
            int jDownOnce = tile.coordinates.y - 1;
            int jDownTwice = tile.coordinates.y - 2;

            if(iLeftOnce >= 0)
            {
                if(jUpTwice < tiles.GetLength(1))
                {
                    Tile nextTile = tiles[iLeftOnce, jUpTwice];
                    if(nextTile.type == Tile.Type.None || nextTile.team != tile.team)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                    }
                }
                if(jDownTwice >= 0)
                {
                    Tile nextTile = tiles[iLeftOnce, jDownTwice];
                    if(nextTile.type == Tile.Type.None || nextTile.team != tile.team)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                    }
                }
            }
            if(iLeftTwice >= 0)
            {
                if(jUpOnce < tiles.GetLength(1))
                {
                    Tile nextTile = tiles[iLeftTwice, jUpOnce];
                    if(nextTile.type == Tile.Type.None || nextTile.team != tile.team)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                    }
                }
                if(jDownOnce >= 0)
                {
                    Tile nextTile = tiles[iLeftTwice, jDownOnce];
                    if(nextTile.type == Tile.Type.None || nextTile.team != tile.team)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                    }
                }
            }
            if(iRightOnce < tiles.GetLength(0))
            {
                if(jUpTwice < tiles.GetLength(1))
                {
                    Tile nextTile = tiles[iRightOnce, jUpTwice];
                    if(nextTile.type == Tile.Type.None || nextTile.team != tile.team)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                    }
                }
                if(jDownTwice >= 0)
                {
                    Tile nextTile = tiles[iRightOnce, jDownTwice];
                    if(nextTile.type == Tile.Type.None || nextTile.team != tile.team)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                    }
                }
            }
            if(iRightTwice < tiles.GetLength(0))
            {
                if(jUpOnce < tiles.GetLength(1))
                {
                    Tile nextTile = tiles[iRightTwice, jUpOnce];
                    if(nextTile.type == Tile.Type.None || nextTile.team != tile.team)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                    }
                }
                if(jDownOnce >= 0)
                {
                    Tile nextTile = tiles[iRightTwice, jDownOnce];
                    if(nextTile.type == Tile.Type.None || nextTile.team != tile.team)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                    }
                }
            }
        }
        else if(tile.type == Tile.Type.Bishop)
        {
            // up right
            for(int i = tile.coordinates.x + 1; i < tiles.GetLength(0); i++)
            {
                for(int j = tile.coordinates.y + 1; j < tiles.GetLength(1); j++)
                {
                    if(i - tile.coordinates.x != j - tile.coordinates.y) continue;
                    Tile nextTile = tiles[i, j];
                    if(nextTile.type == Tile.Type.None)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                        //highlightTile(nextTile.coordinates);
                    } else if(nextTile.team == tile.team)
                    {
                        goto upright;
                    } else if(nextTile.team != tile.team)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                        goto upright;
                    }
                }
            }
        upright: { }

            // down right
            for(int i = tile.coordinates.x + 1; i < tiles.GetLength(0); i++)
            {
                for(int j = tile.coordinates.y - 1; j >= 0; j--)
                {
                    if(Mathf.Abs(i - tile.coordinates.x) != Mathf.Abs(j - tile.coordinates.y)) continue;
                    Tile nextTile = tiles[i, j];
                    if(nextTile.type == Tile.Type.None)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                        //highlightTile(nextTile.coordinates);
                    } else if(nextTile.team == tile.team)
                    {
                        goto downright;
                    } else if(nextTile.team != tile.team)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                        goto downright;
                    }
                }
            }
        downright: { }

            // up left
            for(int i = tile.coordinates.x - 1; i >= 0; i--)
            {
                for(int j = tile.coordinates.y + 1; j < tiles.GetLength(1); j++)
                {
                    if(Mathf.Abs(i - tile.coordinates.x) != Mathf.Abs(j - tile.coordinates.y)) continue;
                    Tile nextTile = tiles[i, j];
                    if(nextTile.type == Tile.Type.None)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                        //highlightTile(nextTile.coordinates);
                    } else if(nextTile.team == tile.team)
                    {
                        goto upleft;
                    } else if(nextTile.team != tile.team)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                        goto upleft;
                    }
                }
            }
        upleft: { }

            // down left
            for(int i = tile.coordinates.x - 1; i >= 0; i--)
            {
                for(int j = tile.coordinates.y - 1; j >= 0; j--)
                {
                    if(i - tile.coordinates.x != j - tile.coordinates.y) continue;
                    Tile nextTile = tiles[i, j];
                    if(nextTile.type == Tile.Type.None)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                        //highlightTile(nextTile.coordinates);
                    }
                    else if(nextTile.team == tile.team)
                    {
                        goto downleft;
                    }
                    else if(nextTile.team != tile.team)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                        goto downleft;
                    }
                }
            }
        downleft: { }

        }
        else if(tile.type == Tile.Type.Pawn)
        {
            int upOnce = tile.coordinates.y + 1;
            int downOnce = tile.coordinates.y - 1;
            int leftOnce = tile.coordinates.x - 1;
            int straight = tile.coordinates.x;
            int rightOnce = tile.coordinates.x + 1;

            if(tile.team == Tile.Team.White)
            {
                if(upOnce < tiles.GetLength(1))
                {
                    Tile nextTile = tiles[straight, upOnce];
                    if(nextTile.type == Tile.Type.None)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                    }

                    if(leftOnce >= 0)
                    {
                        nextTile = tiles[leftOnce, upOnce];
                        if(nextTile.type != Tile.Type.None && nextTile.team != tile.team)
                        {
                            possibleMoves.Add(nextTile.coordinates);
                        }
                    }
                    if(rightOnce < tiles.GetLength(0))
                    {
                        nextTile = tiles[rightOnce, upOnce];
                        if(nextTile.type != Tile.Type.None && nextTile.team != tile.team)
                        {
                            possibleMoves.Add(nextTile.coordinates);
                        }
                    }
                }
            }
            else if(tile.team == Tile.Team.Black)
            {
                if(downOnce >= 0)
                {
                    Tile nextTile = tiles[straight, downOnce];
                    if(nextTile.type == Tile.Type.None)
                    {
                        possibleMoves.Add(nextTile.coordinates);
                    }

                    if(leftOnce >= 0)
                    {
                        nextTile = tiles[leftOnce, downOnce];
                        if(nextTile.type != Tile.Type.None && nextTile.team != tile.team)
                        {
                            possibleMoves.Add(nextTile.coordinates);
                        }
                    }
                    if(rightOnce < tiles.GetLength(0))
                    {
                        nextTile = tiles[rightOnce, downOnce];
                        if(nextTile.type != Tile.Type.None && nextTile.team != tile.team)
                        {
                            possibleMoves.Add(nextTile.coordinates);
                        }
                    }
                }
            }

            if(tile.firstMove)
            {
                int upTwice = tile.coordinates.y + 2;
                int downTwice = tile.coordinates.y - 2;

                if(tile.team == Tile.Team.White)
                {
                    Tile nextTile = tiles[straight, upOnce];
                    if(nextTile.type == Tile.Type.None)
                    {
                        nextTile = tiles[straight, upTwice];
                        if(nextTile.type == Tile.Type.None)
                        {
                            possibleMoves.Add(nextTile.coordinates);
                        }
                    }
                }
                else if(tile.team == Tile.Team.Black)
                {
                    Tile nextTile = tiles[straight, downOnce];
                    if(nextTile.type == Tile.Type.None)
                    {
                        nextTile = tiles[straight, downTwice];
                        if(nextTile.type == Tile.Type.None)
                        {
                            possibleMoves.Add(nextTile.coordinates);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

    private void HighlightPossibleMoves()
    {
        foreach(Vector2Int move in currentPossibleMoves)
        {
            GameObject tileModel = tiles[move.x, move.y].model;// GameObject.Find(tileStrings[move.y, move.x]);
            Quaternion rotation = GameObject.Find("Model_Prop_Board").transform.rotation;
            GameObject hl = Instantiate(highlight, tileModel.transform.position, rotation);
            hl.tag = "Highlight";
            hl.transform.position = new Vector3(hl.transform.position.x, .192f, hl.transform.position.z);
        }
    }

    private void ClearHighlights()
    {
        GameObject[] hls = GameObject.FindGameObjectsWithTag("Highlight");
        for(int i = 0; i < hls.Length; i++)
        {
            Destroy(hls[i]);
        }
    }

    public bool IsMoveACapture(Tile from, Tile to)
    {
        return from.type != Tile.Type.None && to.type != Tile.Type.None && from.team != to.team;
    }

    private void MovePiece(Tile from, Tile to)
    {
        to.type = from.type;
        to.team = from.team;
        to.piece = from.piece;
        from.firstMove = false;
        from.type = Tile.Type.None;
        from.team = Tile.Team.None;
        from.piece = null;
        to.piece.gameObject.transform.position = to.model.transform.position;
    }

    public void CapturePiece(Tile from, Tile to)
    {
        to.type = from.type;
        to.team = from.team;
        if(to.piece != null)
            Destroy(to.piece.gameObject);
        to.piece = from.piece;
        from.firstMove = false;
        from.type = Tile.Type.None;
        from.team = Tile.Team.None;
        from.piece = null;
        to.piece.gameObject.transform.position = to.model.transform.position;
    }

    /*public GameObject TileDataToPrefab(Tile tile)
    {
        return whiteRook;
        if(tile.type == Tile.Type.Rook)
        {
            if(tile.team == Tile.Team.White)
            {
                return whiteRook;
            } else if(tile.team == Tile.Team.Black)
            {
                return blackRook;
            }
        } else if(tile.type == Tile.Type.Knight)
        {
            if(tile.team == Tile.Team.White)
            {
                return whiteKnight;
            } else if(tile.team == Tile.Team.Black)
            {
                return blackKnight;
            }
        } else if(tile.type == Tile.Type.Bishop)
        {
            if(tile.team == Tile.Team.White)
            {
                return whiteBishop;
            } else if(tile.team == Tile.Team.Black)
            {
                return blackBishop;
            }
        } else if(tile.type == Tile.Type.Pawn)
        {
            if(tile.team == Tile.Team.White)
            {
                return whitePawn;
            } else if(tile.team == Tile.Team.Black)
            {
                return blackPawn;
            }
        }
        return null;
    }*/
}
