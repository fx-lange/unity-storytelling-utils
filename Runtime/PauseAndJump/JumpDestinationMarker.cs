using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StorytellingUtils.PauseAndJump
{
    [CustomStyle("JumpDestinationMarker")]
    public class JumpDestinationMarker : UnityEngine.Timeline.Marker, INotification
    {
        public PropertyName id { get; } 
    }
}