using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Daze {
    /// <summary>
    /// A Wav is exactly what you would expect... a Wav.
    /// You can create it from resources using Engine.loadWavFromResources
    /// </summary>
    public class Wav {
        private MediaPlayer player;
        private bool _loop;
        private bool _disposeAtEnd;
        /// <summary>
        /// Set to true to make the Wav remove from the Engine managed Wavs at the end of the sound
        /// </summary>
        public bool disposeAtEnd {
            get => _disposeAtEnd;
            set {
                _disposeAtEnd = value;
                if(_disposeAtEnd) {
                    loop = false;
                }
            }
        }
        /// <summary>
        /// The volume of this Wav, it goes from 0 to 100
        /// </summary>
        public int volume {
            get => (int)(player.Volume*100);
            set {
                player.Volume = value < 0 ? 0 : (value > 100 ? 1 : value / 100.0);
            }
        }

        /// <summary>
        /// Set to true to make the Wav restart after it finished playing.
        /// </summary>
        public bool loop {
            get => _loop;
            set {
                _loop = value;
                if(value) {
                    player.MediaEnded += restartMediaPlayer;
                    if(disposeAtEnd) player.MediaEnded -= disposeMediaPlayer;
                } else {
                    player.MediaEnded -= restartMediaPlayer;
                    if(disposeAtEnd) player.MediaEnded += disposeMediaPlayer;
                }
            }
        }

        internal Wav(string filePath, int volume, bool loop, bool disposeAtEnd = false) {
            player = new MediaPlayer();
            player.Open(new Uri(filePath));
            this.volume = volume;
            this.loop = loop;
            this.disposeAtEnd = disposeAtEnd;
        }

        /// <summary>
        /// You don't really need this comment, right?
        /// (It start the sound)
        /// </summary>
        public void Play() {
            player.Play();
        }

        /// <summary>
        /// You don't really need this comment, do you?
        /// (It pause the sound)
        /// </summary>
        public void Pause() {
            player.Pause();
        }

        /// <summary>
        /// You don't really need this comment, do you?
        /// (It stop the sound)
        /// </summary>
        public void Stop() {
            player.Stop();
        }

        private static void restartMediaPlayer(object sender, EventArgs e) {
            MediaPlayer player = ((MediaPlayer)sender);
            player.Position = TimeSpan.Zero;
            player.Play();
        }
        

        private void disposeMediaPlayer(object sender, EventArgs e) {
            Engine.playingWavs.Remove(this);
        }

    }
}
