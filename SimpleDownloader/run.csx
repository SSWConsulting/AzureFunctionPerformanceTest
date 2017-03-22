#r "SSW.LinkAuditor.BusinessImplementation.dll"
#r "SSW.LinkAuditor.BusinessInterfaces.dll"
#r "SSW.LinkAuditor.Domain.dll"

using System;
using System.Net;
using System.Net.Http;

using SSW.LinkAuditor.BusinessImplementation;
using SSW.LinkAuditor.Domain.Web;
using SSW.LinkAuditor.Domain.Html;
using SSW.LinkAuditor.Domain;

public static void Run(string myQueueItem, ICollector<ScanResultSimple> outputTable, TraceWriter log)
{
    myQueueItem = myQueueItem.Replace('"', ' ').Trim();
    var downloader = new Downloader();
    DateTime startTime = DateTime.Now;
    ContentResult result = downloader.GetContent(myQueueItem);
    var duration = (int)DateTime.Now.Subtract(startTime).TotalMilliseconds;
    outputTable.Add(new ScanResultSimple()
    {
        PartitionKey = "Test",
        RowKey = Guid.NewGuid().ToString(),
        Received = DateTime.Now,
        Duration = duration,
        ResponseUri = myQueueItem,
        ContentLength = result.ContentLength,
        HttpResponseCode = result.HttpResponseCode.ToString(),
        HttpResponseMsg = result.HttpResponseMsg,
        MachineName = Environment.MachineName
    });
    
    log.Info($"Downloaded {myQueueItem}  in {duration}ms, {result.ContentLength} Bytes on Machine {Environment.MachineName}");
}

public class ScanResultSimple
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTime Received { get; set; }
    public int Duration { get; set; }
    public string ResponseUri { get; set; }
    public long ContentLength { get; set; }
    public string HttpResponseCode { get; set; }
    public string HttpResponseMsg { get; set; }
    public string MachineName { get; set; }
}