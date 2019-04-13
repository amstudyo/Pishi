using UnityEngine;
using System.Collections;

public class CContext : MonoBehaviour {

	public static CContext Context = null;

	// Config
	public int GameMusic = 0;
	public int FinishedCount = 0;
	public int LoseCount = 0;

	void Awake()
	{
		if (Context == null)
			Context = this;
		else { // we already have a context
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad (this);
		// Load Config
		GameMusic = PlayerPrefs.GetInt ("GameMusic", 0);
		FinishedCount = PlayerPrefs.GetInt ("FinishedCount", 0);
		LoseCount = PlayerPrefs.GetInt ("LoseCount", 0);
	}

	void OnApplicationQuit()
	{
		PlayerPrefs.SetInt ("GameMusic", GameMusic);
		PlayerPrefs.SetInt ("FinishedCount", FinishedCount);
		PlayerPrefs.SetInt ("LoseCount", LoseCount);
	}

}
