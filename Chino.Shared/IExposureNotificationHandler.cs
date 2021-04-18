using System.Collections.Generic;

namespace Chino
{
    public interface IExposureNotificationHandler
    {
        ExposureNotificationClient GetEnClient();

        void ExposureDetected(List<IExposureWindow> exposureWindows);

        void ExposureDetected(IExposureSummary exposureSummary, List<IExposureInformation> exposureInformations);

        void ExposureNotDetected();
    }
}
