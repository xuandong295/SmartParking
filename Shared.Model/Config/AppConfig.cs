using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Config
{
    public class AppConfig : IAppConfig
    {
        public string DataCollectorQueue { get; set; }
        public string PolicyEngineQueue { get; set; }
        public string PreprocessQueue { get; set; }
        public string CheckServicesQueue { get; set; }
        public string ScanResultQueue { get; set; }
        public string ScheduleQueue { get; set; }
        public string InvestigateQueue { get; set; }
        public string LoggingQueue { get; set; }
        public string ReportQueue { get; set; }
        public string LicenseQueue { get; set; }
        public string MailRequestQueue { get; set; }

        public string Prefix { get; set; }
        public string CLIRequestQueue { get; set; }

        public string S3ReportBucket { get; set; }
        public string S3UserAccessKey { get; set; }
        public string S3UserSecretKey { get; set; }


        public AppConfig()
        {
        }
        public AppConfig(string prefix, string mailRequestQueue)
        {
            MailRequestQueue = mailRequestQueue;
            Prefix = prefix;
        }

        public AppConfig(string dataCollectorQueue, string policyEngineQueue, string preprocessQueue, string requestCheckServicesQueue, string scanResultQueue, string scheduleQueue, string investigateQueue, string loggingQueue, string reportQueue, string cliRequestQueue, string prefix, string s3Bucket, string s3UserAccessKey, string s3UserSecretKey)
        {
            DataCollectorQueue = dataCollectorQueue;
            PolicyEngineQueue = policyEngineQueue;
            PreprocessQueue = preprocessQueue;
            CheckServicesQueue = requestCheckServicesQueue;
            ScanResultQueue = scanResultQueue;
            ScheduleQueue = scheduleQueue;
            InvestigateQueue = investigateQueue;
            LoggingQueue = loggingQueue;
            CLIRequestQueue = cliRequestQueue;
            Prefix = prefix;
            S3ReportBucket = s3Bucket;
            S3UserAccessKey = s3UserAccessKey;
            S3UserSecretKey = s3UserSecretKey;
            ReportQueue = reportQueue;
        }

        public AppConfig GetAppConfig()
        {
            return new AppConfig(DataCollectorQueue, PolicyEngineQueue, PreprocessQueue, CheckServicesQueue, ScanResultQueue, ScheduleQueue, InvestigateQueue, LoggingQueue, ReportQueue, CLIRequestQueue, Prefix, S3ReportBucket, S3UserAccessKey, S3UserSecretKey);
        }

        public void Dispose()
        {

        }
    }

}
