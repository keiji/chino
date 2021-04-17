using System;
using System.Collections.Generic;

namespace Chino
{
    public interface ExposureNotificationHandler
    {
        ExposureNotificationClient GetEnClient();

        void ExposureDetected(List<IExposureWindow> exposureWindows);

        void ExposureDetected(List<ExposureInformation> exposureInformations);

        void ExposureNotDetected();
    }
}
