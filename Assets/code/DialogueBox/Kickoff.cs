using UnityEngine;

public class Kickoff : MonoBehaviour
{

    public GameObject Activatee;

	// Use this for initialization
	void Start () {
        Activatee.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {
	
        if(!Activatee.activeSelf && Input.GetMouseButtonDown(0))
            Activatee.SetActive(true);
	}
}
