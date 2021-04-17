using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExposureNotifications;

namespace Chino
{
    public class ExposureNotificationClient : AbsExposureNotificationClient
    {

        public static ExposureNotificationClient Shared = new ExposureNotificationClient();


        private ENManager EnManager = new ENManager();

        public Task Init()
        {
            return Task.Run(async () =>
            {
                await EnManager.ActivateAsync();
            });
        }

        ~ExposureNotificationClient()
        {
            EnManager.Invalidate();
        }

        public async override Task Start()
        {
            if (EnManager == null)
            {
                return;
            }

            await EnManager.SetExposureNotificationEnabledAsync(true);
        }

        public async override Task Stop()
        {
            if (EnManager == null)
            {
                return;
            }

            await EnManager.SetExposureNotificationEnabledAsync(false);
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
