using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	bool read;
	int xPar, yPar;
	Location[] d1, d2;
	Color[] p1Col, p2Col;
	Piece[] p1, p2;
	int numP1, numP2, numD1, numD2;
	Piece sel;

	// Use this for initialization
	void Start () {
		read = false;

		//Destination Locations
		numD1 = 9;
		numD2 = 9;
		d1 = new Location[9];
		d2 = new Location[9];
		int k = 0;

		for(int i = 9; i >= 7; i--) {
			for(int j = 9; j >= 7; j--) {
				d1[k] = new Location(j, i);
				k++;
			}
		}

		k = 0;
		for(int i = 9; i >= 7; i--) { //y
			for(int j = 0; j <= 2; j++) { //x
				d2[k] = new Location(j,i);
				k++;
			}
		}

		//Grid
		xPar = 10;
		yPar = 10;

		//Pieces
		GameObject[] player1 = GameObject.FindGameObjectsWithTag ("Player_1_Piece");
		GameObject[] player2 = GameObject.FindGameObjectsWithTag ("Player_2_Piece");
		numP1 = 9;
		numP2 = 9;

		sel = null;
		p1Col = new Color [3];
		p1Col[0] = new Color(255f,255f,255f);
		p1Col[1] = new Color(139f,200f,139f);
		p1Col [2] = new Color (0f,253f,33f);
		p1 = new Piece [9];
		for(int i = 0; i < numP1; i++) {
			GameObject o = player1[i];
			Piece p = new Piece(o, p1Col);
			p1[i] = p;
		}

		p2Col = new Color [3];
		p2Col [0] = new Color (255f,0f,0f);
		p2Col [1] = new Color (255f,153f,0f);
		p2Col [2] = new Color (0f,223f,255f);
		p2 = new Piece[9];
		for(int i = 0; i < numP2; i++) {
			GameObject o = player2[i];
			Piece p = new Piece(o, p2Col);
			p2[i] = p;
		}

		read = true;
	}
	Vector3 getMousePos() {
		Vector3 mPos = Input.mousePosition;
		mPos.z = 10.0f;
		mPos = Camera.main.ScreenToWorldPoint (mPos);
		return mPos;
	}
	bool selectFromPlayer1Only(Vector3 mPos) {
		for(int i = 0; i < numP1; i++) {
			Location loc = p1[i].getLoc ();
			if(loc.getX () == Mathf.Floor (mPos.x) && loc.getY () == Mathf.Floor (mPos.y)) {
				if(sel != null) {
					sel.setColor (0);
					sel = null;
				}
				sel = p1[i];
				sel.setColor (1);
				return true;
			}
		}
		return false;
	}
	bool selectFromPlayer2Only(Vector3 mPos) {
		for(int i = 0; i < numP2; i++) {
			Location loc = p2[i].getLoc ();
			if(loc.getX () == Mathf.Floor (mPos.x) && loc.getY () == Mathf.Floor (mPos.y)) {
				if(sel != null) {
					sel.setColor (0);
					sel = null;
				}
				sel = p2[i];
				sel.setColor (1);
				return true;
			}
		}
		return false;
	}
	bool determineIfClickPiece(Vector3 mPos) {
		bool selected = selectFromPlayer1Only (mPos);
		if(selected == false) {
			selected = selectFromPlayer2Only (mPos);
		}
		return selected;
	}
	bool determineIfPieceBlocks(Vector3 mPos) {
		if(mPos.x < 0 || mPos.y < 0 || mPos.x >= xPar || mPos.y >= yPar) {
			return true;
		}
		bool blocked = false;
		for(int i = 0; i < numP1; i++) {
			if(blocked == true) {
				break;
			}
			Location loc = p1[i].getLoc ();
			if(loc.getX () == Mathf.Floor (mPos.x)
			   && loc.getY() == Mathf.Floor (mPos.y)) {
				blocked = true;
			}
		}
		for(int i = 0; i < numP2; i++) {
			if(blocked == true) {
				break;
			}
			Location loc = p1[i].getLoc ();
			if(loc.getX () == Mathf.Floor (mPos.x)
			   && loc.getY() == Mathf.Floor (mPos.y)) {
				blocked = true;
			}
		}
		return blocked;
	}

	bool determinePieceJump(Vector3 mPos) {
		Location selLoc = sel.getLoc ();
		float xDif = Mathf.Floor (mPos.x) - selLoc.getX ();
		float yDif = Mathf.Floor (mPos.y) - selLoc.getY ();
		if(Mathf.Abs (xDif) <= 2 && Mathf.Abs (yDif) <= 2 
		   && mPos.x < xPar && mPos.y < yPar && mPos.x >= 0 && mPos.y >= 0) {
			float xOne = mPos.x - (xDif/2);
			float yOne = mPos.y - (yDif/2);
			Vector3 stepSpace = new Vector3(xOne, yOne, 10f);
			bool PieceBetween = determineIfPieceBlocks (stepSpace);
			if(PieceBetween == true) {
				bool isBlocked = false;
				for(int i = 0; i < numP1; i++) {
					if(isBlocked == true) {
						break;
					}
					Location loc = p1[i].getLoc ();
					if(Mathf.Floor (mPos.x) == loc.getX () && Mathf.Floor (mPos.y) == loc.getY ()) {
						isBlocked = true;
					}
				}
				for(int i = 0; i < numP2; i++) {
					if(isBlocked == true) {
						break;
					}
					Location loc = p2[i].getLoc ();
					if(Mathf.Floor (mPos.x) == loc.getX () && Mathf.Floor (mPos.y) == loc.getY ()) {
						isBlocked = true;
					}
				}
				if(isBlocked == false) {
					Location loc = new Location(Mathf.Floor (mPos.x), Mathf.Floor (mPos.y));
					sel.setLocation (loc);
					return true;
				}
			}
		}
		return false;
	}
	bool determineIfLocWithinStepRange(Vector3 mPos) {
		float x = (int)(mPos.x) - sel.getLoc ().getX ();
		float y = (int)(mPos.y) - sel.getLoc ().getY ();
		if(Mathf.Abs (x) <= 1 && Mathf.Abs (y) <=1 
		   && mPos.x <xPar && mPos.y < yPar && mPos.x >=0 && mPos.y >=0) {
			bool occupied = false;
			for(int i = 0; i < numP1; i++) {
				if(occupied == true) {
					break;
				}
				Location loc = p1[i].getLoc ();
				if((int)(mPos.x) == loc.getX() && (int)(mPos.y) == loc.getY ()) {
					occupied = true;
				}
			}
			for(int i = 0; i < numP2; i++) {
				if(occupied == true) {
					break;
				}
				Location loc = p2[i].getLoc ();
				if((int)(mPos.x) == loc.getX() && (int)(mPos.y) == loc.getY ()) {
					occupied = true;
				}
			}
			if(occupied == false) {
				Location loc = new Location((int)(mPos.x),(int)(mPos.y));
				sel.setLocation (loc);
				sel.setColor (0);
				sel = null;
				return true;
			}
		}
		return false;
	}

	void resetToBasicColor() {
		for(int i = 0; i <numP1; i++) {
			p1[i].setColor (0);
		}
		for(int i =0; i < numP2; i++) {
			p2[i].setColor (0);
		}
	}

	void testForDest() {
		resetToBasicColor ();
		for(int i = 0; i < numD1; i++) {
			for(int j = 0; j < numP1; j++) {
				Location loc = p1[j].getLoc ();
				if(loc.getX () == d1[i].getX () && loc.getY () == d1[i].getY ()) {
					p1[j].setColor (2);
				}
			}
		}

		for(int i = 0; i < numD2; i++) {
			for(int j = 0; j < numP2; j++) {
				
				Location loc = p2[j].getLoc ();
				if(loc.getX () == d2[i].getX () && loc.getY () == d2[i].getY ()) {
					p2[j].setColor (2);
				}
			}
		}

		if(sel != null) {
			sel.setColor (1);
		}
	}

	// Update is called once per frame
	void Update () {
		testForDest ();

		if(Input.GetMouseButtonDown (0)) {
			Vector3 mPos = getMousePos ();
			if(sel == null) {
				determineIfClickPiece (mPos);
				if(sel != null) {
					sel.setColor (1);
				}
			} else {
				bool isPiece = determineIfClickPiece (mPos);
				if(isPiece == false) {
					bool isStep = determineIfLocWithinStepRange (mPos);
					if(isStep == false) {
						determinePieceJump (mPos);
					} else {
						if(sel != null) {
							sel.setColor (0);
							sel = null;
						}
					}
				} else {
					sel.setColor (1);
				}
			}
		}
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
		obj = o;
		isFrozen = 0;
		Transform t = o.transform;
		float x = t.position.x;
		float y = t.position.y;
		l = new Location (x, y);
	}
	public void setColor(int i) {
		obj.GetComponent<SpriteRenderer>().color = colors [i];
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