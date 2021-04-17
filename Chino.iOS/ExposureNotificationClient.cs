using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExposureNotifications;

namespace Chino
{
    public class ExposureNotificationClient : AbsExposureNotificationClient
    {

        public static readonly ExposureNotificationClient Shared = new ExposureNotificationClient();


        private readonly ENManager EnManager = new ENManager();

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
            await EnManager.SetExposureNotificationEnabledAsync(true);
        }

        public async override Task Stop()
        {
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

        public override Task<IExposureNotificationStatus> GetStatus()
        {
            return Task.Run(() =>
             {
                 return (IExposureNotificationStatus)new ExposureNotificationStatus(EnManager.ExposureNotificationStatus);
             });
        }

        public override Task<List<IExposureWindow>> GetExposureWindowsAsync()
        {
            throw new NotImplementedException();
        }

        public async override Task<List<ITemporaryExposureKey>> GetTemporaryExposureKeyHistory()
        {
            ENTemporaryExposureKey[] teks = await EnManager.GetDiagnosisKeysAsync();
            return teks.Select(tek => (ITemporaryExposureKey)new TemporaryExposureKey(tek)).ToList();
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
