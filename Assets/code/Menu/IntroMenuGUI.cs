using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroMenuGUI : MonoBehaviour {

    bool pushedNewGame = false;
    bool pushedQuit = false;
    bool creditsPushed = false;

    public Texture2D title;
    public float buttonWidthPercent = .20f;
    public float buttonHeightPercent = .05f;
    public float spacing = 10f;
    public float TitleHeight = .03f;
    public float TitleScale = 1f;
    public float fadeOutSpeed = 0.4f;
    public float verticalButtonDisp = .6f;
    public float horizontalButtonDisp = .5f;

    public int newGameIndex = 4;
    public int creditsIndex = 2;

    public Texture2D fadeTexture;
	public Texture2D background;
	public GUISkin guiSkin;

    private Rect TitlePos;
    private float alpha = 0.001f;
	private int originalFontSize = 32;
	private float nativeResY = 530.0f;

    void OnGUI()
    {
		GUI.skin = guiSkin;
		GUIStyle backgroundStyle = new GUIStyle();
		backgroundStyle.normal.background = background;
		GUI.Label(new Rect(0,0,Screen.width, Screen.height), "", backgroundStyle);
		GUIStyle menuButton = new GUIStyle(guiSkin.button);
		menuButton.fontSize = (int) (originalFontSize * (Screen.height/nativeResY));

        //Title
        if (title != null)
        {
            TitlePos.width = title.width * TitleScale;
            TitlePos.height = title.height * TitleScale;
            TitlePos.x = Screen.width / 2 - TitlePos.width / 2;
            TitlePos.y = TitleHeight;
            GUI.DrawTexture(TitlePos, title);
        }

        float buttonWidth = Screen.width * buttonWidthPercent;
        float buttonHeight = Screen.height * buttonHeightPercent;

        if (GUI.Button(new Rect(Screen.width * horizontalButtonDisp - buttonWidth/2, 
		                        Screen.height * verticalButtonDisp + buttonHeight * 1 + spacing, 
		                        buttonWidth , buttonHeight), "New Game", menuButton))
        {
            if (NoOtherButtonsPushed())
                pushedNewGame = true;
        }

        if (GUI.Button(new Rect(Screen.width * horizontalButtonDisp - buttonWidth / 2, 
		                        Screen.height * verticalButtonDisp + buttonHeight * 2 + 2 * spacing, 
		                        buttonWidth, buttonHeight), "Credits", menuButton))
        {
            if (NoOtherButtonsPushed())
                creditsPushed = true;
        }

        if (GUI.Button(new Rect(Screen.width * horizontalButtonDisp - buttonWidth / 2, 
		                        Screen.height * verticalButtonDisp + buttonHeight * 3 + 3 * spacing, 
		                        buttonWidth, buttonHeight), "Quit", menuButton))
        {
            if (NoOtherButtonsPushed())
                pushedQuit = true;
        }

        if (pushedNewGame)
        {
            alpha = MenuFade.FadeOut(fadeOutSpeed, fadeTexture, alpha);
            if (alpha >= 1)
                SceneManager.LoadScene(newGameIndex);

        }

        if (creditsPushed)
        {
            alpha = MenuFade.FadeOut(fadeOutSpeed, fadeTexture, alpha);
            if (alpha >= 1)
                SceneManager.LoadScene(creditsIndex);
        }

        if (pushedQuit)
        {
            Application.Quit();
            pushedQuit = false;
        }
    }

    bool NoOtherButtonsPushed()
    {
        return !pushedNewGame && !pushedQuit && !creditsPushed;
    }





}
