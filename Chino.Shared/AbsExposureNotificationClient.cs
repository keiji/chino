using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chino
{

    public abstract class AbsExposureNotificationClient
    {

#nullable enable
        public static ExposureNotificationHandler? Handler { get; set; }
#nullable disable

        public abstract Task Start();

        public abstract Task Stop();

        public abstract Task<bool> IsEnabledAsync();

        public abstract Task<long> GetVersion();

        public abstract Task<IExposureNotificationStatus> GetStatus();

        public abstract Task<List<TemporaryExposureKey>> GetTemporaryExposureKeyHistory();

        public abstract Task ProvideDiagnosisKeys(List<string> keyFiles);

        public abstract Task ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration);

        public abstract Task ProvideDiagnosisKeys(List<string> keyFiles, ExposureConfiguration configuration, string token);

        public abstract Task<List<ExposureWindow>> GetExposureWindowsAsync();

        //public abstract Task RequestPreAuthorizedTemporaryExposureKeyHistory();

        //public abstract Task RequestPreAuthorizedTemporaryExposureKeyRelease();
    }
}
