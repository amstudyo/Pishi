using UnityEngine;
using System.Collections;

public class CreditsManager : MonoBehaviour {

    RectTransform m_Text;
    void Awake()
    {
        m_Text = transform.FindChild("Text").GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        m_Text.anchoredPosition = new Vector2(0, -1024);
    }

	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
            gameObject.SetActive(false);

        m_Text.anchoredPosition = new Vector2(m_Text.anchoredPosition.x, m_Text.anchoredPosition.y + 50 * Time.deltaTime);
        if (m_Text.anchoredPosition.y > 660)
            gameObject.SetActive(false);
    }
}
