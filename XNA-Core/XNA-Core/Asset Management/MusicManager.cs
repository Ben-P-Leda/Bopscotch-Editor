using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Leda.Core.Asset_Management
{
    public sealed class MusicManager
    {
        private static MusicManager _instance = null;
        private static MusicManager Instance { get { if (_instance == null) { _instance = new MusicManager(); } return _instance; } }

        public static bool Initialized { get { return Instance.InstanceInitialized; } }
        public static bool Available { get { return Instance.CheckMediaPlayerIsAvailable(); } }
        public static bool Muted { get { return Instance.InstanceMuted; } set { Instance.InstanceSetMutedState(value); } }
        public static void Initialize() { Instance.AttemptInitialization(); }
        public static void AddTune(string tuneName, Song tuneToAdd) { Instance.InstanceAddTune(tuneName, tuneToAdd); }
        public static void PlayMusic(string tuneName) { Instance.InstancePlayTune(tuneName, false); }
        public static void PlayLoopedMusic(string tuneName) { Instance.InstancePlayTune(tuneName, true); }
        public static void StopMusic() { Instance.InstanceStopTune(); }

        private Dictionary<string, Song> _tunes = null;
        private string _lastSongName;
        private bool _lastSongWasLooped;
        private bool InstanceInitialized { get; set; }
        private bool InstanceMuted { get; set; }

        private MusicManager()
        {
            _tunes = new Dictionary<string, Song>();
            InstanceInitialized = false;
            InstanceMuted = false;
        }

        private void AttemptInitialization()
        {
            try { MediaPlayer.IsMuted = false; InstanceInitialized = true; }
            catch { InstanceInitialized = false; }
        }

        private bool CheckMediaPlayerIsAvailable()
        {
            if (InstanceInitialized) { return MediaPlayer.GameHasControl; }

            return false;
        }

        private void InstanceAddTune(string tuneName, Song tuneToAdd)
        {
            if (!_tunes.ContainsKey(tuneName)) { _tunes.Add(tuneName, tuneToAdd); }
            else { _tunes[tuneName] = tuneToAdd; }
        }

        private void InstancePlayTune(string tuneName, bool loop)
        {
            if ((InstanceInitialized) && (MediaPlayer.GameHasControl))
            {
                if (!InstanceMuted)
                {
                    MediaPlayer.IsRepeating = loop;
                    MediaPlayer.Play(_tunes[tuneName]);
                }

                _lastSongName = tuneName;
                _lastSongWasLooped = loop;
            }
        }

        private void InstanceStopTune()
        {
            if ((InstanceInitialized) && (MediaPlayer.GameHasControl))
            {
                _lastSongName = "";
                _lastSongWasLooped = false;
                MediaPlayer.Stop();
            }
        }

        private void InstanceSetMutedState(bool mute)
        {
            InstanceMuted = mute;

            if ((InstanceInitialized) && (MediaPlayer.GameHasControl))
            {
                if (mute)
                {
                    MediaPlayer.IsMuted = true;
                    MediaPlayer.Stop();
                }
                else
                {
                    MediaPlayer.IsMuted = false;
                    if ((MediaPlayer.State == MediaState.Stopped) && (!string.IsNullOrEmpty(_lastSongName)))
                    {
                        InstancePlayTune(_lastSongName, _lastSongWasLooped);
                    }
                }
            }
        }
    }
}
