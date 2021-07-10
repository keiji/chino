using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chino
{
    /// <summary>
    /// Client of Exposure Notification API.
    /// </summary>
    public abstract class AbsExposureNotificationClient
    {

#nullable enable
        /// <summary>
        /// An interface that receive events from Exposure Notification API.
        /// </summary>
        public static IExposureNotificationHandler? Handler { get; set; }

        /// <summary>
        /// The container class that contains configurations for each platform and version.
        /// </summary>
        public ExposureConfiguration? ExposureConfiguration { get; set; }
#nullable disable

        /// <summary>
        /// Starts Exposure Notification.
        /// Shows a dialog to the user asking for authorization to start Exposure Notification.
        /// </summary>
        public abstract Task StartAsync();

        /// <summary>
        /// Stops Exposure Notification.
        /// </summary>
        public abstract Task StopAsync();

        /// <summary>
        /// Returns the enabled status for Exposure Notification.
        /// </summary>
        /// <returns>True, if Exposure Notification API is enabled.</returns>
        public abstract Task<bool> IsEnabledAsync();

        /// <summary>
        /// Returns the version for Exposure Notification.
        /// </summary>
        /// <returns></returns>
        public abstract Task<long> GetVersionAsync();

        /// <summary>
        /// Gets the list of current Exposure Notification status.
        /// </summary>
        /// <returns></returns>
        public abstract Task<IList<ExposureNotificationStatus>> GetStatusesAsync();

        /// <summary>
        /// Gets TemporaryExposureKey history to be stored on the server.
        ///
        /// This should only be done after proper verification is performed on the client side that the user is diagnosed positive.
        /// Each key returned will have an unknown transmission risk level, clients should choose an appropriate risk level for these keys before uploading them to the server.
        /// </summary>
        /// <returns></returns>
        public abstract Task<List<ITemporaryExposureKey>> GetTemporaryExposureKeyHistoryAsync();

        /// <summary>
        /// Provides diagnosis key files for exposure checking. The files are to be synced from the server.
        /// Old diagnosis keys (for example older than 14 days), will be ignored.
        /// </summary>
        /// <param name="keyFiles"></param>
        public abstract Task ProvideDiagnosisKeysAsync(List<string> keyFiles);

        /// <summary>
        /// Provides diagnosis key files for exposure checking. The files are to be synced from the server.
        /// Old diagnosis keys (for example older than 14 days), will be ignored.
        /// </summary>
        /// <param name="keyFiles"></param>
        /// <param name="configuration"></param>
        public abstract Task ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration);

        /// <summary>
        /// Provides diagnosis key files for exposure checking. The files are to be synced from the server.
        /// Old diagnosis keys (for example older than 14 days), will be ignored.
        ///
        /// This method is deprecated.
        /// </summary>
        /// <param name="keyFiles"></param>
        /// <param name="configuration"></param>
        /// <param name="token"></param>
        public abstract Task ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration, string token);

        /// <summary>
        /// Shows a dialog to the user asking for authorization to get TemporaryExposureKeys in the background.
        ///
        /// If approved, the client application will be able to call requestPreAuthorizedTemporaryExposureKeyRelease() one time in the next 5 days to get a list of TemporaryExposureKeys for a user which has tested positive.
        /// </summary>
        public abstract Task RequestPreAuthorizedTemporaryExposureKeyHistoryAsync();

        /// <summary>
        /// If consent has previously been requested and granted by the user using RequestPreAuthorizedTemporaryExposureKeyHistory(),
        /// then this method will cause keys to be released to the client application after the screen is unlocked by the user.
        /// </summary>
        public abstract Task RequestPreAuthorizedTemporaryExposureKeyReleaseAsync();
    }
}
