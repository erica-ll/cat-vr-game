using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using Whisper;
using System.Collections;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

[RequireComponent(typeof(AudioSource))]
public class VoiceInputManager : MonoBehaviour
{
    [Header("Whisper")]
    [SerializeField] private WhisperManager whisperManager;

    [Header("Hand Gesture")]
    [Tooltip("Add an XRHandTrackingEvents component (set to Left) to this or any GameObject and assign it here.")]
    [SerializeField] private XRHandTrackingEvents leftHandTrackingEvents;
    [Tooltip("Assign the FistHandShape asset (create it via Assets > Create > XR > Hand Interactions > Hand Shape).")]
    [SerializeField] private XRHandShape fistHandShape;
    [Tooltip("Seconds the fist must be held before recording starts. Prevents accidental triggers.")]
    [SerializeField] private float gestureHoldTime = 0.15f;

    [Header("Recording")]
    [SerializeField] private int maxRecordingSeconds = 30;
    [SerializeField] private int sampleRate = 16000;

    [Header("Debug")]
    [SerializeField] private bool playbackAfterRecording = true;

    [Header("Events")]
    public UnityEvent<string> onTranscriptionComplete;
    public UnityEvent onRecordingStarted;
    public UnityEvent onRecordingEnded;
    public UnityEvent onFistDetected;
    public UnityEvent onFistReleased;

    private bool _isRecording;
    private string _micDevice;
    private AudioClip _recordingClip;
    private AudioSource _audioSource;

    // Gesture state
    private bool _gestureRaw;       // true while fist shape is detected each update
    private bool _gestureConfirmed; // true once hold time has passed and recording started
    private float _gestureHoldStart;

    // Debug heartbeat
    private float _nextDebugLog;

    private void Start()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            Debug.Log("[VoiceInputManager] Requesting microphone permission.");
        }
#endif
        Debug.Log($"[VoiceInputManager] Available microphones: {string.Join(", ", Microphone.devices)}");

        _audioSource = GetComponent<AudioSource>();
        // Ensure 2D playback on Quest — Oculus Spatializer can interfere otherwise
        _audioSource.spatialBlend = 0f;
        _audioSource.bypassEffects = true;

        if (Microphone.devices.Length > 0)
            _micDevice = Microphone.devices[0];
        else
            Debug.LogWarning("[VoiceInputManager] No microphone detected.");

        if (leftHandTrackingEvents == null)
            Debug.LogWarning("[VoiceInputManager] leftHandTrackingEvents is not assigned.");
        if (fistHandShape == null)
            Debug.LogWarning("[VoiceInputManager] fistHandShape is not assigned.");
    }

    private void OnEnable()
    {
        if (leftHandTrackingEvents != null)
            leftHandTrackingEvents.jointsUpdated.AddListener(OnLeftHandJointsUpdated);
    }

    private void OnDisable()
    {
        if (leftHandTrackingEvents != null)
            leftHandTrackingEvents.jointsUpdated.RemoveListener(OnLeftHandJointsUpdated);
    }

    private void OnLeftHandJointsUpdated(XRHandJointsUpdatedEventArgs args)
    {
        if (fistHandShape == null) return;

        bool detected = leftHandTrackingEvents.handIsTracked && fistHandShape.CheckConditions(args);

        // Heartbeat log every 2 seconds — visible in adb logcat
        float now = Time.timeSinceLevelLoad;
        if (now >= _nextDebugLog)
        {
            Debug.Log($"[VoiceInputManager] fist={detected}  confirmed={_gestureConfirmed}  recording={_isRecording}");
            _nextDebugLog = now + 2f;
        }

        if (detected && !_gestureRaw)
        {
            // Fist just started
            _gestureRaw = true;
            _gestureHoldStart = now;
            onFistDetected?.Invoke();
        }
        else if (!detected && _gestureRaw)
        {
            // Fist released
            _gestureRaw = false;
            if (_gestureConfirmed)
            {
                _gestureConfirmed = false;
                StopRecordingAndTranscribe();
            }
            else
            {
                onFistReleased?.Invoke();
            }
        }

        // Confirm gesture once hold time has elapsed
        if (_gestureRaw && !_gestureConfirmed && now - _gestureHoldStart >= gestureHoldTime)
        {
            _gestureConfirmed = true;
            StartRecording();
        }
    }

    private void StartRecording()
    {
        if (_isRecording || _micDevice == null) return;

        _isRecording = true;
        _recordingClip = Microphone.Start(_micDevice, false, maxRecordingSeconds, sampleRate);
        onRecordingStarted?.Invoke();
        Debug.Log("[VoiceInputManager] Recording started.");
    }

    private void StopRecordingAndTranscribe()
    {
        if (!_isRecording) return;

        int samples = Microphone.GetPosition(_micDevice);
        Microphone.End(_micDevice);
        _isRecording = false;
        onRecordingEnded?.Invoke();
        Debug.Log($"[VoiceInputManager] Recording stopped. Samples: {samples} ({(float)samples / sampleRate:F2}s)");

        if (samples <= 0 || _recordingClip == null) return;

        AudioClip trimmed = TrimClip(_recordingClip, samples);
        _recordingClip = null;

        if (playbackAfterRecording)
        {
            _audioSource.clip = trimmed;
            _audioSource.Play();
        }

        StartCoroutine(TranscribeClip(trimmed));
    }

    private IEnumerator TranscribeClip(AudioClip clip)
    {
        var task = whisperManager.GetTextAsync(clip);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError($"[VoiceInputManager] Transcription failed: {task.Exception}");
            yield break;
        }

        string text = task.Result?.Result;
        Debug.Log($"[VoiceInputManager] Transcription result: '{text}'");
        if (!string.IsNullOrWhiteSpace(text))
        {
            Debug.Log($"[VoiceInputManager] Transcribed: {text}");
            onTranscriptionComplete?.Invoke(text);
        }
    }

    private AudioClip TrimClip(AudioClip source, int sampleCount)
    {
        float[] data = new float[sampleCount * source.channels];
        source.GetData(data, 0);
        AudioClip trimmed = AudioClip.Create("recording", sampleCount, source.channels, source.frequency, false);
        trimmed.SetData(data, 0);
        return trimmed;
    }
}
