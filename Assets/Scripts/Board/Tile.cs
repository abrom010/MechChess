using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
    public enum Type {
        None, Rook, Knight, Bishop, Pawn
    }

    public enum Team {
        None, White, Black
    }

    public GameObject model;
    public Type type;
    public Team team;
    public Vector2Int coordinates;
    public GameObject piece;
    public bool firstMove;

    public Tile(int i, int j, GameObject model) {
        this.model = model;
        this.type = Type.None;
        this.team = Team.None;
        this.coordinates = new Vector2Int(i, j);
        this.piece = null;
        this.firstMove = false;
    }

    public void SetUp(Type type, Team team) {
        this.type = type;
        this.team = team;
        if(this.type == Type.Pawn) {
            this.firstMove = true;
        }
    }
}
