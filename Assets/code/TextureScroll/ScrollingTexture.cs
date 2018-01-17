using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ScrollingTexture : MonoBehaviour {

    public Vector2 speed;

	// Use this for initialization
    MeshRenderer _rendererthing;
	void Start () {
        _rendererthing = this.GetComponent<MeshRenderer>();

	}
	
	// Update is called once per frame
	void Update () {
        _rendererthing.material.SetTextureOffset("_MainTex", GetComponent<Renderer>().material.GetTextureOffset("_MainTex") + Time.deltaTime * speed);
	}
}
