using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chino
{
    /// <summary>
    /// An interface that receive events from Exposure Notification API.
    /// </summary>
    public interface IExposureNotificationHandler
    {
        AbsExposureNotificationClient GetEnClient();

        /// <summary>
        /// Get(provide) ExposureConfiguration to Cappuccino.
        /// </summary>
        public Task<ExposureConfiguration> GetExposureConfigurationAsync();

        /// <summary>
        /// An event that TEKs are released by the Preauthorized API.
        /// </summary>
        /// <param name="temporaryExposureKeys">List of Temporary Exposure Keys</param>
        public Task TemporaryExposureKeyReleasedAsync(IList<TemporaryExposureKey> temporaryExposureKeys)
        {
            // do nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// An event of DiagnosisKeysDataMappingConfiguration(or InfectiousnessForDaysSinceOnsetOfSymptoms) is applied.
        /// </summary>
        public Task DiagnosisKeysDataMappingAppliedAsync()
        {
            // do nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// A pre-event of Exposure is detected, before retrieving Exposure infomations.
        /// </summary>
        /// <param name="exposureConfiguration">Exposure configuration parameters that can be provided when detecting exposure.</param>
        public Task PreExposureDetectedAsync(ExposureConfiguration exposureConfiguration)
        {
            // do nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// An event of Exposure is detected for EN API v2.
        /// </summary>
        /// <param name="dailySummaries">Daily exposure summary to pass to client side.</param>
        /// <param name="exposureWindows">List of duration of up to 30 minutes during which beacons from a TEK were observed.</param>
        /// <param name="exposureConfiguration">Exposure configuration parameters that can be provided when detecting exposure.</param>
        public Task ExposureDetectedAsync(
            IList<DailySummary> dailySummaries,
            IList<ExposureWindow> exposureWindows,
            ExposureConfiguration exposureConfiguration
            )
        {
            // do nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// An event of Exposure is detected for EN API v2.
        /// </summary>
        /// <param name="exposureSummary">Summary information about recent exposures.</param>
        /// <param name="dailySummaries">Daily exposure summary to pass to client side.</param>
        /// <param name="exposureWindows">List of duration of up to 30 minutes during which beacons from a TEK were observed.</param>
        /// <param name="exposureConfiguration">Exposure configuration parameters that can be provided when detecting exposure.</param>
        public Task ExposureDetectedAsync(
            ExposureSummary exposureSummary,
            IList<DailySummary> dailySummaries,
            IList<ExposureWindow> exposureWindows,
            ExposureConfiguration exposureConfiguration
            )
        {
            // do nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// An event of Exposure is detected for EN API v1.
        /// </summary>
        /// <param name="exposureSummary">Summary information about recent exposures.</param>
        /// <param name="exposureInformations">List of information about an exposure.</param>
        /// <param name="exposureConfiguration">Exposure configuration parameters that can be provided when detecting exposure.</param>
        public Task ExposureDetectedAsync(
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformations,
            ExposureConfiguration exposureConfiguration
            )
        {
            // do nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// An event of Exposure is not detected.
        /// </summary>
        /// <param name="exposureConfiguration">Exposure configuration parameters that can be provided when detecting exposure.</param>
        public Task ExposureNotDetectedAsync(ExposureConfiguration exposureConfiguration)
        {
            // do nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// An event of Exception is occurred.
        /// </summary>
        /// <param name="exception">Exception of Exposure Notifications API</param>
        public Task ExceptionOccurredAsync(ENException exception)
        {
            // do nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// An event of Exception is occurred.
        /// </summary>
        /// <param name="exception">General exception</param>
        public Task ExceptionOccurredAsync(Exception exception)
        {
            // do nothing
            return Task.CompletedTask;
        }
    }
}
