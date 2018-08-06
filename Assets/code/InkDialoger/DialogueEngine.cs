using System;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class DialogueEngine : MonoBehaviour
{
    public TextAsset stuff;

    private float timeToFinish;
    private float secondsPerCharacter;
    public float charactersPerSecond = 5;
    public List<speaker> speakers;
    public KeyCode continueKey = KeyCode.Space;
    private Dictionary<string, speaker> lookupPeeps;
    private Story storyPlayer;
    public TextMeshProUGUI textTarget;
    public Transform ButtonPrefab;
    public Transform buttonLocation;
    public float buttonDisplacement;
    public Camera profileCamera;

    private string displayText;
    private string wholeText;
    private float progress;
    private bool readyToAdvance;
    private bool choicesDrawn;
    private List<GameObject> currentChoices;

    private bool advance;

    // Use this for initialization
	void Start () {
        lookupPeeps = new Dictionary<string, speaker>();
	    foreach (var speaker in speakers)
	    {
	        lookupPeeps[speaker.speakerName] = speaker;
	    }

        storyPlayer = new Story(stuff.text);
	    progress = 0f;
	    readyToAdvance = false;
	    advance = true;
	    textTarget.text = "";
        currentChoices = new List<GameObject>();
	}

    // Update is called once per frame
    void Update ()
    {

        secondsPerCharacter = 1 / charactersPerSecond;
	    if (stuff != null)
	    {
	        if (advance)
	        {
	            if (storyPlayer.canContinue)
	            {
	                wholeText = storyPlayer.Continue();
	                SetPortrait();
	            }

	            advance = false;
	            progress = 0;
	            choicesDrawn = false;
	        }


	        if (readyToAdvance && storyPlayer.currentChoices?.Count > 0)
	        {
	            if (!choicesDrawn)
	            {
	                int i = 0;
	                var basePos = buttonLocation.transform.position;

	                clearChoices();
	                foreach (var currentChoice in storyPlayer.currentChoices)
	                {
	                    var thing = Instantiate(ButtonPrefab, this.gameObject.transform);
	                    var closureCount = i;
	                    thing.gameObject.GetComponentInChildren<Text>().text = currentChoice.text;
	                    thing.gameObject.GetComponent<Button>().onClick.AddListener(delegate {chosenChoiceAction(closureCount);});
	                    thing.transform.position = new Vector3(basePos.x, basePos.y + i * buttonDisplacement, basePos.z);
	                    currentChoices.Add(thing.gameObject);
	                    i++;
	                }

	                choicesDrawn = true;
	            }

	        }
            else if (readyToAdvance && Input.GetKeyUp(continueKey))
	        {
	            advance = true;
	        }

	        if (storyPlayer.currentChoices?.Count == 0 && currentChoices.Count != 0)
	        {
                clearChoices();
	        }

            var charCount = wholeText.Length;
	        timeToFinish = secondsPerCharacter * charCount;
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

    private void SetPortrait()
    {
        foreach (var speaker in speakers)
        {
            var speakerTag = speaker.speakerName + ":";
            if (wholeText.Contains(speakerTag))
            {
                wholeText = wholeText.Replace(speakerTag, "");
                if (profileCamera != null)
                {
                    profileCamera.transform.position = lookupPeeps[speaker.speakerName].camAngle.position;
                    profileCamera.transform.rotation = lookupPeeps[speaker.speakerName].camAngle.rotation;
                }
            }
        }
    }

    private void clearChoices()
    {
        foreach (var currentChoice in currentChoices)
        {
            //TODO: let these things destroy themselves
            Destroy(currentChoice);
        }

        currentChoices = new List<GameObject>();
    }

    void chosenChoiceAction(int choiceNum)
    {
        Debug.Log($"chose choice {choiceNum}");
        storyPlayer.ChooseChoiceIndex(choiceNum);
        advance = true;
        choicesDrawn = false;
    }
}

[Serializable]
public class speaker
{
    public string speakerName;
    public Transform prefabAttached;
    public bool hasAnimator;
    public Transform camAngle;
}
