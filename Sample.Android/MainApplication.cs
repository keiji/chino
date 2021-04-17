using System;
using System.Collections.Generic;
using Android.App;
using Android.Runtime;
using Chino;

namespace Sample.Android
{

#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class MainApplication : Application, IExposureNotificationHandler
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        private ExposureNotificationClient EnClient = null;

        public override void OnCreate()
        {
        }
        public ExposureNotificationClient GetEnClient()
        {
            if (EnClient == null)
            {
                EnClient = new ExposureNotificationClient();
                EnClient.Init(this);
            }

            return EnClient;
        }

        public void ExposureDetected(List<IExposureWindow> exposureWindows)
        {
            throw new NotImplementedException();
        }

        public void ExposureDetected(List<ExposureInformation> exposureInformations)
        {
            throw new NotImplementedException();
        }

        public void ExposureNotDetected()
        {
            throw new NotImplementedException();
        }
    }
}
