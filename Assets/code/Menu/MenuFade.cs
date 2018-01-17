using System.Collections.Generic;
using UnityEngine;


public static class MenuFade
{
    //Fade Screen, based on Griffo's answer
    //http://answers.unity3d.com/questions/341350/how-to-fade-out-a-scene.html
    

    private const float drawDepth = -1000;

    private const float fadeDir = 1;

    public static float FadeOut(float fadeSpeed, Texture2D fadeTex, float alpha)
    {

        alpha += fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        drawTex(fadeTex, alpha);

        return alpha;
    }

    public static float FadeIn(float fadeSpeed, Texture2D fadeTex, float alpha)
    {

        alpha += fadeSpeed * Time.deltaTime * -1;
        alpha = Mathf.Clamp01(alpha);

        drawTex(fadeTex, alpha);

        return alpha;
    }

    private static void drawTex(Texture2D fadeTex, float alpha)
    {
        Color temp = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.color = temp;

        GUI.depth = (int)drawDepth;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTex);
    }
}

