// Editor utility — run via  Assets > Create > XR > Hand Interactions > Fist Hand Shape
// This creates a pre-configured XRHandShape asset for a closed fist (all fingers fully curled).
// After creation, tweak the per-finger tolerances in the Inspector if needed.

using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using System.Collections.Generic;

public static class CreateFistHandShape
{
    [MenuItem("Assets/Create/XR/Hand Interactions/Fist Hand Shape")]
    static void Create()
    {
        var shape = ScriptableObject.CreateInstance<XRHandShape>();

        // For a closed fist all fingers curl toward the palm.
        // desired=1 means fully curled. lowerTolerance gives a window below that.
        // Thumb curls less than the other fingers, so we're more generous there.
        shape.fingerShapeConditions = new List<XRFingerShapeCondition>
        {
            MakeCondition(XRHandFingerID.Thumb,   fullCurlDesired: 0.7f, lower: 0.7f, upper: 0.3f),
            MakeCondition(XRHandFingerID.Index,   fullCurlDesired: 1.0f, lower: 0.4f, upper: 0.0f),
            MakeCondition(XRHandFingerID.Middle,  fullCurlDesired: 1.0f, lower: 0.4f, upper: 0.0f),
            MakeCondition(XRHandFingerID.Ring,    fullCurlDesired: 1.0f, lower: 0.4f, upper: 0.0f),
            MakeCondition(XRHandFingerID.Little,  fullCurlDesired: 1.0f, lower: 0.45f, upper: 0.0f),
        };

        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/FistHandShape.asset");
        AssetDatabase.CreateAsset(shape, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = shape;
        EditorGUIUtility.PingObject(shape);
        Debug.Log($"[CreateFistHandShape] Created asset at {path}");
    }

    static XRFingerShapeCondition MakeCondition(XRHandFingerID finger, float fullCurlDesired, float lower, float upper)
    {
        var condition = new XRFingerShapeCondition();
        condition.fingerID = finger;
        condition.targets = new[]
        {
            new XRFingerShapeCondition.Target
            {
                shapeType      = XRFingerShapeType.FullCurl,
                desired        = fullCurlDesired,
                lowerTolerance = lower,
                upperTolerance = upper,
            }
        };
        return condition;
    }
}
