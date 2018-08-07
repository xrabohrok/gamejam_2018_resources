using System;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    public Animator anim;

    private string displayText;
    private string wholeText;
    private float progress;
    private bool readyToAdvance;
    private bool choicesDrawn;
    private List<GameObject> currentChoices;
    private TMP_FontAsset defaultFont;
    private TextMeshProUGUI tmpText;

    private bool advance;

    private string portraitName;
    private int charsPerTrigger = 10;
    private int lastTriggeredCount = 0;

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

	    if (anim == null)
	    {
	        anim = this.GetComponent<Animator>();
	    }
	    portraitName = "";

	    tmpText = GetComponentInChildren<TextMeshProUGUI>();
	    defaultFont = tmpText.font;
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
	                if (anim != null) anim.SetBool("Text", true);
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
	        var lengthOfDisplayedText = Mathf.CeilToInt( Mathf.Lerp(0, charCount - 1,  Mathf.Clamp(progress / timeToFinish, 0, 1)));
	        TriggerSpeakerAnimator(lengthOfDisplayedText);
//            Debug.Log($"charCount : {charCount}, substring : {length}, ticks: {progress}, readyToAdvance: {readyToAdvance}");
	        displayText = wholeText.Substring(0, lengthOfDisplayedText);
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

    private void TriggerSpeakerAnimator(int length)
    {
        if (length % charsPerTrigger == 0 &&
            portraitName != "" &&
            lookupPeeps.ContainsKey(portraitName) &&
            lookupPeeps[portraitName].speakerAnimator != null &&
            length != lastTriggeredCount)
        {
            int triggerIndex = 1;
            if (lookupPeeps[portraitName].speakingTriggers.Count >= 1)
            {
                triggerIndex = Mathf.CeilToInt(Random.value * lookupPeeps[portraitName].speakingTriggers.Count) -1;
                Debug.Log(lookupPeeps[portraitName].speakingTriggers[triggerIndex]);

                lastTriggeredCount = length;
            }
            lookupPeeps[portraitName].speakerAnimator.SetTrigger(lookupPeeps[portraitName].speakingTriggers[triggerIndex]);
        }
    }

    private void SetPortrait()
    {
        var hasPortrait = false;
        portraitName = "";
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
                hasPortrait = true;
                portraitName = speaker.speakerName;

                if (speaker.speakerFont != null)
                {
                    tmpText.font = speaker.speakerFont;
                }
                else
                {
                    tmpText.font = defaultFont;
                }
            }
        }
        if(anim != null) anim.SetBool("Portrait", hasPortrait) ;
        if (!hasPortrait)
        {
            tmpText.font = defaultFont;
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
    public Transform camAngle;
    public Animator speakerAnimator;
    public List<string> speakingTriggers;
    public TMP_FontAsset speakerFont;
}
