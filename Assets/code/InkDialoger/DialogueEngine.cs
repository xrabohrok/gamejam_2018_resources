using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class DialogueEngine : MonoBehaviour
{
    public TextAsset stuff;

    public float timeToFinish = 2.0f;
    public List<speaker> speakers;
    public KeyCode continueKey = KeyCode.Space;
    private Dictionary<string, speaker> lookupPeeps;
    private Story storyPlayer;
    public Text textTarget;

    private string displayText;
    private string wholeText;
    private float progress;
    private bool readyToAdvance;

    private bool advance;

    // Use this for initialization
	void Start () {
        lookupPeeps = new Dictionary<string, speaker>();
	    storyPlayer = new Story(stuff.text);
	    progress = 0f;
	    readyToAdvance = false;
	    advance = true;
	    textTarget.text = "";
	}

    // Update is called once per frame
    void Update () {
	    if (stuff != null)
	    {
	        if (advance)
	        {
	            if (storyPlayer.canContinue)
	            {
	                wholeText = storyPlayer.Continue();
	            }

	            advance = false;
	            progress = 0;
            }


	        if (readyToAdvance && Input.GetKeyUp(continueKey))
	        {
	            advance = true;
	        }

	        var charCount = wholeText.Length;
            
	        var length = Mathf.CeilToInt( Mathf.Lerp(0, charCount - 1,  Mathf.Clamp(progress / timeToFinish, 0, 1)));
//            Debug.Log($"charCount : {charCount}, substring : {length}, ticks: {progress}, readyToAdvance: {readyToAdvance}");
	        displayText = wholeText.Substring(0, length);
	        textTarget.text = displayText;

	        if (progress > timeToFinish)
	        {
	            readyToAdvance = true;
	        }
	        else
	        {
	            progress += Time.deltaTime;
            }
        }
    }
}

[Serializable]
public class speaker
{
    public string speakerName;
    public Transform prefabAttached;
    public bool hasAnimator;
}
