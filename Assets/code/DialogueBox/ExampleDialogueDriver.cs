using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class ExampleDialogueDriver : MonoBehaviour {
    private Animator anim;
    private Renderer renderThing;
    private DialogueBox dialogueThing;

    private int currentPhase = 0;
    private bool isAllFinished;
    private bool seeded;

    public List<string> dialogueItems; 

    // Use this for initialization
	void Start ()
	{
	    anim = GetComponent<Animator>();
	    anim.enabled = false;
	    renderThing =  GetComponentInChildren<Renderer>();
	    renderThing.enabled = false;

	    dialogueThing = GetComponent<DialogueBox>();
	    dialogueThing.enabled = false;
	}

    // Update is called once per frame
	void Update ()
	{
	    if (currentPhase == 0 && !seeded)
	    {
	        anim.enabled = true;
	        renderThing.enabled = true;
	        dialogueThing.LoadDialogue(dialogueItems[currentPhase]);
	        seeded = true;
	    }
        else if (currentPhase < dialogueItems.Count  && !seeded && dialogueItems.Count > 0 && currentPhase > 0)
        {
            dialogueThing.LoadDialogue(dialogueItems[currentPhase]);
            seeded = true;
        }
        else if (!seeded)
        {
            anim.SetTrigger("duck");
        }

	    currentPhase = advancePhase(currentPhase);

        if (dialogueThing.isTalking && !dialogueThing.Finished)
            triggerChatter();

    }

    private void triggerChatter()
    {
        if (!(anim.GetCurrentAnimatorStateInfo(1).IsName("UI_Chatter1") || anim.GetCurrentAnimatorStateInfo(1).IsName("UI_Chatter2")))
        {
            anim.SetTrigger("chatter1");
        }
        else
        {
            anim.SetTrigger("chatterContinue");
        }
    }



    private int advancePhase( int currentPhase)
    {

        if (Input.GetMouseButtonDown(0))
        {
            seeded = false;
            Debug.Log(string.Format("Advancing to scene{0}", currentPhase));
            return currentPhase + 1;
        }
        
        return currentPhase;
    }

    //This is defined as an animation trigger on the Imported model
    public void HeadDown()
    {
        anim.enabled = false;
        renderThing.enabled = false;
        dialogueThing.enabled = false;
    }

    public void HeadsUp()
    {
        dialogueThing.enabled = true;
        seeded = false;
    }
}
