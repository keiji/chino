using System.Collections.Generic;

namespace Chino
{
    public interface IExposureNotificationHandler
    {
        AbsExposureNotificationClient GetEnClient();

        void ExposureDetected(List<IExposureWindow> exposureWindows);

        void ExposureDetected(IExposureSummary exposureSummary, List<IExposureInformation> exposureInformations);

        void ExposureNotDetected();
    }
}
