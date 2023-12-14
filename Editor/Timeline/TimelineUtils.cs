using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;

public class TimelineUtils : MonoBehaviour
{
    [MenuItem("Tools/Timeline/Toggle Timeline Lock")]
    private static void LockTimeline()
    {
        var timelineWindow = TimelineEditor.GetWindow();
        if (timelineWindow != null && timelineWindow.navigator.GetRootContext().director != null)
        {
            timelineWindow.locked = !timelineWindow.locked;
            // EditorUtility.SetDirty(timelineWindow); //TODO how to redraw window if not focused?
        }
    }
    
    [MenuItem("Tools/Timeline/Toggle Timeline Preview")]
    private static void ToggleTimelinePreview()
    {
        var timelineWindow = TimelineEditor.GetWindow();
        if (timelineWindow != null)
        {
             var state = TimelineWindow.instance.state; //internal && copied from TimelineWindow::PreviewModeButtonGUI
            var enabled = !state.previewMode;
            {
                // turn off auto play as well, so it doesn't auto reenable
                if (!enabled)
                {
                    state.SetPlaying(false);
                    state.recording = false;
                }

                state.previewMode = enabled;
                // if we are successfully enabled, rebuild the graph so initial states work correctly
                // Note: testing both values because previewMode setter can "fail"
                if (enabled && state.previewMode)
                    state.rebuildGraph = true;
            }
        }
    }
}
