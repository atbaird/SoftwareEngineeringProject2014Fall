using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour {

	public float xV, yV;
	// Use this for initialization
	void Start () {
		setPosition (0, 0);
	}
	public void setPosition(float xVar, float yVar) {
		xV = xVar;
		yV = yVar;
		gameObject.GetComponent<Transform>().localPosition = new Vector3 (xV, yV, 0.0f);
		Debug.Log (this.transform.position.x + " " + this.transform.position.y);
	}
	public GameObject getGameObject() {
		return this.gameObject;
	}
	public float[] getPosition() {
		float[] arr = new float[2];
		arr[0] = xV;
		arr[1] = yV;
		return arr;
	}
	public float[] getRealPosition() {
		float[] arr = new float[2];
		arr [0] = gameObject.GetComponent<Transform>().position.x;
		arr [1] = gameObject.GetComponent<Transform>().position.y;
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
