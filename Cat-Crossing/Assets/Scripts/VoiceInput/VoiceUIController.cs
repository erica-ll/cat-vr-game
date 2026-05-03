using UnityEngine;
using TMPro;

public class VoiceUIController : MonoBehaviour
{
    [SerializeField] private VoiceInputManager voiceInputManager;
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private TMP_Text statusText;

    private float _hideAt = -1f;

    private void Start()
    {
        panelGroup.alpha = 0f;
        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;

        voiceInputManager.onRecordingStarted.AddListener(OnRecordingStarted);
        voiceInputManager.onRecordingEnded.AddListener(OnRecordingEnded);
        voiceInputManager.onTranscriptionComplete.AddListener(OnTranscriptionComplete);
        voiceInputManager.onFistDetected.AddListener(ShowFistDetected);
        voiceInputManager.onFistReleased.AddListener(HideFistDetected);
    }

    private void Update()
    {
        // Tick the auto-hide timer for the result state
        if (_hideAt > 0f && Time.time >= _hideAt)
        {
            _hideAt = -1f;
            SetVisible(false, "");
        }
    }

    private void OnRecordingStarted()
    {
        SetVisible(true, "Recording...");
    }

    private void OnRecordingEnded()
    {
        SetVisible(true, "Transcribing...");
    }

    private void OnTranscriptionComplete(string text)
    {
        SetVisible(true, string.IsNullOrWhiteSpace(text) ? "(silence)" : text);
        _hideAt = Time.time + 5f;
    }

    public void ShowFistDetected()
    {
        // Called externally when fist gesture is first detected (before hold time)
        if (_hideAt > 0f) return; // don't override result display
        SetVisible(true, "Fist detected...");
    }

    public void HideFistDetected()
    {
        // Called when fist is released without triggering recording
        if (_hideAt > 0f) return;
        SetVisible(false, "");
    }

    private void SetVisible(bool visible, string text)
    {
        panelGroup.alpha = visible ? 1f : 0f;
        if (!string.IsNullOrEmpty(text)) statusText.text = text;
    }
}
