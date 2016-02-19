/*============================================================================== 
 * Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/
using UnityEngine;
using System.Collections;
using Vuforia;

public class FrameMarkerUIView : ISampleAppUIView {
    
    #region PUBLIC_PROPERTIES
    public CameraDevice.FocusMode FocusMode
    {
        get {
            return m_focusMode;
        }
        set {
            m_focusMode = value;
        }
    }
    #endregion PUBLIC_PROPERTIES
    
    #region PUBLIC_MEMBER_VARIABLES
    public event System.Action TappedToClose;
    public SampleAppUIBox mBox;
    public SampleAppUILabel mFrameMarkerLabel;
    public SampleAppUILabel mCameraLabel;
    public SampleAppUIButton mCloseButton;
    public SampleAppUICheckButton mAboutLabel;
    #endregion PUBLIC_MEMBER_VARIABLES
    
    #region PRIVATE_MEMBER_VARIABLES
    private CameraDevice.FocusMode m_focusMode;
    private SampleAppsUILayout mLayout;
    #endregion PRIVATE_MEMBER_VARIABLES
    
    #region PUBLIC_METHODS
    
    public void LoadView()
    {

        mLayout = new SampleAppsUILayout();
        mFrameMarkerLabel = mLayout.AddLabel("Frame Markers");
        mAboutLabel = mLayout.AddSimpleButton("About");
        mLayout.AddGap(2);
        Rect CloseButtonRect = new Rect(0, Screen.height - (100 * Screen.width) / 800.0f, Screen.width, (70.0f * Screen.width) / 800.0f);
        mCloseButton = mLayout.AddButton("Close", CloseButtonRect); 
    }
    
    public void UnLoadView()
    {
        mFrameMarkerLabel = null;
        mAboutLabel = null;
        mCameraLabel = null;
    }
    
    public void UpdateUI(bool tf)
    {
        if(!tf)
        {
            return;
        }

        mLayout.Draw();
    }

    public void OnTappedToClose ()
    {
        if(this.TappedToClose != null)
        {
            this.TappedToClose();
        }
    }
    #endregion PUBLIC_METHODS
}

