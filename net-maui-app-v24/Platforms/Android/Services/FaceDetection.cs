
namespace net_maui_app_v24.Platforms.Android.Services
{
    public class FaceDetection
    {
        /*
        private CameraSourcePreview mPreview;
        private GraphicOverlay mGraphicOverlay;

        public FaceDetection(CameraSourcePreview mPreview, GraphicOverlay mGraphicOverlay)
        {
            this.mPreview = mPreview;
            this.mGraphicOverlay = mGraphicOverlay;
        }

        public void CreateFaceDetect()
        {
            try
            {
                var context = Platform.AppContext;

                FaceDetector detector = new FaceDetector.Builder(context)
                    .SetClassificationType(ClassificationType.All)
                    .Build();

                if (!detector.IsOperational)
                    return;

                mCameraSource = new CameraSource.Builder(context, detector)
                            .SetRequestedPreviewSize(640, 480)
                            .SetFacing(CameraFacing.Front)
                            .SetRequestedFps(30.0f)
                            .Build();

            }
            catch (Exception ex)
            {
                throw new FileLoadException($"Exceção: {ex.Message}");
            }
        }

        public void StartFaceDetect()
        {
            try
            {
                int code = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(
                    Platform.AppContext);

                if (code != ConnectionResult.Success)
                    return;

                if (mCameraSource != null)
                {
                    try
                    {
                        mPreview.Start(mCameraSource, mGraphicOverlay);
                    }
                    catch (System.Exception e)
                    {
                        mCameraSource.Release();
                        mCameraSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FileLoadException($"Exceção: {ex.Message}");
            }
        }
        */
        /*
        public Tracker Create(Java.Lang.Object item)
        {
            return new GraphicFaceTracker(mGraphicOverlay, mCameraSource);
        }
        */
    }
}