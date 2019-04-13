using UnityEngine;
using System.Collections;

public class Startup_Video : MonoBehaviour {

	public GameObject m_Canvas;

	IEnumerator  Start () 
	{
		StartCoroutine (TestIE ());
		Handheld.PlayFullScreenMovie ("startup_video.mp4", Color.white, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.Fill);
		yield return new WaitForSeconds(2.5f);
		Application.LoadLevel ("MainMenu");
	}

	IEnumerator  TestIE () 
	{
		yield return new WaitForSeconds (0.5f);
		m_Canvas.SetActive (true);
	}
}
