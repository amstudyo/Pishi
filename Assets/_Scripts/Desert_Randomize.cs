using UnityEngine;
using System.Collections;

public class Desert_Randomize : MonoBehaviour {

    IEnumerator Test()
    {
        yield return new WaitForSeconds(0.5f);
        int first = Random.Range(0, 4);
        int second = Random.Range(0, 4);
        while (second == first)
            second = Random.Range(0, 4);
        int third = Random.Range(0, 4);
        while (third == first || third == second)
            third = Random.Range(0, 4);
        int forth = Random.Range(0, 4);
        while (forth == first || forth == second || forth == third)
            forth = Random.Range(0, 4);
        int fifth = 4;

        float startZ = 70f + Random.Range(0f, 5f);
        float min = 200;
        float max = 210;
        if (gameObject.name == "Desert2")
        {
            min = 220;
            max = 280;
        }
        else if (gameObject.name == "Moze")
        {
            startZ = -190;
            min = 170;
            max = 210;
        }
        transform.GetChild(first).localPosition = new Vector3(transform.GetChild(first).localPosition.x, transform.GetChild(first).localPosition.y, startZ);
        startZ -= Random.Range(min, max);
        transform.GetChild(second).localPosition = new Vector3(transform.GetChild(second).localPosition.x, transform.GetChild(second).localPosition.y, startZ);
        startZ -= Random.Range(min, max);
        transform.GetChild(third).localPosition = new Vector3(transform.GetChild(third).localPosition.x, transform.GetChild(third).localPosition.y, startZ);
        startZ -= Random.Range(min, max);
        transform.GetChild(forth).localPosition = new Vector3(transform.GetChild(forth).localPosition.x, transform.GetChild(forth).localPosition.y, startZ);
        if (gameObject.name == "Moze")
        {
            startZ -= Random.Range(min, max);
            transform.GetChild(fifth).localPosition = new Vector3(transform.GetChild(fifth).localPosition.x, transform.GetChild(fifth).localPosition.y, startZ);
        }
    }

	void Start()
    {
        StartCoroutine(Test());
        
    }
}
