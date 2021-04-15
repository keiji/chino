using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chino
{
    public class ExposureNotificationClient: AbsExposureNotificationClient
    {

        public override Task Start()
        {
            throw new NotImplementedException();
        }

        public override Task Stop()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> IsEnabledAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<long> GetVersion()
        {
            throw new NotImplementedException();
        }

        public override Task<List<ExposureWindow>> GetExposureWindowsAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<List<TemporaryExposureKey>> GetTemporaryExposureKeyHistory()
        {
            throw new NotImplementedException();
        }

        public override Task ProvideDiagnosisKeys(List<string> keyFiles)
        {
            throw new NotImplementedException();
        }

        public override Task ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public override Task ProvideDiagnosisKeys(List<string> keyFiles, ExposureConfiguration configuration, string token)
        {
            throw new NotImplementedException();
        }

        //public override Task RequestPreAuthorizedTemporaryExposureKeyHistory()
        //{
        //    throw new NotImplementedException();
        //}

        //public override Task RequestPreAuthorizedTemporaryExposureKeyRelease()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
