using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public GameObject m_Credits;
	public RectTransform SoundPickerFA;
    public RectTransform SoundPickerEN;
    public GameObject HighScoresFA;
    public GameObject HighScoresEN;
    public GameObject m_Loading;
	float soundPickerFATargetX = -100;
    float soundPickerENTargetX = 300;
    AsyncOperation m_LoadAsync = null;
    bool m_FirstRun = false;
    public GameObject m_FA;
    public GameObject m_ENG;
	

	void Start()
	{
		if (PlayerPrefs.GetInt ("FirstRun", 0) == 0) 
		{
            m_FirstRun = true;
			Handheld.PlayFullScreenMovie ("tutorial.mp4", Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.Fill);
			PlayerPrefs.SetInt ("FirstRun", 1);
		}

        int lang = PlayerPrefs.GetInt("Language", 0);
        SelectLanguage(lang);
    }

    public void SelectLanguage(int i)
    {
        if (i == 1)
        {
            m_FA.SetActive(false);
            m_ENG.SetActive(true);
        }
        else if (i == 2)
        {
            m_FA.SetActive(true);
            m_ENG.SetActive(false);
        }
        PlayerPrefs.SetInt("Language", i);
    }

    float animFillAmount = 0f;

	void Update()
	{
		SoundPickerFA.anchoredPosition = new Vector2(Mathf.MoveTowards (SoundPickerFA.anchoredPosition.x, soundPickerFATargetX, 500 * Time.deltaTime), 0);
        SoundPickerEN.anchoredPosition = new Vector2(Mathf.MoveTowards(SoundPickerEN.anchoredPosition.x, soundPickerENTargetX, 500 * Time.deltaTime), 0);
        if (HighScoresFA.activeSelf) {
			HighScoresFA.transform.FindChild("Finished").GetComponentInChildren<Text>().text = CContext.Context.FinishedCount.ToString();
			HighScoresFA.transform.FindChild("Lost").GetComponentInChildren<Text>().text = CContext.Context.LoseCount.ToString();
		}
        if (HighScoresEN.activeSelf)
        {
            HighScoresEN.transform.FindChild("Finished").GetComponentInChildren<Text>().text = CContext.Context.FinishedCount.ToString();
            HighScoresEN.transform.FindChild("Lost").GetComponentInChildren<Text>().text = CContext.Context.LoseCount.ToString();
        }
        

		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit ();
	}


	public void OnHelpClick()
	{
        Handheld.PlayFullScreenMovie("tutorial.mp4", Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.Fill);
    }
	public void OnSoundsClick()
	{
		soundPickerENTargetX = 0;
        soundPickerFATargetX = 280;
	}
	public void OnHighScoresClick()
	{
        if (m_FA.activeSelf)
		    HighScoresFA.SetActive(true);
        else
            HighScoresEN.SetActive(true);
    }
	public void OnHighScore_Back()
	{
        if (m_FA.activeSelf)
            HighScoresFA.SetActive(false);
        else
            HighScoresEN.SetActive(false);
    }
	public void OnTrackClick(int i)
	{
		CContext.Context.GameMusic = i;
		soundPickerFATargetX = -100;
        soundPickerENTargetX = 300;
    }

    public void OnCreditsClick()
    {
        m_Credits.SetActive(true);
    }

	public void OnExitClick()
	{
		Application.Quit ();
	}

    IEnumerator LoadingScreen()
    {
        float t = 0f;
        while (true)
        {
            if (m_Loading.activeSelf)
            {
                m_Loading.transform.FindChild("Image").GetComponent<Image>().fillAmount = animFillAmount;
                animFillAmount = Mathf.PingPong(t * 2f, 1);
                t += Time.deltaTime;
            }
            yield return null;
        }
    }
	public void OnPlayClick()
	{
        m_Loading.SetActive(true);
        StartCoroutine(LoadingScreen());
		m_LoadAsync = Application.LoadLevelAsync ("Game_01");
        m_LoadAsync.priority = 100;
        
    }
}
