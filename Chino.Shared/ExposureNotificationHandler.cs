using System;
using System.Collections.Generic;

namespace Chino
{
    public interface ExposureNotificationHandler
    {
        ExposureNotificationClient GetEnClient();

        void ExposureDetected(List<ExposureWindow> exposureWindows);

        void ExposureDetected(List<ExposureInformation> exposureInformations);

        void ExposureNotDetected();
    }
}
