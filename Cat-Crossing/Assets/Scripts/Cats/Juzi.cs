using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Juzi : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    [Header("Gaze Detection")]
    [SerializeField] private float gazeRequiredSeconds = 3f;
    [SerializeField] private float gazeMaxDistance = 8f;
    [SerializeField] private float gazeAngleThreshold = 30f;

    [Header("Walk & Circle")]
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float circleRadius = 1.5f;
    [SerializeField] private float circleSpeed = 60f; // degrees per second

    [Header("Dialogue")]
    [SerializeField] private string dialogueLine = "Meow!";

    private bool _plotStarted;
    private bool _cancelled;

    private void Start()
    {
        if (player == null)
            Debug.LogWarning("[Juzi] Player reference not assigned.");
    }

    private void Update()
    {
        if (!_plotStarted && IsPlayerGazing())
            StartCoroutine(RunPlot());
    }

    // Returns true while the player is looking at Juzi — used each frame in Update before the plot starts
    private bool IsPlayerGazing() => false; // TODO

    private IEnumerator RunPlot()
    {
        _plotStarted = true;

        yield return WaitForPlayerGaze();
        if (_cancelled) yield break;

        yield return NoticePlayer();
        if (_cancelled) yield break;

        yield return WalkToPlayer();
        if (_cancelled) yield break;

        yield return CircleAroundPlayer();
        if (_cancelled) yield break;

        yield return Speak();
    }

    // Phase 1: wait until the player has been looking at Juzi for gazeRequiredSeconds
    private IEnumerator WaitForPlayerGaze()
    {
        yield break; // TODO
    }

    // Phase 2: Juzi reacts — play a notice animation and wait for it to finish
    private IEnumerator NoticePlayer()
    {
        yield break; // TODO
    }

    // Phase 3: walk toward the player until within stopDistance, playing walk animation
    private IEnumerator WalkToPlayer()
    {
        yield break; // TODO
    }

    // Phase 4: orbit once (360°) around the player at circleRadius
    private IEnumerator CircleAroundPlayer()
    {
        yield break; // TODO
    }

    // Phase 5: show dialogue / play voice clip and wait for it to end
    private IEnumerator Speak()
    {
        yield break; // TODO
    }
}
