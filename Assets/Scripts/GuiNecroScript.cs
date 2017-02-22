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
    public bool doWindow0 = true;
    public bool doWindow1 = true;
    public bool doWindow2 = true;
    public bool doWindow3 = true;
    public bool doWindow4 = true;

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

//if you're using the spikes you'll need to find sizes that work well with them these are a few...
    private Rect windowRect0 = new Rect(500, 140, 350, 510);
    private Rect windowRect1 = new Rect(380, 40, 262, 420);
    private Rect windowRect2 = new Rect(700, 40, 306, 480);
    private Rect windowRect3 = new Rect(0, 40, 350, 500);

    //skin info
    private Vector2 scrollPosition;
    private float HroizSliderValue = 0.5f;
    private float VertSliderValue = 0.5f;
    private bool ToggleBTN = false;

    private string NecroText = "This started as a question... How flexible is the built in GUI in unity? The answer... pretty damn flexible! "
        + "At first I wasn’t so sure; it seemed no one ever used it to make a non OS style GUI at least not a publicly available one."
        + "So I decided I couldn’t be sure until I tried to develop a full GUI, Long story short Necromancer was the result and is now available to the"
        + "general public, free for comercial and non-comercial use. I only ask that if you add something Share it.   Credits to Kevin King for the fonts.";

    public void AddSpikes(float winX)
    {
        spikeCount = Mathf.Floor(winX - 152) / 22;
        GUILayout.BeginHorizontal();
        GUILayout.Label("", "SpikeLeft");//-------------------------------- custom

        for (int i = 0; i < spikeCount; i++)
        {
            GUILayout.Label("", "SpikeMid");//-------------------------------- custom
        }

        GUILayout.Label("", "SpikeRight");//-------------------------------- custom
        GUILayout.EndHorizontal();
    }

    public void FancyTop(float topX)
    {
        leafOffset = (topX / 2) - 64;
        frameOffset = (topX / 2) - 27;
        skullOffset = (topX / 2) - 20;
        GUI.Label(new Rect(leafOffset, 18, 0, 0), "", "GoldLeaf");//-------------------------------- custom	
        GUI.Label(new Rect(frameOffset, 3, 0, 0), "", "IconFrame");//-------------------------------- custom	
        GUI.Label(new Rect(skullOffset, 12, 0, 0), "", "Skull");//-------------------------------- custom	
    }

    public void WaxSeal(float x, float y)
    {
        WSwaxOffsetX = x - 120;
        WSwaxOffsetY = y - 115;
        WSribbonOffsetX = x - 114;
        WSribbonOffsetY = y - 83;

        GUI.Label(new Rect(WSribbonOffsetX, WSribbonOffsetY, 0, 0), "", "RibbonBlue");//-------------------------------- custom	
        GUI.Label(new Rect(WSwaxOffsetX, WSwaxOffsetY, 0, 0), "", "WaxSeal");//-------------------------------- custom	
    }

    public void DeathBadge(int x, int y)
    {
        RibbonOffsetX = x;
        FrameOffsetX = x + 3;
        SkullOffsetX = x + 10;
        RibbonOffsetY = y + 22;
        FrameOffsetY = y;
        SkullOffsetY = y + 9;

        GUI.Label(new Rect(RibbonOffsetX, RibbonOffsetY, 0, 0), "", "RibbonRed");//-------------------------------- custom	
        GUI.Label(new Rect(FrameOffsetX, FrameOffsetY, 0, 0), "", "IconFrame");//-------------------------------- custom	
        GUI.Label(new Rect(SkullOffsetX, SkullOffsetY, 0, 0), "", "Skull");//-------------------------------- custom	
    }

    public void DoMyWindow0(int windowID)
    {
        // use the spike function to add the spikes
        // note: were passing the width of the window to the function
        AddSpikes(windowRect0.width);

        GUILayout.BeginVertical();
        GUILayout.Space(8);
        GUILayout.Label("", "Divider");//-------------------------------- custom
        GUILayout.Label("Standard Label");
        GUILayout.Label("Short Label", "ShortLabel");//-------------------------------- custom
        GUILayout.Label("", "Divider");//-------------------------------- custom
        GUILayout.Button("Standard Button");
        GUILayout.Button("Short Button", "ShortButton");//-------------------------------- custom
        GUILayout.Label("", "Divider");//-------------------------------- custom
        ToggleBTN = GUILayout.Toggle(ToggleBTN, "This is a Toggle Button");
        GUILayout.Label("", "Divider");//-------------------------------- custom
        GUILayout.Box("This is a textbox\n this can be expanded by using \\n");
        GUILayout.TextField("This is a textfield\n You cant see this text!!");
        GUILayout.TextArea("This is a textArea\n this can be expanded by using \\n");
        GUILayout.EndVertical();

        // Make the windows be draggable.
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    public void DoMyWindow1(int windowID)
    {
        // use the spike function to add the spikes
        AddSpikes(windowRect1.width);

        GUILayout.BeginVertical();
        GUILayout.Label("", "Divider");//-------------------------------- custom
        GUILayout.Label("Plain Text", "PlainText");//------------------------------------ custom
        GUILayout.Label("Italic Text", "ItalicText");//---------------------------------- custom
        GUILayout.Label("Light Text", "LightText");//----------------------------------- custom
        GUILayout.Label("Bold Text", "BoldText");//------------------------------------- custom
        GUILayout.Label("Disabled Text", "DisabledText");//-------------------------- custom
        GUILayout.Label("Cursed Text", "CursedText");//------------------- custom
        GUILayout.Label("Legendary Text", "LegendaryText");//-------------------- custom
        GUILayout.Label("Outlined Text", "OutlineText");//--------------------------- custom
        GUILayout.Label("Italic Outline Text", "ItalicOutlineText");//---------------------------------- custom
        GUILayout.Label("Light Outline Text", "LightOutlineText");//----------------------------------- custom
        GUILayout.Label("Bold Outline Text", "BoldOutlineText");//----------------- custom
        GUILayout.EndVertical();
        // Make the windows be draggable.
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    public void DoMyWindow2(int windowID)
    {
        // use the spike function to add the spikes
        AddSpikes(windowRect2.width);

        GUILayout.Space(8);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true);
        GUILayout.Label(NecroText, "PlainText");
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        GUILayout.Space(8);
        HroizSliderValue = GUILayout.HorizontalSlider(HroizSliderValue, 0.0f, 1.1f);
        VertSliderValue = GUILayout.VerticalSlider(VertSliderValue, 0.0f, 1.1f, GUILayout.Height(70));
        DeathBadge(200, 350);
        GUILayout.EndVertical();
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    //bringing it all together
    public void DoMyWindow3(int windowID)
    {
        // use the spike function to add the spikes
        AddSpikes(windowRect3.width);

        //add a fancy top using the fancy top function
        FancyTop(windowRect0.width);

        GUILayout.Space(8);
        GUILayout.BeginVertical();
        GUILayout.Label("Necromancer");
        GUILayout.Label("", "Divider");
        GUILayout.Label("Necromancer is a free to use GUI for the unity community. this skin can be used in commercial and non-commercial products.", "LightText");
        GUILayout.Label("", "Divider");
        GUILayout.Space(8);
        doWindow0 = GUILayout.Toggle(doWindow0, "Standard Components");
        doWindow1 = GUILayout.Toggle(doWindow1, "Text Examples");
        doWindow2 = GUILayout.Toggle(doWindow2, "Sliders");
        GUILayout.Space(8);
        GUILayout.Label("", "Divider");
        GUILayout.Label("Please read through the source of this script to see", "PlainText");
        GUILayout.BeginHorizontal();
        GUILayout.Label("how to use special ", "PlainText");
        GUILayout.Label("Components ", "LegendaryText");
        GUILayout.Label("and ", "PlainText");
        GUILayout.Label("Functions ", "CursedText");
        GUILayout.Label("!", "PlainText");
        GUILayout.EndHorizontal();
        GUILayout.Label("", "Divider");
        GUILayout.Space(26);
        GUILayout.Label("Created By Jason Wentzel 2011", "SingleQuotes");
        GUILayout.EndVertical();

        // add a wax seal at the bottom of the window
        WaxSeal(windowRect3.width, windowRect3.height);

        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    public void OnGUI()
    {
        GUI.skin = mySkin;

        if (doWindow0)
            windowRect0 = GUI.Window(0, windowRect0, DoMyWindow0, "");
        //now adjust to the group. (0,0) is the topleft corner of the group.
        GUI.BeginGroup(new Rect(0, 0, 100, 100));
        // End the group we started above. This is very important to remember!
        GUI.EndGroup();

        if (doWindow1)
            windowRect1 = GUI.Window(1, windowRect1, DoMyWindow1, "");
        //now adjust to the group. (0,0) is the topleft corner of the group.
        GUI.BeginGroup(new Rect(0, 0, 100, 100));
        // End the group we started above. This is very important to remember!
        GUI.EndGroup();

        if (doWindow2)
            windowRect2 = GUI.Window(2, windowRect2, DoMyWindow2, "");
        //now adjust to the group. (0,0) is the topleft corner of the group.
        GUI.BeginGroup(new Rect(0, 0, 100, 100));
        // End the group we started above. This is very important to remember!
        GUI.EndGroup();

        if (doWindow3)
            windowRect3 = GUI.Window(3, windowRect3, DoMyWindow3, "");
        //now adjust to the group. (0,0) is the topleft corner of the group.
        GUI.BeginGroup(new Rect(0, 0, 100, 100));
        // End the group we started above. This is very important to remember!
        GUI.EndGroup();
    }
}