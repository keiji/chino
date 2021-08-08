using Android.App.Job;

namespace Chino.Android.Google
{
    public class JobSetting
    {
        public readonly long InitialBackoffTimeMillis;
        public readonly BackoffPolicy BackoffPolicy;
        public readonly bool Persisted;

        public JobSetting(long initialBackoffTimeMillis, BackoffPolicy backoffPolicy, bool persisted)
        {
            InitialBackoffTimeMillis = initialBackoffTimeMillis;
            BackoffPolicy = backoffPolicy;
            Persisted = persisted;
        }
    }
}
