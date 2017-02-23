/*
Necromancer GUI Demo Script
Author: Jason Wentzel
jc_wentzel@ironboundstudios.com

In this script you'll find some handy little functions for some of the 
Custom elements in the skin, these should help you create your own;

AddSpikes (not perfect but works well enough if you’re careful with your window widths)
FancyTop (just an example of using the elements to do a centered header graphic)
WaxSeal (adds the waxseal and ribbon to the right of the window)
DeathBadge (adds the iconFrame, skull, and ribbon elements properly aligned)

*/

using UnityEngine;

public class GuiNecroScript : MonoBehaviour
{
    // Window activators
    public bool doTaskBar = true;

    // Necro skin control attributes
    private float leafOffset;
    private float frameOffset;
    private float skullOffset;

    private float RibbonOffsetX;
    private float FrameOffsetX;
    private float SkullOffsetX;
    private float RibbonOffsetY;
    private float FrameOffsetY;
    private float SkullOffsetY;

    private float WSwaxOffsetX;
    private float WSwaxOffsetY;
    private float WSribbonOffsetX;
    private float WSribbonOffsetY;

    private float spikeCount;

    // This script will only work with the Necromancer skin
    public GUISkin mySkin;

    // Task bar window
    public static float taskBarHeight = 100.0f;
    public static float taskBarMarginX = 100.0f;
    public static float taskBarWidth = Screen.width - (taskBarMarginX * 2.0f);
    private Rect windowTaskBar;

    // Custom skin attributes
    private Vector2 scrollPosition;
    private float HroizSliderValue = 0.5f;
    private float VertSliderValue = 0.5f;
    private bool ToggleBTN = false;

    // Buttons
    public GUIContent buttonContent;
    public float btnWidth = 80.0f;
    public float btnHeight = 80.0f;

    public void AddSpikes(float winX)
    {
        spikeCount = Mathf.Floor(winX - 152) / 22;
        GUILayout.BeginHorizontal();
        GUILayout.Label("", "SpikeLeft");

        for (int i = 0; i < spikeCount; i++)
        {
            GUILayout.Label("", "SpikeMid");
        }

        GUILayout.Label("", "SpikeRight");
        GUILayout.EndHorizontal();
    }

    public void FancyTop(float topX)
    {
        leafOffset = (topX / 2) - 64;
        frameOffset = (topX / 2) - 27;
        skullOffset = (topX / 2) - 20;
        GUI.Label(new Rect(leafOffset, 18, 0, 0), "", "GoldLeaf");
        GUI.Label(new Rect(frameOffset, 3, 0, 0), "", "IconFrame");	
        GUI.Label(new Rect(skullOffset, 12, 0, 0), "", "Skull");
    }

    public void WaxSeal(float x, float y)
    {
        WSwaxOffsetX = x - 120;
        WSwaxOffsetY = y - 115;
        WSribbonOffsetX = x - 114;
        WSribbonOffsetY = y - 83;

        GUI.Label(new Rect(WSribbonOffsetX, WSribbonOffsetY, 0, 0), "", "RibbonBlue");
        GUI.Label(new Rect(WSwaxOffsetX, WSwaxOffsetY, 0, 0), "", "WaxSeal");
    }

    public void DeathBadge(int x, int y)
    {
        RibbonOffsetX = x;
        FrameOffsetX = x + 3;
        SkullOffsetX = x + 10;
        RibbonOffsetY = y + 22;
        FrameOffsetY = y;
        SkullOffsetY = y + 9;

        GUI.Label(new Rect(RibbonOffsetX, RibbonOffsetY, 0, 0), "", "RibbonRed");
        GUI.Label(new Rect(FrameOffsetX, FrameOffsetY, 0, 0), "", "IconFrame");	
        GUI.Label(new Rect(SkullOffsetX, SkullOffsetY, 0, 0), "", "Skull");	
    }

    public void DoTaskBar(int windowID)
    {
        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        GUI.Button(new Rect(btnWidth * 0.5f, (taskBarHeight * 0.5f) - (btnHeight * 0.5f), btnWidth, btnHeight), buttonContent);
        //GUI.Button(new Rect(0.0f, (taskBarHeight * 0.5f) - btnHeight * 0.5f, btnWidth, btnHeight), buttonContent);
        GUILayout.EndHorizontal();
    }

    public void OnGUI()
    {
        GUI.skin = mySkin;

        taskBarWidth = Screen.width - (taskBarMarginX * 2.0f);
        windowTaskBar = new Rect(taskBarMarginX, Screen.height - (taskBarHeight), taskBarWidth, taskBarHeight);

        if (doTaskBar)
            windowTaskBar = GUI.Window(0, windowTaskBar, DoTaskBar, "Place cards here!");
    }
}