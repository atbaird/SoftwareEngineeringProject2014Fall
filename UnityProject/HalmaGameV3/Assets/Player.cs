using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private Sprite defaultSprite, selected, won;
	float x, y;
	int xVar, yVar;
	// Use this for initialization
	void Start () {
		x = 0;
		y = 0;
		xVar = 0;
		yVar = 0;
		this.transform.position = new Vector3 (x, y, 1);
		SpriteRenderer render = GetComponent<SpriteRenderer> ();
		defaultSprite = render.sprite;
		selected = null;
		won = null;

	}
	public void setPosition(float xV, float yV) {
		x = xV;
		y = yV;
		this.transform.position = new Vector2 (x, y);
	}
	public float[] getPosition() {
		float[] arr = new float[2];
		arr[0] = x;
		arr[1] = y;
		return arr;
	}
	public void setGridPosition (int xV, int yV) {
		xVar = xV;
		yVar = yV;

	}
	public void setName(string n) {
		this.transform.gameObject.tag = n;
	}
	public GameObject getObject() {
		return this.gameObject;
	}
	public void setColor(Color col) {
		GetComponent<SpriteRenderer>().color = col;
	}
	public Color getColor() {
		return GetComponent < SpriteRenderer > ().color;
	}
	public int[] getGridPosition() {
		int[] arr = new int[2];
		arr [0] = xVar;
		arr [1] = yVar;
		return arr;
	}
	public void loadSelWonSprites(Sprite sel, Sprite wo) {
		won = wo;
		selected = sel;
	}
	public Sprite getDefaultSprite() {
		return defaultSprite;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
