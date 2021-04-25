using System;
using System.Collections.Generic;

namespace Chino
{
    public interface IExposureNotificationHandler
    {
        AbsExposureNotificationClient GetEnClient();

        public void TemporaryExposureKeyReleased(IList<ITemporaryExposureKey> temporaryExposureKeys)
        {
            throw new NotImplementedException();
        }

        public void ExposureDetected(IList<IDailySummary> dailySummaries, IList<IExposureWindow> exposureWindows)
        {
            throw new NotImplementedException();
        }

        public void ExposureDetected(IExposureSummary exposureSummary, IList<IExposureInformation> exposureInformations)
        {
            throw new NotImplementedException();
        }

        public void ExposureNotDetected();
    }
}
