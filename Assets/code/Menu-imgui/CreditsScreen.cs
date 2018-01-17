using UnityEngine;
using UnityEngine.SceneManagement;


public class CreditsScreen : MonoBehaviour 
{
    bool FirstLaunch = true;
    bool pushedGoBack;

    public Texture2D fadeTexture;
    public Texture2D Credits;
    public float buttonWidthPercent = .20f;
    public float buttonHeightPercent = .05f;
    public float buttonDisplacementPercent = .8f;
	public float buttonDisplacementPercentX = .6f;
    public int backButtonIndex = 0;
    public float fadeSpeed = .4f;
    public float CreditsHeight = .03f;
    public float CreditsScale = 1f;

    private Rect TitlePos;
    private float alpha = 1f;

	public Texture2D background;
	public GUISkin guiSkin;

	private int originalFontSize = 32;
	private float nativeResY = 530.0f;

    public void OnGUI()
    {
		GUI.skin = guiSkin;
		GUIStyle backgroundStyle = new GUIStyle();
		backgroundStyle.normal.background = background;
		GUI.Label(new Rect(0,0,Screen.width, Screen.height), "", backgroundStyle);
		GUIStyle menuButton = new GUIStyle(guiSkin.button);
		menuButton.fontSize = (int) (originalFontSize * (Screen.height/nativeResY));

        //Credtis
        if (Credits != null)
        {
            TitlePos.width = Credits.width * CreditsScale;
            TitlePos.height = Credits.height * CreditsScale;
            TitlePos.x = Screen.width / 2 - TitlePos.width / 2;
            TitlePos.y = CreditsHeight;
            GUI.DrawTexture(TitlePos, Credits);
        }

        float buttonWidth = Screen.width * buttonWidthPercent;
        float buttonHeight = Screen.height * buttonHeightPercent;

        if (FirstLaunch)
        {
            alpha = MenuFade.FadeIn(fadeSpeed, fadeTexture, alpha);
            if (alpha <= 0)
            {
                FirstLaunch = false;
            }
        }

        else if (GUI.Button(new Rect(Screen.width * buttonDisplacementPercentX, 
		                             Screen.height * buttonDisplacementPercent, buttonWidth, buttonHeight), "Go Back", menuButton))
        {
            pushedGoBack = true;
        }

        if (pushedGoBack)
        {
            alpha = MenuFade.FadeOut(fadeSpeed, fadeTexture, alpha);
            if(alpha >= 1)
            {
                SceneManager.LoadScene(backButtonIndex);
            }
        }
    }
}

