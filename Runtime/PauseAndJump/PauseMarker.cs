using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StorytellingUtils.PauseAndJump
{
    [CustomStyle("PauseMarker")]
    public class PauseMarker : UnityEngine.Timeline.Marker, INotification
    {
        public PropertyName id { get; }
    }
}