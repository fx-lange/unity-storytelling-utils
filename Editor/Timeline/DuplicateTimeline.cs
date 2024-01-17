using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace StoryTellingUtils.Editor
{
    public static class DuplicateTimeline
    {

        //1. Add this script to your Assets/Editor/
        //2. Select in Hierarchy Window your Timeline object (GameObject containing Playable Director component)
        //3. Use new menu item : Timeline > Duplicate With Bindings

        [MenuItem("Tools/Timeline/Duplicate With Bindings", true)]
        public static bool DuplicateWithBindingsValidate()
        {      
            if (Selection.activeGameObject == null)
                return false;

            var playableDirector = Selection.activeGameObject.GetComponent<PlayableDirector>();
            if (playableDirector == null)
                return false;

            var playableAsset = playableDirector.playableAsset;
            if (playableAsset == null)
                return false;

            var path = AssetDatabase.GetAssetPath(playableAsset);
            if (string.IsNullOrEmpty(path))
                return false;

            return true;
        }

        [MenuItem("Tools/Timeline/Duplicate With Bindings")]
        public static void DuplicateWithBindings()
        {
            if (Selection.activeGameObject == null)
                return;

            var playableDirector = Selection.activeGameObject.GetComponent<PlayableDirector>();
            if (playableDirector == null)
                return;

            var playableAsset = playableDirector.playableAsset;
            if (playableAsset == null)
                return;

            Debug.Log(playableAsset);
            var path = AssetDatabase.GetAssetPath(playableAsset);
            if (string.IsNullOrEmpty(path))
                return;

            string newPath = path.Replace(".playable", " (Clone).playable");
            if (!AssetDatabase.CopyAsset(path, newPath))
            {
                Debug.LogError("Couldn't Clone Asset");
                return;
            }

            var newPlayableAsset = AssetDatabase.LoadMainAssetAtPath(newPath) as PlayableAsset;
            var gameObject = GameObject.Instantiate(Selection.activeGameObject);
            var newPlayableDirector = gameObject.GetComponent<PlayableDirector>();
            newPlayableDirector.playableAsset = newPlayableAsset;

            var oldBindings = playableAsset.outputs.ToArray();
            var newBindings = newPlayableAsset.outputs.ToArray();

            for (int i = 0; i < oldBindings.Length; i++)
            {
                newPlayableDirector.SetGenericBinding(newBindings[i].sourceObject,
                    playableDirector.GetGenericBinding(oldBindings[i].sourceObject)
                );
            }

            //remove old bindings
            for (int i = 0; i < oldBindings.Length; i++)
            {
                newPlayableDirector.ClearGenericBinding(oldBindings[i].sourceObject);
               
            }

       
        }
    }
}
