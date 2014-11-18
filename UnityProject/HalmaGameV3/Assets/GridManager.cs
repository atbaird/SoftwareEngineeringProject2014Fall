using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {
	public Cell[,] cells;
	public List<Player> player1;
	public List<Player> player2;
	public Location[] des1;
	public Location[] des2;

	public int xDim;
	public int yDim;
	public bool readyToBeRead;
	public int turn;
	public int numOfDes;
	public int numOfPlayerPieces;

	Vector3 mouseScreen;

	// Use this for initialization
	void Start () {
		readyToBeRead = false;

		//grid
		turn = 0;
		xDim = 10;
		yDim = 10;
		loadGrid ();

		//players
		int plaX = 3;
		int plaY = 3;

		Player[] players = FindObjectsOfType (typeof(Player)) as Player[];
		Sprite sp1 = Resources.Load <Sprite> ("Sprites/Player_1_Piece");
		Sprite sp2 = Resources.Load <Sprite> ("Sprites/Player_2_Piece");

		foreach (Player play in players) {
			if(play.getDefaultSprite() == sp1) {
				player1.Add (play);

			} else if(play.getDefaultSprite() == sp2) {
				player2.Add (play);

			}
		}

		Location[] plaLoc1 = getBottomLeftOfSize (plaX, plaY);
		Location[] plaLoc2 = getBottomRightOfSize (plaX, plaY);
		numOfPlayerPieces = plaX * plaY;
		placePlayers (plaLoc1, plaLoc2);
		updatePlayerToGridLocations ();

		//destinations
		int desX = 3;
		int desY = 3;
		des1 = getUpperRightOfSize (desX, desY);
		des2 = getUpperLeftOfSize (desX, desY);
		numOfDes = desX * desY;

		readyToBeRead = true;
	}

	public void placePlayers(Location[] locs1, Location[] locs2) {
		for(int i = 0; i < locs1.GetLength (0); i++) {
			player1[i].setGridPosition (locs1[i].getX (), locs1[i].getY ());
		}

		for(int i = 0; i < locs2.GetLength (0); i++) {
			player2[i].setGridPosition (locs2[i].getX (), locs2[i].getY());
		}
	}
	public void updatePlayerToGridLocations() {
		float[] spriteSize = cells [0, 0].getSize ();
		foreach(Player play in player1) {
			int[] gridPos = play.getGridPosition ();
			float x = spriteSize[0] * gridPos[0];
			float y = spriteSize[1] * gridPos[1];
			play.setPosition (x, y);
		}
		foreach(Player play in player2) {
			int[] gridPos = play.getGridPosition ();
			float x = spriteSize[0] * gridPos[0];
			float y = spriteSize[1] * gridPos[1];
			play.setPosition (x,y);
		}
	}

	public void loadGrid() {
		Cell[] cellScripts = FindObjectsOfType (typeof(Cell)) as Cell[];
		cells = new Cell [xDim, yDim];
		int numOfCells = cellScripts.GetLength (0);

		int start = 0;
		for(int i = 0; i < xDim; i++) {
			for(int j = 0; j < yDim; j++) {
				if(start >= numOfCells) {
					break;
				}
				cells[i,j] = cellScripts[start];
				float[] size = cells[i,j].getSize ();
				cells[i,j].setPosition (i * size[0], j * size[1]);
				start++;
			}
			if(start>= numOfCells) {
				break;
			}
		}
	}

	public bool readyToRead() {
		return readyToBeRead;
	}
	public float[] getMaxGridSize() {
		float[] cellSize = cells [0, 0].getSize ();
		cellSize [0] = cellSize [0] * xDim;
		cellSize [1] = cellSize [1] * yDim;
		return cellSize;
	}
	public float[] bottomLeftCellPosition() {
		float[] cellPos = cells [0, 0].getPosition();
		return cellPos;
	}
	public float[] upperRightCellPosition() {
		float[] cellPos = cells [xDim - 1, yDim - 1].getPosition ();
		return cellPos;
	}
	private int[] verifyParam(int x, int y) {
		int[] arr = new int[2];
		if(x > xDim) {
			x = xDim;
		} if(y > yDim) {
			y = yDim;
		}
		arr [0] = x;
		arr [1] = y;
		return arr;
	}
	public Location[] getBottomLeftOfSize(int x, int y) {
		int[] arr = verifyParam (x, y);
		x = arr [0];
		y = arr [1];
		int xY = x * y;
		Location [] locs = new Location[xY];
		int k = 0;
		for(int i = 0; i < x; i++) {
			for(int j = 0; j < y; j++) {
				if(k >= xY) {
					break;
				}
				locs[k] = new Location(x,y);
				k++;
			}
			if(k >= xY) {
				break;
			}
		}
		return locs;
	}
	public Location[] getBottomRightOfSize(int x, int y) {
		int[] arr = verifyParam (x, y);
		x = arr [0];
		y = arr [0];
		int xY = x * y;
		Location [] locs = new Location[xY];
		int k = 0;
		for(int i = xDim-1; i >= xDim-1-x; i--) {
			for(int j = 0; j < y; j++) {
				if(k >= xY) {
					break;
				}
				locs[k] = new Location(x,y);
				k++;
			}
			if(k >= xY) {
				break;
			}
		}
		return locs;
	}
	public Location[] getUpperLeftOfSize(int x, int y) {
		int[] arr = verifyParam (x, y);
		x = arr [0];
		y = arr [0];
		int xY = x * y;
		Location [] locs = new Location[xY];
		int k = 0;
		for(int i = 0; i < x; i++) {
			for(int j = yDim-1; j >= yDim-1-y; j--) {
				if(k >= xY) {
					break;
				}
				locs[k] = new Location(x,y);
				k++;
			}
			if(k >= xY) {
				break;
			}
		}
		return locs;
	}
	public Location[] getUpperRightOfSize(int x, int y) {
		int[] arr = verifyParam (x, y);
		x = arr [0];
		y = arr [0];
		int xY = x * y;
		Location [] locs = new Location[xY];
		int k = 0;
		for(int i = xDim-1; i >= xDim-1-x; i--) {
			for(int j = y; j >= yDim-1-y; j--) {
				if(k >= xY) {
					break;
				}
				locs[k] = new Location(x,y);
				k++;
			}
			if(k >= xY) {
				break;
			}
		}
		return locs;
	}
	
	// Update is called once per frame
	void Update () {

		//mouse
		mouseScreen.x = Input.mousePosition.x;
		mouseScreen.y = Input.mousePosition.y;
		mouseScreen.z = 1;

		Vector3 mouseToWorld = Camera.main.ScreenToWorldPoint (mouseScreen);
		float cellX = mouseToWorld.x;
		float cellY = mouseToWorld.y;

		if(Input.GetMouseButtonDown(0)) {
			Debug.Log ("Starting ray cast");
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity);
			if(hit) {
				Debug.Log ("Hit object: " + hit.collider.gameObject.name);

			}
		}
	}
}

public class Location{
	int x,y;
	public Location () {
		x = -1;
		y = -1;
	}
	public Location (int xV, int yV) {
		setX (xV);
		setY (yV);
	}
	public void setX(int xV) {
		x = xV;
	}
	public void setY(int yV) {
		y = yV;
	}
	public int getX() {
		return x;
	}
	public int getY() {
		return y;
	}
	public string toString() {
		return "{" + x + "," + y + "}";
	}
}
