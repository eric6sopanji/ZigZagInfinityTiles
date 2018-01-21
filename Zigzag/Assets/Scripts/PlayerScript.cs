using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {


	public GameObject xTilePrefab;
	public GameObject yTilePrefab;

	public Transform contactPoint;
	public LayerMask whatIsGround;
	public GameObject ps;
	public GameObject cam;
	public float speed  ;
	
	public bool onXTile ;
	public bool onYTile ;
	public bool generateTile;

	public Text scoreText;
	public GameObject startObj;
	public Animator anim;
	public Animator scoreAnim;

	
	public List<GameObject> currentTileX;
	public List<GameObject> currentTileY;
	public List<GameObject> recycledTileX ;
	public 	List<GameObject> recycledTileY ;
	
    public AudioClip [] audios;


	Vector3 dir ;
	bool camMoved;
	public int score;
	bool restart;
	bool gmStarted;
	float tileGenerateSpeed;
	List<GameObject> closeAnimX;
	List <GameObject> closeAnimY;



	void Awake(){
		CreateTiles (30);

	}

	void Start () {

		gmStarted = false;
		CreateAtStart (10);
		generateTile = true;
		scoreText.text = "SCORE : " + score;
		restart = true;
	
		closeAnimX = new List<GameObject>();
		closeAnimY = new List<GameObject>();



	}


	void FixedUpdate(){
		
		if (currentTileY.Count != 0 && currentTileX.Count != 0 && IsGrounded()&& !camMoved) {
			CamFollow ();
		}

		transform.Translate (dir * speed * Time .deltaTime);
	}

	void Update () {

			OnClickMouse ();
			IsGrounded ();

		if (!IsGrounded () && restart || score < 0 && restart ) {
			if(score < 0)
				scoreAnim.SetTrigger("Score0");
			
			StartCoroutine(PlayerDead());
			restart = false;
		}
	
	}
	IEnumerator PlayerDead(){


		GetComponent<AudioSource> ().volume = 1;
		GetComponent<AudioSource> ().pitch = 1;

		GetComponent<AudioSource>().clip = audios[5];
		GetComponent<AudioSource>().Play();


		yield return new WaitForSeconds (2);


		if (PlayerPrefs.GetInt ("GamePlayed", 0) >PlayerPrefs.GetInt("AD",30)) {

		}

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

	void OnTriggerEnter(Collider other){


		if (other.CompareTag ("Block")) {

			dir = -dir;
			int randomTileNumber = Random.Range (5, 9);
			ActivateTiles (randomTileNumber);
			score--;

			if(score >= 0){
			scoreText.text  = "" + score;
				GetComponent<AudioSource> ().volume = 1;
				GetComponent<AudioSource> ().pitch = 1;

				GetComponent<AudioSource>().clip = audios[1];
				GetComponent<AudioSource>().Play();
			}

		} else if (other.CompareTag ("XTile")) {
			onYTile = false;
			onXTile = true;
		} else if (other.CompareTag ("YTile")) {
			onXTile = false;
			onYTile = true;
		} else if (other.CompareTag ("Coin")) {

			score += 2;
			Instantiate(ps,other.transform.position,other.transform.rotation);
			other.gameObject.SetActive (false);
			scoreText.text = "" + score;
			GetComponent<AudioSource> ().pitch = 1;

			GetComponent<AudioSource> ().volume = .5f;
			GetComponent<AudioSource>().clip = audios[3];
			GetComponent<AudioSource>().Play();
		}
	}


	
	void OnClickMouse(){


		if (Input.GetMouseButtonDown (0) && IsGrounded() && score >= 0) {
			

			if (!gmStarted) {
				

				GetComponent<Rigidbody> ().isKinematic = false;
				anim.SetTrigger ("GameStarted");

				if (!anim.GetCurrentAnimatorStateInfo (0).IsName ("GameStarted"))
				
				scoreText.gameObject.SetActive (true);
				gmStarted = true;
				GetComponent<AudioSource> ().pitch = 1;
				GetComponent<AudioSource> ().volume = 1;
				GetComponent<AudioSource> ().clip = audios [0];
				GetComponent<AudioSource> ().Play ();
		
			} else {
				GetComponent<AudioSource> ().pitch = 1.35f;
				GetComponent<AudioSource> ().volume = 1;
				GetComponent<AudioSource>().clip = audios[1];
				GetComponent<AudioSource>().Play();
			}
		
	
				score += 5;
				
			scoreText.text = "" + score;
			generateTile = true;

			if (dir == Vector3.right || dir == -Vector3.right) {
				dir = Vector3.forward;
			} else if (dir == Vector3.forward || -dir == Vector3.forward) {
				dir = -Vector3.right;
			}else{
				dir = -Vector3.right;
			}
		}
	}


	void CamFollow(){
		GameObject tmp = null;

		if (onYTile && currentTileX.Count != 0) {

			tmp = currentTileX[currentTileX.Count -1];

		}else if (onXTile && currentTileY.Count != 0) {

			tmp = currentTileY[currentTileY.Count - 1];

		}
		Vector3 point = cam.GetComponent<Camera>().WorldToViewportPoint(tmp.transform.position);
		Vector3 delta = tmp.transform.position - cam.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); 
		Vector3 destination = cam.transform.position + delta;
		Vector3 velocity = Vector3.zero;
		cam.transform.position =  Vector3.SmoothDamp(cam.transform.position, destination,ref velocity, .35f);

	}
		
	private bool IsGrounded(){


		Collider [] colliders = Physics.OverlapSphere (contactPoint.position, .3f, whatIsGround);
		for(int i = 0; i<colliders.Length; i++){
			if(colliders[i].gameObject != gameObject){

				return true;
			}
		}

		return false;

	}

		
	public void CreateTiles(int amount){

		for (int i = 0; i < amount; i++) {
			recycledTileX.Add (Instantiate (xTilePrefab));
			recycledTileX [recycledTileX.Count - 1].name = ("xTile");
			recycledTileX [recycledTileX.Count - 1].SetActive (false);
			recycledTileY.Add (Instantiate (yTilePrefab));
			recycledTileY [recycledTileY.Count - 1].name = ("yTile");		
			recycledTileY [recycledTileY.Count - 1].SetActive (false);
		}
	}

	void ManageSpeed(){
		speed = Random .Range (16, 44)/4;
	}

	void CreateAtStart(int amount){

		for (int i = 0; i < amount ; i++){
			GameObject tmp = recycledTileX[recycledTileX.Count-1];
			recycledTileX.Remove(tmp );


			tmp.transform.position = currentTileX[currentTileX.Count - 1].transform.GetChild(0).transform.GetChild (1).transform.position;
			currentTileX .Add( tmp);

			int spawnPickUp = Random.Range (0, 10);

			if (spawnPickUp == 0) {
				currentTileX[currentTileX.Count - 1]. transform.GetChild(2).gameObject.SetActive(true);
			}

			if(i == amount -1){
				tmp.transform.GetChild (1).gameObject.SetActive(true);
			}
		}
		tileGenerateSpeed = .3f;
		StartCoroutine(TileWakeUp());
	}

	public void ActivateTiles(int amount){

		if (recycledTileX.Count < 15 || recycledTileY.Count < 15 ) {
			CreateTiles(10);
			Debug.Log ("generating extra 10 tiles");
		}

		if(onYTile && generateTile){
			ManageSpeed();
			if(currentTileX.Count != 0){
				if(currentTileX.Count != 0){
				for(int i = 0; i < currentTileX.Count; i++){
					if(currentTileX[currentTileX.Count - 1-i].transform.GetChild(1).gameObject.activeSelf){
						currentTileX[currentTileX.Count - 1-i].transform.GetChild(1).gameObject.SetActive(false);
					}
						closeAnimX.Add (currentTileX [currentTileX.Count - i - 1]);
				}

			}
				StartCoroutine (DeactivateXTile ());		
                currentTileX.Clear();
			}

			int randomBlockNumber = Random.Range(0, currentTileY.Count -3);
			int randomTileNumber = Random.Range(randomBlockNumber + 1, currentTileY.Count -1);

		
			currentTileY[randomBlockNumber].transform.GetChild (1).gameObject.SetActive (true);
			GameObject tmp = recycledTileX[recycledTileX.Count - 1];
			recycledTileX.Remove (tmp);

			tmp.transform.position = currentTileY[randomTileNumber].transform.GetChild (0).transform.GetChild (1).transform.position;

			currentTileX.Add(tmp);

			for(int i = 0; i < amount; i++){
				tmp = recycledTileX[recycledTileX.Count - 1];
				recycledTileX.Remove (tmp);

				tmp.transform.position = currentTileX[currentTileX.Count - 1].transform.GetChild (0).transform.GetChild (1).transform.position;
		
				currentTileX.Add(tmp);
				
				if(i == amount-1){
					tmp.transform.GetChild (1).gameObject.SetActive (true);
				}

				int spawnPickUp = Random.Range (0, 10);

				if (spawnPickUp == 0) {
					currentTileX[currentTileX.Count - 1]. transform.GetChild(2).gameObject.SetActive(true);
				}
			}

			StartCoroutine (WakeUpWOnY());
			generateTile = false;
			camMoved = false;
		}


		if(onXTile && generateTile){

			if(currentTileY.Count != 0){
				ManageSpeed();

				for(int i = 0; i < currentTileY.Count; i++){

					if(currentTileY.Count != 0){
					if(currentTileY[currentTileY.Count - 1-i].transform.GetChild(1).gameObject.activeSelf){

							currentTileY[currentTileY.Count - 1-i].transform.GetChild(1).gameObject.SetActive(false);
				}

				
						closeAnimY.Add (currentTileY [currentTileY.Count - i - 1]);
					}
				}
				StartCoroutine (DeactivateYTile ());
				currentTileY.Clear();
			}
			int randomBlockNumber = Random.Range(0, currentTileX.Count -3);
			int randomTileNumber = Random.Range(randomBlockNumber +1 , currentTileX.Count -1);

			currentTileX[randomBlockNumber].transform.GetChild (1).gameObject.SetActive (true);
			GameObject tmp = recycledTileY[recycledTileY.Count - 1];
			recycledTileY.Remove (tmp);

			tmp.transform.position = currentTileX[randomTileNumber].transform.GetChild (0).transform.GetChild (0).transform.position;

			currentTileY.Add(tmp);

			for(int i = 0; i < amount; i++){
				tmp = recycledTileY[recycledTileY.Count - 1];
				recycledTileY.Remove (tmp);
			
				tmp.transform.position = currentTileY[currentTileY.Count - 1].transform.GetChild (0).transform.GetChild (0).transform.position;
			
				currentTileY.Add(tmp);

				if(i == amount-1){
					tmp.transform.GetChild (1).gameObject.SetActive (true);
				}
				int spawnPickUp = Random.Range (0, 10);
				
				if (spawnPickUp == 0) {
					currentTileY[currentTileY.Count - 1]. transform.GetChild(2).gameObject.SetActive(true);
				}
			}
			tileGenerateSpeed = .3f;
			StartCoroutine (WakeUpWOnX());
			generateTile = false;
			camMoved = false;
		}
	}
	
	IEnumerator DeactivateXTile(){
		tileGenerateSpeed = .3f;

		if (closeAnimX.Count != 0) {
			foreach(GameObject tile in closeAnimX) {

				tile .GetComponent<Animator> ().SetTrigger ("CloseTile");

				if (tileGenerateSpeed > .15f) {
					tileGenerateSpeed -= .1f;
					yield return new WaitForSeconds (tileGenerateSpeed);

				} else{
					yield return new WaitForSeconds (tileGenerateSpeed);
				}
				recycledTileX.Add (tile);
			}


			closeAnimX.Clear ();
		}
	}
	IEnumerator DeactivateYTile(){
		tileGenerateSpeed = .3f;
		if (closeAnimY.Count != 0) {

			foreach(GameObject tile in closeAnimY) {

				tile .GetComponent<Animator> ().SetTrigger ("CloseTile");

				if (tileGenerateSpeed > .15f) {
					tileGenerateSpeed -= .1f;
					yield return new WaitForSeconds (tileGenerateSpeed);

				} else{
					yield return new WaitForSeconds (tileGenerateSpeed);
				}
				recycledTileY.Add (tile);
			}

			closeAnimY.Clear ();
		}
	}

	IEnumerator WakeUpWOnX(){

		foreach (GameObject tile in currentTileY) {
			tile.SetActive (true);
				tile.GetComponent<Animator>().SetTrigger ("OpenTile");
	
		
		if (tileGenerateSpeed > .15f) {
			tileGenerateSpeed -= .1f;
			yield return new WaitForSeconds (tileGenerateSpeed);
		} else{
			yield return new WaitForSeconds (tileGenerateSpeed);
		}

		}
	}
	IEnumerator WakeUpWOnY(){

		foreach (GameObject tile in currentTileX) {
			tile.SetActive (true);
			tile.GetComponent<Animator>().SetTrigger ("OpenTile");
		
			if (tileGenerateSpeed > .15f) {
				tileGenerateSpeed -= .1f;
				yield return new WaitForSeconds (tileGenerateSpeed);
			} else{
				yield return new WaitForSeconds (tileGenerateSpeed);
			}
			
		}
	}
	IEnumerator TileWakeUp(){

		foreach (GameObject tile in currentTileX) {
			tile.SetActive (true);
		
			tile.GetComponent<Animator>().SetTrigger ("OpenTile");



			if (tileGenerateSpeed > .15f) {
				tileGenerateSpeed -= .1f;
				yield return new WaitForSeconds (tileGenerateSpeed);
			} else{
				yield return new WaitForSeconds (tileGenerateSpeed);
			}
            
		}
	}

   

 
	
}
