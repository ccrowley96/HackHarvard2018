using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveBike : MonoBehaviour {
	
	//Move Keys
	public KeyCode upKey;
	public KeyCode downKey;
	public KeyCode leftKey;
	public KeyCode rightKey;
	public GameObject gameOverText;
	public GameObject restartText;
	private bool waitForRestart = false;
	private bool gameOver = false;

	enum direction {up, down, left, right};
	direction heading = direction.up;

	float thrust = 8f;
	float turnPenalty = .8f;

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

	public void GameOver(){
		gameOver = true;
		GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);

	}
	
	// Check for collision
	void OnTriggerEnter2D(Collider2D col){
		// If collider isn't current wall, kill player
		if(col != currentWall){
			GameObject.Find("player_pink").gameObject.GetComponent<MoveBike>().GameOver();
			GameObject.Find("player_cyan").gameObject.GetComponent<MoveBike>().GameOver();
			Destroy(GameObject.Find("Wall"));
			gameOverText.SetActive(true);
			restartText.SetActive(true);
			waitForRestart = true;
		}
	}


	// Use this for initialization
	void Start () {
		// Set initial updwards velocity
		GetComponent<Rigidbody2D>().velocity = Vector2.up * speed;
		SpawnWall();
		gameOverText.SetActive(false);
		restartText.SetActive(false);
		waitForRestart = false;
		gameOver = false;
	}

	void FixedUpdate(){
		if(!gameOver){
			switch (heading)
			{
				case direction.up:
					GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * thrust);
					break;
				case direction.down:
					GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.down * thrust);
					break;
				case direction.right:
					GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.right * thrust);
					break;
				case direction.left:
					GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.left * thrust);
					break;
				default:
					break;
			}
			speed = GetComponent<Rigidbody2D>().velocity.magnitude;
			FitColliderBetween(currentWall,lastWallEnd, transform.position);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!gameOver){

			// Check for key presses and modify velocity accordingly
			if(Input.GetKeyDown(upKey) && heading != direction.down && heading != direction.up){
				GetComponent<Rigidbody2D>().velocity = Vector2.up * speed * turnPenalty;
				SpawnWall();
				heading = direction.up;
			} else if(Input.GetKeyDown(downKey) && heading != direction.up && heading != direction.down){
				GetComponent<Rigidbody2D>().velocity = Vector2.down * speed * turnPenalty;
				SpawnWall();
				heading = direction.down;
			} else if(Input.GetKeyDown(leftKey) && heading != direction.right && heading != direction.left){
				GetComponent<Rigidbody2D>().velocity = Vector2.left * speed * turnPenalty;
				SpawnWall();
				heading = direction.left;
			} else if(Input.GetKeyDown(rightKey) && heading != direction.left && heading != direction.right){
				GetComponent<Rigidbody2D>().velocity = Vector2.right * speed * turnPenalty;
				SpawnWall();
				heading = direction.right;
			}
			FitColliderBetween(currentWall,lastWallEnd, transform.position);
		}
		if(Input.GetKeyDown(KeyCode.Space) && waitForRestart){
			waitForRestart = false;
			SceneManager.LoadScene(0);
		}
		
	}
}
