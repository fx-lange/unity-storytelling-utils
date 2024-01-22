using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StorytellingUtils.PauseAndJump
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TimelineJump : MonoBehaviour, INotificationReceiver
    {
        [SerializeField] private List<PauseReceiver> _pauseReceivers; 
        [SerializeField] private float jumpBackLeeway = 1f;

        public event Action<int, string> OnArrived = delegate { };

        private List<IMarker> _markers = new List<IMarker>();
        private PlayableDirector _director;

        private PlayableDirector Director
        {
            get
            {
                if (_director == null)
                {
                    _director = GetComponent<PlayableDirector>();
                }

                return _director;
            }
        }

        private void Start()
        {
            LoadMarkers();
        }

        [ContextMenu("Jump Next")]
        public void JumpNext()
        {
            if (_markers.Count == 0)
            {
                LoadMarkers();
            }
            
            var currTime = Director.time;
            
            foreach (var marker in _markers)
            {
                if (marker.time < currTime)
                {
                    continue;
                }

                foreach (var pauseReceiver in _pauseReceivers)
                {
                    pauseReceiver.IgnoreJump(true);
                }

                StartCoroutine(AfterJump());
                Director.time = marker.time;
                Director.Resume();
                return;
            }
        }

        public void JumpTo(int index)
        {
            if (_markers.Count == 0)
            {
                LoadMarkers();
            }

            if (_markers.Count <= index)
            {
                Debug.LogError($"JumpMarker {index} missing");
                return;
            }

            foreach (var pauseReceiver in _pauseReceivers)
            {
                pauseReceiver.IgnoreJump(true);
            }

            StartCoroutine(AfterJump());
            Director.time = _markers[index].time;
            Director.Resume();
        }

        private IEnumerator AfterJump()
        {
            yield return new WaitForSeconds(1);
            foreach (var pauseReceiver in _pauseReceivers)
            {
                pauseReceiver.IgnoreJump(false);
            }
        }
        
        [ContextMenu("Jump Back")]
        public void JumpBack()
        {
            if (_markers.Count == 0)
            {
                LoadMarkers();
            }
            
            var currTime = Director.time;
            double lastMarkerTime = -1;
            var prevExists = false;
            
            foreach (var marker in _markers)
            {
                if (marker.time + jumpBackLeeway < currTime )
                {
                    lastMarkerTime = marker.time;
                    prevExists = true;
                    continue;
                }

                break;
            }

            if (prevExists)
            {
                Director.time = lastMarkerTime;
                Director.Resume();
            }
            else
            {
                Director.time = 0;
                Director.Play();
            }
        }
        
        private void LoadMarkers()
        {
            var timelineAsset = Director.playableAsset as TimelineAsset;

            var allMarkers = timelineAsset.markerTrack.GetMarkers();
            _markers.Clear();
            
            foreach (var marker in allMarkers)
            {
                if (marker is JumpDestinationMarker)
                {
                    _markers.Add(marker);
                }
            }

            _markers = _markers.OrderBy(m => m.time).ToList();
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            var marker = notification as JumpDestinationMarker;
            if (marker == null)
            {
                return;
            }

            OnArrived(_markers.IndexOf(marker), marker.name);
        }
    }
}