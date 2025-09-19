using NcnnDotNet;
using NcnnDotNet.OpenCV;
using net_maui_app_v24.Interfaces;
using net_maui_app_v24.Models;
using UltraFaceDotNet;

namespace net_maui_app_v24.Services
{
    public class DetectService : IDetectService
    {

        private readonly UltraFace _UltraFace;
        
        public DetectService()
        {
            try
            {
                Byte[] RFB_320_bin = net_maui_app_v24.Properties.Resources.RFB_320_bin;
                Byte[] RFB_320_param = net_maui_app_v24.Properties.Resources.RFB_320_param;

                var file_name = new[] { "RFB-320.bin", "RFB-320.param" };

                foreach (var file in file_name)
                {
                    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), file);
                    using (var fs = File.Create(path))
                    if (file == "RFB-320.bin")
                    {
                        using (var stream = new MemoryStream(RFB_320_bin))
                        {
                            if (stream != null)
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                stream.CopyTo(fs);
                                stream.Seek(fs.Length, SeekOrigin.End);
                            }
                        }
                    } 
                    else if (file == "RFB-320.param")
                    {
                        using (var stream = new MemoryStream(RFB_320_param))
                        {
                            if (stream != null)
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                stream.CopyTo(fs);
                                stream.Seek(fs.Length, SeekOrigin.End);
                            }
                        }
                    }
                }

                var binPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), file_name[0]);
                var paramPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), file_name[1]);

                var param = new UltraFaceParameter
                {
                    BinFilePath = binPath,
                    ParamFilePath = paramPath,
                    InputWidth = 320,
                    InputLength = 240,
                    NumThread = 1,
                    ScoreThreshold = 0.7f
                };

                this._UltraFace = UltraFace.Create(param);
            }
            catch (Exception ex)
            {
                throw new FileLoadException($"Exceção: {ex.Message}");
            }
        }

        public DetectResult Detect(byte[] file)
        {
            byte[] faces1 = net_maui_app_v24.Properties.Resources.faces1;
            string file_faces1 = "faces1.jpg";
            string path_faces1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), file_faces1);
            using (FileStream fs = File.Create(path_faces1))
            using (MemoryStream stream = new MemoryStream(faces1))
            {
                if (stream != null)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fs);
                }
            }

            using NcnnDotNet.OpenCV.Mat frame1 = Cv2.ImRead(path_faces1, CvLoadImage.Grayscale);
            using NcnnDotNet.OpenCV.Mat frame = Cv2.ImDecode(file, CvLoadImage.Grayscale);
            if (frame.IsEmpty)
                throw new NotSupportedException("This file is not supported!!");

            if (Ncnn.IsSupportVulkan)
                Ncnn.CreateGpuInstance();

            using NcnnDotNet.Mat inMat = NcnnDotNet.Mat.FromPixels(frame.Data, PixelType.Bgr2Rgb, frame.Cols, frame.Rows);

            //var faceInfos = this._UltraFace.Detect(inMat).ToArray();

            if (Ncnn.IsSupportVulkan)
                Ncnn.DestroyGpuInstance();

            //return new DetectResult(frame.Cols, frame.Rows, faceInfos);
            return new DetectResult(frame.Cols, frame.Rows, null);
        }
    }
}
