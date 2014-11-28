using UnityEngine;
using System.Collections;

public class CameraCenterer : MonoBehaviour {
	bool positionCentered;
	// Use this for initialization
	void Start () {
		positionCentered = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(positionCentered == false) {
			GameManager gridManage = FindObjectOfType(typeof(GameManager)) as GameManager;
			if(gridManage.readyToRead () == true) {
				float[] sizeOfGrid = gridManage.getMaxGridSize ();
				repositionAndScaleCamera (sizeOfGrid, gridManage.bottomLeftCellPosition(), gridManage.upperRightCellPosition());
				positionCentered = true;
			}
		}
	}
	private void repositionAndScaleCamera(float[] sizeOfGrid, float[] bottomLeft, float[] upperRight) {
		
		//reposition
		float xPos = sizeOfGrid [0] / 2;
		float yPos = sizeOfGrid [1] / 2;
		this.transform.position = new Vector3 (xPos, yPos, -10);
		
		//resize
		//Camera.main.orthograpthicSize = (sizeOfGrid[0] + 2) * Screen.height / Screen.width * 0.5;
	}
}