using Hangfire;

namespace PortListener.Jobs
{
    public static class BackgroundRecurringJobManager
    {
        public static void EnqueuePingJob()
        {
            RecurringJob.RemoveIfExists(nameof(PingIpsBackgroundJobService));
            RecurringJob.AddOrUpdate<PingIpsBackgroundJobService>(
                nameof(PingIpsBackgroundJobService),
                job => job.Execute(),
                "*/2 * * * *",
                TimeZoneInfo.Local);
        }
    }
}
