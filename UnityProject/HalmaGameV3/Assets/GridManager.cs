using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {
	public Cell[,] cells;
	public List<Player> player1;
	public List<Player> player2;
	public Location[] des1;
	public Location[] des2;
	public Color[] colors;
	
	public int xDim;
	public int yDim;
	public bool readyToBeRead;
	public int turn;
	public int numOfDes;
	public int numOfPlayerPieces;
	public Transform cellCell;
	public Transform playerplay;
	
	Vector3 mouseScreen;
	
	// Use this for initialization
	void Start () {
		readyToBeRead = false;

		//Colors
		colors = new Color[6];
		colors[0] = new Color(255,255,255); //p1
		colors[1] = new Color(160,160,160); //p1 select
		colors[2] = new Color(0,250,23); //p1 won
		colors[3] = new Color(255,0,0); //p2
		colors[4] = new Color(255, 164, 0); //p2 select
		colors[5] = new Color(0,255,244); //p2 won

		//grid
		turn = 0;
		xDim = 10;
		yDim = 10;
		loadGrid ();
		
		//players
		int plaX = 3;
		int plaY = 3;
		
		Player[] players = FindObjectsOfType (typeof(Player)) as Player[];
		
		foreach (Player play in players) {
			if(play.getColor() == colors[0]) {
				play.name = "Player_1_Piece";
				player1.Add (play);
				
			} else if(play.getColor() == colors[3]) {
				play.name = "Player_2_Piece";
				player2.Add (play);
				
			}
		}
		if(players.GetLength(0) < (plaX * plaY * 2)) {
			for(int i = 0; i < (plaX*plaY*2 -players.GetLength(0)); i++) {
				Vector3 vec = new Vector3(0,0,0);
				GameObject obj = Instantiate (playerplay, vec, Quaternion.identity) as GameObject;
			}
			players = FindObjectsOfType (typeof(Player)) as Player[];
		}
		foreach (Player play in players) {
			if(player1.Count < (plaX * plaY)) {
				player1.Add (play);
			} else if(player2.Count < (plaX * plaY)) {
				play.setColor (colors[3]);
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
		int j = 0;
		int limit = player1.Count;
		for(int i = 0; i < locs1.GetLength (0); i++) {
			if(j < limit) {
				player1[j].setGridPosition (locs1[i].getX (), locs1[i].getY ());
				j++;
				Debug.Log(locs1[i].getX () + " " + locs1[i].getY());
			}
		}
		j = 0;
		limit = player2.Count;
		for(int i = 0; i < locs2.GetLength (0); i++) {
			if(j < limit) {
				player2[j].setGridPosition (locs2[i].getX (), locs2[i].getY());
				j++;
			}
		}
	}
	public void updatePlayerToGridLocations() {
		float[] spriteSize = new float[2];
		spriteSize [0] = cellCell.renderer.bounds.size.x;
		spriteSize [1] = cellCell.renderer.bounds.size.y;
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

		while(cellScripts.GetLength(0) < (xDim * yDim)) {
			Vector3 vec = new Vector3(0, 0, 0);
			GameObject obj = Instantiate (cellCell, vec, Quaternion.identity) as GameObject;
			cellScripts = FindObjectsOfType (typeof(Cell)) as Cell[];
		}
		Cell[] cellScript = FindObjectsOfType (typeof(Cell)) as Cell[];

		float[] size = cellScript[0].getSize ();
		int start = 0;
		for(int i = 0; i < xDim; i++) {
			for(int j = 0; j < yDim; j++) {
				float posX = i * size[0];
				float posY = j * size[1];
				if(start >= numOfCells) {
					break;
				} else {
					cells[i,j] = cellScript[start];
					cells[i,j].setPosition (posX, posY);
					start++;
				}
			}
			if(start >= numOfCells) {
				break;
			}
		}
	}
	
	public bool readyToRead() {
		return readyToBeRead;
	}
	public float[] getMaxGridSize() {
		float[] cellSize = new float[2];
		cellSize [0] = cellCell.renderer.bounds.size.x * xDim;
		cellSize [1] = cellCell.renderer.bounds.size.y * yDim;
		return cellSize;
	}
	public float[] bottomLeftCellPosition() {
		float[] cellPos = new float[2];
		cellPos [0] = 0;
		cellPos [1] = 0;
		return cellPos;
	}
	public float[] upperRightCellPosition() {
		float[] cellPos = new float[2];
		cellPos [0] = cellCell.renderer.bounds.size.x * (xDim - 1);
		cellPos [1] = cellCell.renderer.bounds.size.y * (yDim - 1);
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
				locs[k] = new Location(i,j);
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
				locs[k] = new Location(i,j);
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
				locs[k] = new Location(i,j);
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
				locs[k] = new Location(i,j);
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
