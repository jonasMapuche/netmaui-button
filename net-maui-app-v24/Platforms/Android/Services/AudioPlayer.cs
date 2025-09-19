using Android.Media;
using net_maui_app_v24.Interfaces;
using Stream = Android.Media.Stream;

namespace net_maui_app_v24.Platforms.Android.Services
{
    public class AudioPlayer : IAudioPlayer
    {
        private MediaPlayer mediaPlayer;
        private int currentPositionLength = 0;
        private bool isPrepared;
        private bool isCompleted;

        public void PlayAudio(string filePath)
        {
            if (this.mediaPlayer != null && !this.mediaPlayer.IsPlaying)
            {
                this.mediaPlayer.SeekTo(currentPositionLength);
                this.currentPositionLength = 0;
                this.mediaPlayer.Start();
            }
            else if (this.mediaPlayer == null || !this.mediaPlayer.IsPlaying)
            {
                try
                {
                    this.isCompleted = false;
                    this.mediaPlayer = new MediaPlayer();
                    this.mediaPlayer.SetDataSource(filePath);
                    this.mediaPlayer.SetAudioStreamType(Stream.Music);
                    this.mediaPlayer.PrepareAsync();
                    this.mediaPlayer.Prepared += (sender, args) =>
                    {
                        this.isPrepared = true;
                        this.mediaPlayer.Start();
                    };
                    this.mediaPlayer.Completion += (sender, args) =>
                    {
                        this.isCompleted = true;
                    };
                }
                catch (Exception e)
                {
                    this.mediaPlayer = null;
                }
            }
        }
        public void Stop()
        {
            if (this.mediaPlayer != null)
            {
                if (isPrepared)
                {
                    this.mediaPlayer.Stop();
                    this.mediaPlayer.Release();
                    this.isPrepared = false;
                }
                this.isCompleted = false;
                this.mediaPlayer = null;
            }
        }
    }

}
