using UnityEngine;
using System.Collections;

public class Park_TreeRandomize : MonoBehaviour {

	void Awake()
    {
        int mode = Random.Range(1, 3);
        transform.FindChild(mode.ToString()).gameObject.SetActive(true);
    }
}
