using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace StorytellingUtils.Editor
{
    public static class PlaymodeAnimationRecorder
    {
        [MenuItem("Tools/Animation/Add Pos KeyFrame")]
        private static void AddKeyFrame()
        {
            if (!Init(out var selectedGameObject, out var animationWindow, out var currentClip))
            {
                return;
            }

            var time = animationWindow.time;
            var bindings = AnimationUtility.GetCurveBindings(currentClip);

            if (bindings.Length == 0)
            {
                var newBindings = AnimationUtility.GetAnimatableBindings(selectedGameObject, selectedGameObject)
                    .Where(b => b.type == typeof(Transform) && b.propertyName.Contains("Pos")).ToList();

                foreach (var binding in newBindings)
                {
                    AnimationUtility.SetEditorCurve(currentClip, binding, new AnimationCurve());
                }

                bindings = newBindings.ToArray();
            }

            foreach (var curveBinding in bindings)
            {
                if (curveBinding.type != typeof(Transform))
                {
                    Debug.Log($"Ignore Type {curveBinding.type}");
                    continue;
                }

                var curve = AnimationUtility.GetEditorCurve(currentClip, curveBinding);

                if (AnimationUtility.GetFloatValue(selectedGameObject, curveBinding, out var value))
                {
                    curve.AddKey(time, value);
                }

                AnimationUtility.SetEditorCurve(currentClip, curveBinding, curve);
            }
        }

        [MenuItem("Tools/Animation/Update Pos KeyFrame")]
        private static void UpdateKeyFrame()
        {
            if (!Init(out var selectedGameObject, out var animationWindow, out var currentClip))
            {
                return;
            }

            var time = animationWindow.time;
            var bindings = AnimationUtility.GetCurveBindings(currentClip);
            foreach (var curveBinding in bindings)
            {
                if (curveBinding.type != typeof(Transform))
                {
                    Debug.Log($"Ignore Type {curveBinding.type}");
                    continue;
                }

                var curve = AnimationUtility.GetEditorCurve(currentClip, curveBinding);

                int closestIdx = -1;
                float closestTime = float.MaxValue;
                for (var index = 0; index < curve.keys.Length; index++)
                {
                    var keyframe = curve.keys[index];
                    var timeDiff = math.abs(keyframe.time - time);
                    if (timeDiff < closestTime)
                    {
                        closestIdx = index;
                        closestTime = timeDiff;
                    }
                }

                if (closestIdx < 0)
                {
                    Debug.LogWarning("No keyframe found");
                    continue;
                }

                if (AnimationUtility.GetFloatValue(selectedGameObject, curveBinding, out var value))
                {
                    curve.MoveKey(closestIdx, new Keyframe(curve.keys[closestIdx].time, value));
                }

                AnimationUtility.SetEditorCurve(currentClip, curveBinding, curve);
            }
        }

        private static bool Init(out GameObject selectedGameObject, out AnimationWindow animationWindow,
            out AnimationClip currentClip)
        {
            animationWindow = null;
            currentClip = null;

            selectedGameObject = Selection.gameObjects.FirstOrDefault();
            if (selectedGameObject == null)
            {
                Debug.LogWarning("No GameObject selected");
                return false;
            }

            var animationWindowType = System.Type.GetType("UnityEditor.AnimationWindow,UnityEditor");
            animationWindow = EditorWindow.GetWindow(animationWindowType) as AnimationWindow;

            if (animationWindow == null)
            {
                Debug.LogWarning("No Animation Window found");
                return false;
            }

            currentClip = animationWindow.animationClip;
            if (currentClip == null)
            {
                Debug.LogWarning("No active animation clip");
                return false;
            }

            return true;
        }
    }
}