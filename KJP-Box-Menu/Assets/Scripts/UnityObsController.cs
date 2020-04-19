using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnityObsController : MonoBehaviour
{
    protected OBSWebsocket _obs;

    [SerializeField]
    private TextMeshProUGUI btnConnect, tbPluginVersion, tbOBSVersion, tbCurrentScene, tbRecording, tbStreaming;
    [SerializeField]
    private TextMeshProUGUI txtStreamTime, txtKbitsSec, txtBytesSec, txtFramerate, txtStrain, txtDroppedFrames, txtTotalFrames;
    [SerializeField]
    private GameObject recordButton;
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private string serverIP = "ws://localhost:4444", serverPassword;

    private string updateRecordingText;


    public void Awake()
    {
        _obs = new OBSWebsocket();

        _obs.Connected += onConnect;
        _obs.Disconnected += onDisconnect;

        _obs.SceneChanged += onSceneChange;
        _obs.SceneCollectionChanged += onSceneColChange;
        _obs.ProfileChanged += onProfileChange;
        _obs.TransitionChanged += onTransitionChange;
        _obs.TransitionDurationChanged += onTransitionDurationChange;

        _obs.StreamingStateChanged += onStreamingStateChange;
        _obs.RecordingStateChanged += onRecordingStateChange;

        _obs.StreamStatus += onStreamData;

        recordButton.SetActive(false);
    }

    private void onConnect(object sender, EventArgs e)
    {
        btnConnect.text = "Disconnect";
        recordButton.SetActive(true);

        var versionInfo = _obs.GetVersion();
        tbPluginVersion.text = versionInfo.PluginVersion;
        tbOBSVersion.text = versionInfo.OBSStudioVersion;



        var streamStatus = _obs.GetStreamingStatus();
        if (streamStatus.IsStreaming)
            onStreamingStateChange(_obs, OutputState.Started);
        else
            onStreamingStateChange(_obs, OutputState.Stopped);

        if (streamStatus.IsRecording)
            onRecordingStateChange(_obs, OutputState.Started);
        else
            onRecordingStateChange(_obs, OutputState.Stopped);
    }

    private void onDisconnect(object sender, EventArgs e)
    {
        btnConnect.text = "Connect";
        recordButton.SetActive(false);
    }

    private void onSceneChange(OBSWebsocket sender, string newSceneName)
    {
        tbCurrentScene.text = newSceneName;
    }

    private void onSceneColChange(object sender, EventArgs e)
    {
        Debug.Log($"SceneCollectionChanged :{_obs.GetCurrentSceneCollection()}");
    }

    private void onProfileChange(object sender, EventArgs e)
    {
        Debug.Log($"ProfileChange :{_obs.GetCurrentProfile()}");
    }

    private void onTransitionChange(OBSWebsocket sender, string newTransitionName)
    {
        Debug.Log($"TransitionChange :{newTransitionName}");
    }

    private void onTransitionDurationChange(OBSWebsocket sender, int newDuration)
    {
        Debug.Log($"TransitionDurationChange :{newDuration}");
    }

    private void onStreamingStateChange(OBSWebsocket sender, OutputState newState)
    {
        string state;
        switch (newState)
        {
            case OutputState.Starting:
                state = "Stream starting...";
                break;

            case OutputState.Started:
                state = "Stop streaming";
                break;

            case OutputState.Stopping:
                state = "Stream stopping...";
                break;

            case OutputState.Stopped:
                state = "Start streaming";
                break;

            default:
                state = "State unknown";
                break;
        }

        Debug.Log($"StreamingStateChange: {state}");
        if(tbStreaming)
            tbStreaming.text = state;
    }

    private void onRecordingStateChange(OBSWebsocket sender, OutputState newState)
    {
        string state;
        switch (newState)
        {
            case OutputState.Starting:
                state = "Recording starting...";
                break;

            case OutputState.Started:
                state = "Stop recording";
                break;

            case OutputState.Stopping:
                state = "Recording stopping...";
                break;

            case OutputState.Stopped:
                state = "Start recording";
                break;

            default:
                state = "State unknown";
                break;
        }

        Debug.Log($"RecordingStateChange: {state}");
        updateRecordingText = state;
    }
    private void Update()
    {
        if (updateRecordingText != null)
        {
            tbRecording.text = updateRecordingText;
            updateRecordingText = null;
        }
    }

    private void onStreamData(OBSWebsocket sender, StreamStatus data)
    {
        if(txtStreamTime)
            txtStreamTime.text = data.TotalStreamTime.ToString() + " sec";
        if (txtKbitsSec)
            txtKbitsSec.text = data.KbitsPerSec.ToString() + " kbit/s";
        if (txtBytesSec)
            txtBytesSec.text = data.BytesPerSec.ToString() + " bytes/s";
        if (txtFramerate)
            txtFramerate.text = data.FPS.ToString() + " FPS";
        if (txtStrain)
            txtStrain.text = (data.Strain * 100).ToString() + " %";
        if (txtDroppedFrames)
            txtDroppedFrames.text = data.DroppedFrames.ToString();
        if (txtTotalFrames)
            txtTotalFrames.text = data.TotalFrames.ToString();
    }

    public void btnToggleRecording_Click()
    {
        _obs.ToggleRecording();
    }

    public void btnConnect_Click()
    {
        if (!_obs.IsConnected)
        {
            try
            {
                _obs.Connect(serverIP, serverPassword);
            }
            catch (AuthFailureException)
            {
                Debug.LogError("Authentication failed.");
                return;
            }
            catch (ErrorResponseException ex)
            {
                Debug.LogError("Connect failed : " + ex.Message);
                return;
            }
        }
        else
        {
            _obs.Disconnect();
        }
    }

}
