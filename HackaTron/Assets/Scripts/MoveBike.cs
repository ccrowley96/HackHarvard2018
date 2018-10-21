using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBike : MonoBehaviourPun {
	
	[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
	public static GameObject LocalPlayerInstance;
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
		if (!photonView.IsMine)
		{
			return;
		}
		 // Save last wall's position
    	lastWallEnd = transform.position;

		GameObject temp = PhotonView.Instantiate(wallPrefab, transform.position, Quaternion.identity);
		currentWall = temp.GetComponent<Collider2D>();		
	}

	// Fit collider between two points
	void FitColliderBetween(Collider2D col, Vector2 a, Vector2 b){
		if (!photonView.IsMine)
		{
			return;
		}
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
		if (!photonView.IsMine)
		{
			return;
		}
		// If collider isn't current wall, kill player
		if(col != currentWall){
			Debug.Log("Player Lost: " + name);
			GameManager.Instance.LeaveRoom();
			Destroy(gameObject);
		}
	}

	void Awake(){
		// #Important
		// used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
		if (photonView.IsMine)
		{
			PlayerManager.LocalPlayerInstance = this.gameObject;
		}
		// #Critical
		// we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () {
		
		CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();
		// Camera to follow only the local player
		 if (_cameraWork != null)
		{
			if (photonView.IsMine)
			{
				_cameraWork.OnStartFollowing();
			}
		}
		else
		{
			Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
		}
		if (!photonView.IsMine)
		{
			return;
		}
		// Set initial updwards velocity
		GetComponent<Rigidbody2D>().velocity = Vector2.up * speed;
		SpawnWall();
	}
	
	// Update is called once per frame
	void Update () {
		// Return if not in the current player network instance
		if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
		{
			return;
		}
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
