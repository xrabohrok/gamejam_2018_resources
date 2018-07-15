-> test

=== test
You stare at the monster in front you
bill: I don't know what you want from me here
bob: dude, shut up
- (rechoice)
* [I don't...] ...know what the heck is going on here. -> exclamation
+ [I... ok?] -> narrator
- (narrator)
bob: we are speaking from an ink derived text file.
bill: oh, yeah.  The file is exported to JSON from the ink engine and re-imported as a text asset.
bob: yes, and then this dialogue engine also is checking for a name at the beginning.
bill: yeah, if you don't specifiy someone, that's assumed to be the narrator
YES, GOOOD.
bill: bob: JESUS
- (reask)
* [why do I care?] -> why_care
* [what does this do for me?] -> what_do
+ [I think I get it.] -> ending

= why_care
bill: That's a great question
bob: Yeah.  Well, I mean, have you ever had to manage dialogue?
bill: actually, yeah, it sucks.
-> reask

= what_do
bill: how do I use this?
bob: you will need to apply the component correctly, to any arbitrary object that will control this particular set of dialogue.
- bob: Then, you will need to add one or more speakers to the speakers list. 
* ok...
- bob: Make sure that you name them correctly! Otherwise, lines won't line up.
You should also add some sort of visual effectors as well, otherwise only the narrator will be speaking.
-> reask

= ending
bob: I think you get it now.
bill: toodles!
-> DONE

=== exclamation
bob: I am not sure either.
bill: This is a tech demo, dumbass
+ oh? -> test.rechoice


-> DONE


