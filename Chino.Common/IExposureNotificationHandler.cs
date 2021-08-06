using System;
using System.Collections.Generic;

namespace Chino
{
    /// <summary>
    /// An interface that receive events from Exposure Notification API.
    /// </summary>
    public interface IExposureNotificationHandler
    {
        AbsExposureNotificationClient GetEnClient();

        /// <summary>
        /// An event that TEKs are released by the Preauthorized API.
        /// </summary>
        /// <param name="temporaryExposureKeys">List of Temporary Exposure Keys</param>
        public void TemporaryExposureKeyReleased(IList<TemporaryExposureKey> temporaryExposureKeys)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// An event of Exposure is detected for EN API v2.
        /// </summary>
        /// <param name="dailySummaries">Daily exposure summary to pass to client side.</param>
        /// <param name="exposureWindows">List of duration of up to 30 minutes during which beacons from a TEK were observed.</param>
        public void ExposureDetected(IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// An event of Exposure is detected for EN API v1.
        /// </summary>
        /// <param name="exposureSummary">Summary information about recent exposures.</param>
        /// <param name="exposureInformations">List of information about an exposure.</param>
        public void ExposureDetected(ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// An event of Exposure is not detected.
        /// </summary>
        public void ExposureNotDetected();
    }
}
