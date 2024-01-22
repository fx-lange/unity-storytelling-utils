using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StorytellingUtils.PauseAndJump
{
    [RequireComponent(typeof(PlayableDirector))]
    public class PauseReceiver : MonoBehaviour, INotificationReceiver
    {
        [SerializeField] private PlayableDirector _masterDirector;
        [SerializeField] private UnityEvent<int,string> _timelinePaused;
        
        private PlayableDirector _director;
        private bool _ignoreJump = false;
        private List<IMarker> _markers = new();

        public void IgnoreJump(bool ignore)
        {
            _ignoreJump = ignore;
        }
        
        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
        }

        private void Start()
        {
            LoadMarkers();
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            var pauseMarker = notification as PauseMarker;
            if (pauseMarker == null)
            {
                return;
            }

            var justHappened = Math.Abs(pauseMarker.time - _director.time) < 0.05f;
            if (!_ignoreJump || justHappened)
            {
                _masterDirector.Pause();
                _timelinePaused.Invoke(_markers.IndexOf(pauseMarker),pauseMarker.name);
            }
        }
        
        private void LoadMarkers()
        {
            var timelineAsset = _director.playableAsset as TimelineAsset;
            var allMarkers = timelineAsset.markerTrack.GetMarkers();
            _markers.Clear();
            
            foreach (var marker in allMarkers)
            {
                if (marker is PauseMarker)
                {
                    _markers.Add(marker);
                }
            }

            _markers = _markers.OrderBy(m => m.time).ToList();
        }
    }
}