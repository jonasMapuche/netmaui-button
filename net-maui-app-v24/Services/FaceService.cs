using OpenCvSharp;
using System.Text;
using Size = OpenCvSharp.Size;

namespace net_maui_app_v24.Services
{
    public class FaceService
    {
        public async Task<int> Image1() 
        {
            try 
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

                string haarcascade_frontalface_alt = net_maui_app_v24.Properties.Resources.haarcascade_frontalface_alt;
                string file_xml = "haarcascade_frontalface_alt.xml";
                string path_xml = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), file_xml);
                using (var fs1 = File.Create(path_xml))
                {
                    using (var stream1 = new StreamWriter(fs1, Encoding.UTF8))
                    {
                        if (stream1 != null)
                        {
                            await stream1.WriteAsync(haarcascade_frontalface_alt);
                        }
                    }
                }

                int quantity = 0;
                CascadeClassifier cascade = new CascadeClassifier("haarcascade_frontalface_alt.xml");

                Scalar color = Scalar.FromRgb(0, 255, 0);
                using (Mat srcImage = new Mat(path_faces1))
                using (Mat grayImage = new Mat())
                {
                    Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGRA2GRAY);
                    Cv2.EqualizeHist(grayImage, grayImage);

                    var faces = cascade.DetectMultiScale(
                        image: grayImage,
                        minSize: new Size(60, 60));

                    quantity = faces.Length;

                    foreach (var faceRect in faces)
                    {
                        Cv2.Rectangle(srcImage, faceRect, color, 3);
                    }

                    Cv2.ImShow("Face Detection", srcImage);
                    int key = Cv2.WaitKey(0);
                }
                return quantity;
            }
            catch (Exception ex)
            {
                throw new FileLoadException($"Exceção: {ex.Message}");
            }

        }


    }
}
