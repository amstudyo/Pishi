using UnityEngine;
using System.Collections;

public class CloudsAnimation : MonoBehaviour {

    RectTransform[] Childs;

    void Awake()
    {
        Childs = GetComponentsInChildren<RectTransform>();
        for (int i = 0; i < Childs.Length; i++)
        {
            if (Random.Range(0, 2) == 0)
                Childs[i].name = "left";
            else
                Childs[i].name = "right";
        }
        Childs[0].name = "Clouds";
    }

	void Update ()
    {
	    foreach (RectTransform child in Childs)
        {
            if (child.name == "Clouds")
                continue;
            if (child.name == "right")
                child.anchoredPosition = new Vector2(child.anchoredPosition.x + 30 * Time.deltaTime, child.anchoredPosition.y);
            else
                child.anchoredPosition = new Vector2(child.anchoredPosition.x - 30 * Time.deltaTime, child.anchoredPosition.y);
            if (child.anchoredPosition.x < -800)
            {
                if (Random.Range(0, 2) == 0)
                {
                    child.anchoredPosition = new Vector2(child.anchoredPosition.x, Random.Range(150, 480));
                    child.name = "right";
                }
                else
                {
                    child.anchoredPosition = new Vector2(800, Random.Range(150, 480));
                }
            }
            if (child.anchoredPosition.x > 800)
            {
                if (Random.Range(0, 2) == 0)
                {
                    child.anchoredPosition = new Vector2(child.anchoredPosition.x, Random.Range(150, 480));
                    child.name = "left";
                }
                else
                {
                    child.anchoredPosition = new Vector2(-800, Random.Range(150, 480));
                }
            }
        }
	}
}
