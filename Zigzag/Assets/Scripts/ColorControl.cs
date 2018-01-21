using UnityEngine;
using System.Collections;

public class ColorControl : MonoBehaviour {

	public Material tileMaterial;
	public Material blockM;
	public Material ballM;

	public Color[] colorList;
	public PlayerScript playerS;
	int currentScore;
	int targetScore;
	int ranNumber;
	int selectedNum;
	Color ranColor; 



	bool colorChanged; 
	void Start () {
		
		currentScore = 0;
		targetScore = 16;
		colorChanged = false;



		ranNumber = Random.Range (0, colorList.Length);

		ranColor = colorList [ranNumber];
		
		if (ranNumber < 6) {
			selectedNum = ranNumber + 6;
		} else {
			selectedNum = ranNumber - 6;
		}
		StartCoroutine (UpdateScore ());
	}
	

	void Update () {

		if (currentScore >= targetScore) {
			if(ranNumber == 6){
					blockM.color = Color.Lerp (blockM.color, colorList[11], Time.time * 0.0003f);	
					ballM.color = Color.Lerp (ballM.color, colorList[1], Time.time * 0.0003f);	
				}
				else if(ranNumber == 5){
					blockM.color = Color.Lerp (blockM.color, colorList[10], Time.time * 0.0003f);	
					ballM.color = Color.Lerp (ballM.color, colorList[0], Time.time * 0.0003f);	
			}else {
				blockM.color = Color.Lerp (blockM.color, colorList[selectedNum -1], Time.time * 0.0003f);	
				ballM.color = Color.Lerp (ballM.color, colorList[selectedNum +1], Time.time * 0.0003f);	
			}
			tileMaterial.color = Color.Lerp (tileMaterial.color, ranColor, Time.time * 0.0003f);

			if (!colorChanged) {
				StartCoroutine (ControlColor ());
				colorChanged = true;

				GetComponent<AudioSource> ().Play();

			}
		}
	
				
		}

	IEnumerator UpdateScore(){
		yield return new WaitForSeconds (4);
		currentScore = playerS.score;
		StartCoroutine (UpdateScore ());
	}

	IEnumerator ControlColor(){
		yield return new WaitForSeconds (3);

		targetScore += 19;

		ranNumber = Random.Range (0, colorList.Length);

		ranColor = colorList [ranNumber];
	

		if(ranNumber < 6)
			selectedNum = ranNumber +6;
		else
			selectedNum = ranNumber -6;
		colorChanged = false;
	}
}
