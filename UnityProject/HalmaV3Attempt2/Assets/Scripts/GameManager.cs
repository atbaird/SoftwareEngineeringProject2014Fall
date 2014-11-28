using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	bool read;
	int xPar, yPar;
	Color[] p1Col, p2Col;
	Piece[] p1, p2;

	// Use this for initialization
	void Start () {
		read = false;

		//Grid
		xPar = 10;
		yPar = 10;

		//Pieces
		GameObject[] player1 = GameObject.FindGameObjectsWithTag ("Player_1_Piece");
		GameObject[] player2 = GameObject.FindGameObjectsWithTag ("Player_2_Piece");

		p1Col = new Color [3];
		p1Col[0] = new Color(0,0,0);
		p1Col[1] = new Color(163,163,163);
		p1Col [2] = new Color (0,253,33);
		p1 = new Piece [9];
		for(int i = 0; i < 9; i++) {
			GameObject o = player1[i];
			Piece p = new Piece(o, p1Col);
			p1[i] = p;
		}

		p2Col = new Color [3];
		p2Col [0] = new Color (255,0,0);
		p2Col [1] = new Color (255,153,0);
		p2Col [2] = new Color (0,223,255);
		p2 = new Piece[9];
		for(int i = 0; i < 9; i++) {
			GameObject o = player2[i];
			Piece p = new Piece(o, p2Col);
			p2[i] = p;
		}

		read = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public float[] getMaxGridSize() {
		float[] arr = new float[2];
		arr[0] = 10;
		arr[1] = 10;
		return arr;
	}
	public bool readyToRead() {
		return read;
	}

	public float[] bottomLeftCellPosition() {
		float[] arr = new float[2];
		arr [0] = 0;
		arr [1] = 0;
		return arr;
	}

	public float[] upperRightCellPosition() {
		float[] arr = new float[2];
		arr [0] = 0;
		arr [1] = 0;
		return arr;
	}
}



public class Location {
	float x,y;
	public Location() {
		x = 0;
		y = 0;
	}
	public Location(float xV, float yV) {
		x = xV;
		y = yV;
	}
	public float[] getLocation() {
		float[] arr = new float[2];
		arr [0] = x;
		arr [1] = y;
		return arr;
	}
	public float getX() {
		return x;
	}
	public float getY() {
		return y;
	}
	public bool compareLocation(Location loc) {
		if(loc.getX () == this.getX () && loc.getY () == this.getY ()) {
			return true;
		}
		return false;
	}
	public bool compareLocation(float xV, float yV) {
		Location loc = new Location (xV, yV);
		return compareLocation (loc);
	}
}

public class Piece {
	GameObject obj;
	Location l;
	int isFrozen;
	Color[] colors;


	public Piece(GameObject o, Color[] cols) {
		colors = cols;
		obj = 0;
		isFrozen = 0;
		Transform t = o.transform;
		float x = t.position.x;
		float y = t.position.y;
		l = new Location (x,y);
	}
	public Piece(GameObject o, Location loc, Color[] cols) {
		obj = o;
		isFrozen = 0;
		l = loc;
		colors = cols;
	} public Piece(GameObject o, float x, float y, Color[] cols) {
		l = new Location (x, y);
		isFrozen = 0;
		obj = 0;
	}
	public Location getLoc() {
		return l;
	}
	public GameObject getObject() {
		return obj;
	}
	public bool compareLocation(Location loc) {
		return l.compareLocation (loc);
	}
	public void setLocation(Location loc) {
		Vector3 temp = new Vector3 (loc.getX (), loc.getY (), 0);
		obj.transform.position = temp;
		l = loc;
	}
	public bool isThisFrozen() {
		if(isFrozen > 0) {
			return true;
		} else {
			return false;
		}
	}
	public void setFrozen(int froze) {
		if(froze >= 0) {
			isFrozen = froze;
		} else {
			isFrozen = froze * -1;
		}
	}
	public void decrementFrozen() {
		if(isFrozen > 0) {
			isFrozen--;
		}
	}
}

public class Player {
	Piece[] pieces;

	public Player(Piece[] pics) {
		pieces = pics;
	}
	public Piece[] getPieces() {
		return pieces;
	}
	public bool PieceInLocation(Location loc) {
		bool isThere = false;
		foreach(Piece pic in pieces) {
			isThere = pic.compareLocation (loc);
			if(isThere == true) {
				break;
			}
		}
		return isThere;
	}

}