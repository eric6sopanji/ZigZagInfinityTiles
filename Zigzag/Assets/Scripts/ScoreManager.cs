using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {


	int score;
	public Text lastScoreT;
	public Text highScoreT;
	public Text gamePlayedT;

	int highScore;

	void Start () {

		lastScoreT.text = "LAST SCORE : " +PlayerPrefs.GetInt ("LastScore", 0).ToString();
		highScoreT.text = "BEST SCORE : " + PlayerPrefs.GetInt ("HighScore", 0).ToString();
		gamePlayedT.text = "GAME PLAYED : " + (PlayerPrefs.GetInt ("GamePlayed", 0)).ToString();


		if (PlayerPrefs.GetInt ("LastScore", 0) == PlayerPrefs.GetInt ("HighScore", 0) && PlayerPrefs.GetInt ("GamePlayed", 0) > 0) {
			lastScoreT.color = new Color32(174,54,54,255);
			highScoreT .color = new Color32(174,54,54,255);
		}

		highScore = PlayerPrefs.GetInt ("HighScore", 0);
		int gameStartCount = PlayerPrefs.GetInt ("GamePlayed", 0);
		gameStartCount++;

		PlayerPrefs.SetInt ("GamePlayed", gameStartCount);
}


	
	void Update () {

	
		if (score != GetComponent<PlayerScript> ().score) {

			score = GetComponent<PlayerScript> ().score;
			PlayerPrefs.SetInt("LastScore", score);

			if(score > highScore){
				highScore = score;

				PlayerPrefs.SetInt("HighScore",highScore);
			
			}
		}
	}
}
