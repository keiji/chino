using Android.App.Job;

namespace Chino.Android.Google
{
    public class JobSetting
    {
        private readonly long _initialBackoffTimeMillis;
        private readonly BackoffPolicy _backoffPolicy;
        private readonly bool _persisted;

        public JobSetting(long initialBackoffTimeMillis, BackoffPolicy backoffPolicy, bool persisted)
        {
            _initialBackoffTimeMillis = initialBackoffTimeMillis;
            _backoffPolicy = backoffPolicy;
            _persisted = persisted;
        }

        internal void Apply(JobInfo.Builder builder)
        {
            builder
                .SetBackoffCriteria(_initialBackoffTimeMillis, _backoffPolicy)
                .SetPersisted(_persisted);
        }
    }
}
