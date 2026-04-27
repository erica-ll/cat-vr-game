using System.Collections;
using UnityEngine;
using Whisper;
using Whisper.Utils;

public class WhisperTest : MonoBehaviour
{
    public WhisperManager whisperManager;
    public string wavFileName = "Audio/test.wav";

    void Start()
    {
        StartCoroutine(RunWhisperTest());
    }

    private IEnumerator RunWhisperTest()
    {
        // Build the full path to the WAV file in Assets
        string filePath = System.IO.Path.Combine(Application.dataPath, wavFileName);

        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError($"WAV file not found at: {filePath}");
            yield break;
        }

        // Load the WAV file as an AudioClip
        string fileUrl = "file://" + filePath;
        using (var www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(fileUrl, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load WAV file: {www.error}");
                yield break;
            }

            AudioClip clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);

            if (clip == null)
            {
                Debug.LogError("AudioClip is null after loading.");
                yield break;
            }

            Debug.Log("WAV file loaded successfully. Running Whisper transcription...");

            // --- Run in English ---
            whisperManager.language = "en";
            var englishTask = whisperManager.GetTextAsync(clip);
            yield return new WaitUntil(() => englishTask.IsCompleted);

            string englishResult = englishTask.Result?.Result ?? "[No result]";

            // --- Run in Chinese ---
            whisperManager.language = "zh";
            var chineseTask = whisperManager.GetTextAsync(clip);
            yield return new WaitUntil(() => chineseTask.IsCompleted);

            string chineseResult = chineseTask.Result?.Result ?? "[No result]";

            // --- Print both on the same line ---
            Debug.Log($"[English]: {englishResult}    |    [Chinese]: {chineseResult}");
        }
    }
}