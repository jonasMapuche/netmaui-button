using Android.Media;
using Java.IO;
using net_maui_app_v24.Interfaces;

namespace net_maui_app_v24.Platforms.Android.Services
{
    public class RecordAudio : IRecordAudio
    {
        private MediaRecorder mediaRecorder;
        private string storagePath;
        private bool isRecordStarted = false;

        private AudioRecord? audioRecord;
        private int bufferSize;
        private ChannelIn channelIn = ChannelIn.Mono;
        private Encoding encoding = Encoding.Pcm16bit;
        private int sampleRate = 44100;
        private int bitDepth = 16;
        const int WAVHEADERLENGTH = 44;
        private int channelMono = 1;

        public void StartRecordWav()
        {
            if (this.mediaRecorder == null)
            {
                SetAudioFilePath("wav");
                this.bufferSize = AudioRecord.GetMinBufferSize(this.sampleRate, this.channelIn, this.encoding);
                this.audioRecord = new AudioRecord(AudioSource.Mic, this.sampleRate, this.channelIn, this.encoding, this.bufferSize);
                this.audioRecord.StartRecording();
                Task.Run(WriteAudioDataToFile);
            }
        }

        public void StartRecord()
        {
            if (this.mediaRecorder == null)
            {
                SetAudioFilePath("mp3");
                this.mediaRecorder = new MediaRecorder();
                this.mediaRecorder.Reset();
                this.mediaRecorder.SetAudioSource(AudioSource.Mic);
                this.mediaRecorder.SetOutputFormat(OutputFormat.AacAdts);
                this.mediaRecorder.SetAudioEncoder(AudioEncoder.Aac);
                this.mediaRecorder.SetOutputFile(storagePath);
                this.mediaRecorder.Prepare();
                this.mediaRecorder.Start();
            }
            else
            {
                this.mediaRecorder.Resume();
            }
            this.isRecordStarted = true;
        }
        public string StopRecord()
        {
            if (this.mediaRecorder == null)
            {
                return string.Empty;
            }
            this.mediaRecorder.Resume();
            this.mediaRecorder.Stop();
            this.mediaRecorder = null;
            this.isRecordStarted = false;
            return this.storagePath;
        }
        private void SetAudioFilePath(string extension)
        {
            string fileName = "/Record_" + DateTime.UtcNow.ToString("ddMMM_hhmmss") + (extension == "mp3"? ".mp3" : ".wav");
            var path = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            this.storagePath = path + fileName;
            Directory.CreateDirectory(path);
        }

        public string StopRecordWav()
        {
            if (this.audioRecord?.RecordingState == RecordState.Recording)
            {
                this.audioRecord?.Stop();
            }
            UpdateAudioHeaderToFile();
            return this.storagePath;
        }

        void UpdateAudioHeaderToFile()
        {
            try
            {
                RandomAccessFile randomAccessFile = new(this.storagePath, "rw");

                var totalAudioLength = randomAccessFile.Length();
                var totalDataLength = totalAudioLength + 36;

                var header = GetWaveFileHeader(totalAudioLength, totalDataLength, this.sampleRate, this.channelMono, this.bitDepth);

                randomAccessFile.Seek(0);
                randomAccessFile.Write(header, 0, WAVHEADERLENGTH);

                randomAccessFile.Close();
            }
            catch (Exception ex)
            {
                throw new FileLoadException($"Exceção: {ex.Message}");
            }
        }

        void WriteAudioDataToFile()
        {
            try
            {
                var data = new byte[this.bufferSize];
                string audioFilePath = this.storagePath;
                FileOutputStream? outputStream;

                try
                {
                    outputStream = new FileOutputStream(audioFilePath);
                }
                catch (Exception ex)
                {
                    throw new FileLoadException($"Exceção: {ex.Message}");
                }

                if (audioRecord is not null)
                {
                    var header = GetWaveFileHeader(0, 0, this.sampleRate, this.channelMono, this.bitDepth);
                    outputStream.Write(header, 0, WAVHEADERLENGTH);

                    while (this.audioRecord.RecordingState == RecordState.Recording)
                    {
                        var read = audioRecord.Read(data, 0, this.bufferSize);
                        outputStream.Write(data, 0, read);
                    }

                    outputStream.Close();
                }
            }
            catch (Exception ex)
            {
                throw new FileLoadException($"Exceção: {ex.Message}");
            }
        }

        static byte[] GetWaveFileHeader(long audioLength, long dataLength, long sampleRate, int channels, int bitDepth)
        {
            int blockAlign = (int)(channels * (bitDepth / 8));
            long byteRate = sampleRate * blockAlign;
            byte[] header = new byte[WAVHEADERLENGTH];

            header[0] = Convert.ToByte('R'); // RIFF/WAVE header
            header[1] = Convert.ToByte('I'); // (byte)'I'
            header[2] = Convert.ToByte('F');
            header[3] = Convert.ToByte('F');
            header[4] = (byte)(dataLength & 0xff);
            header[5] = (byte)((dataLength >> 8) & 0xff);
            header[6] = (byte)((dataLength >> 16) & 0xff);
            header[7] = (byte)((dataLength >> 24) & 0xff);
            header[8] = Convert.ToByte('W');
            header[9] = Convert.ToByte('A');
            header[10] = Convert.ToByte('V');
            header[11] = Convert.ToByte('E');
            header[12] = Convert.ToByte('f'); // fmt chunk
            header[13] = Convert.ToByte('m');
            header[14] = Convert.ToByte('t');
            header[15] = (byte)' ';
            header[16] = 16; // 4 bytes - size of fmt chunk
            header[17] = 0;
            header[18] = 0;
            header[19] = 0;
            header[20] = 1; // format = 1
            header[21] = 0;
            header[22] = Convert.ToByte(channels);
            header[23] = 0;
            header[24] = (byte)(sampleRate & 0xff);
            header[25] = (byte)((sampleRate >> 8) & 0xff);
            header[26] = (byte)((sampleRate >> 16) & 0xff);
            header[27] = (byte)((sampleRate >> 24) & 0xff);
            header[28] = (byte)(byteRate & 0xff);
            header[29] = (byte)((byteRate >> 8) & 0xff);
            header[30] = (byte)((byteRate >> 16) & 0xff);
            header[31] = (byte)((byteRate >> 24) & 0xff);
            header[32] = (byte)(blockAlign); // block align
            header[33] = 0;
            header[34] = Convert.ToByte(bitDepth); // bits per sample
            header[35] = 0;
            header[36] = Convert.ToByte('d');
            header[37] = Convert.ToByte('a');
            header[38] = Convert.ToByte('t');
            header[39] = Convert.ToByte('a');
            header[40] = (byte)(audioLength & 0xff);
            header[41] = (byte)((audioLength >> 8) & 0xff);
            header[42] = (byte)((audioLength >> 16) & 0xff);
            header[43] = (byte)((audioLength >> 24) & 0xff);

            return header;
        }
    }
}
