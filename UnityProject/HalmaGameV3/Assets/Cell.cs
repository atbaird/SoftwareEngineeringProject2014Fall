using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour {

	float x, y;
	// Use this for initialization
	void Start () {
		x = 0;
		y = 0;
		this.transform.position = new Vector2 (x, y);
	}
	public void setPosition(float xVar, float yVar) {
		x = xVar;
		y = yVar;
		this.transform.position = new Vector2 (x, y);
	}
	public float[] getPosition() {
		float[] arr = new float[2];
		arr[0] = x;
		arr[1] = y;
		return arr;
	}
	public float[] getSize() {
		float[] arr = new float[2];
		var renderer = this.GetComponent<Renderer> ();
		arr[0] = renderer.bounds.size.x;
		arr[1] = renderer.bounds.size.y;
		return arr;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
