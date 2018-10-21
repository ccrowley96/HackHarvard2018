using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBike : MonoBehaviour {
	
	//Move Keys
	public KeyCode upKey;
	public KeyCode downKey;
	public KeyCode leftKey;
	public KeyCode rightKey;

	// Walls
	public GameObject wallPrefab;
	Collider2D currentWall;
	// Position of last wall placement
	Vector2 lastWallEnd;

	public float speed = 15;
	
	// Instantiate a new light wall
	void SpawnWall(){
		 // Save last wall's position
    	lastWallEnd = transform.position;

		GameObject temp = Instantiate(wallPrefab, transform.position, Quaternion.identity);
		currentWall = temp.GetComponent<Collider2D>();		
	}

	// Fit collider between two points
	void FitColliderBetween(Collider2D col, Vector2 a, Vector2 b){
		// Calculate center
		col.transform.position = a + (b - a) * 0.5f;

		// Scale fit horizontally or vertically
		float dist = Vector2.Distance(a,b);
		if(a.x != b.x) // scale in x
			col.transform.localScale = new Vector2(dist + 1, 1);
		else // scale in y
			col.transform.localScale = new Vector2(1, dist + 1);
	}
	
	// Check for collision
	void OnTriggerEnter2D(Collider2D col){
		// If collider isn't current wall, kill player
		if(col != currentWall){
			Debug.Log("Player Lost: " + name);
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		// Set initial updwards velocity
		GetComponent<Rigidbody2D>().velocity = Vector2.up * speed;
		SpawnWall();
	}
	
	// Update is called once per frame
	void Update () {
		// Check for key presses and modify velocity accordingly
		if(Input.GetKeyDown(upKey)){
			GetComponent<Rigidbody2D>().velocity = Vector2.up * speed;
			SpawnWall();
		} else if(Input.GetKeyDown(downKey)){
			GetComponent<Rigidbody2D>().velocity = Vector2.down * speed;
			SpawnWall();
		} else if(Input.GetKeyDown(leftKey)){
			GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
			SpawnWall();
		} else if(Input.GetKeyDown(rightKey)){
			GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
			SpawnWall();
		}

		FitColliderBetween(currentWall, lastWallEnd, transform.position);
	}
}
