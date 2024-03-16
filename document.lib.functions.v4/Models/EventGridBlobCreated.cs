using System;
using System.Text.Json.Serialization;

namespace document.lib.functions.v4.Models;

public class EventGridBlobCreated
{
    [JsonPropertyName("topic")]
    public string Topic { get; set; } = null!;

    [JsonPropertyName("subject")]
    public string Subject { get; set; } = null!;

    [JsonPropertyName("eventType")]
    public string EventType { get; set; } = null!;

    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("data")]
    public Data Data { get; set; } = null!;

    [JsonPropertyName("dataVersion")]
    public string DataVersion { get; set; } = null!;

    [JsonPropertyName("metadataVersion")]
    public string MetadataVersion { get; set; } = null!;

    [JsonPropertyName("eventTime")]
    public DateTime EventTime { get; set; } = DateTime.MinValue;
}

public class Data
{

    [JsonPropertyName("api")]
    public string Api { get; set; } = null!;
    
    [JsonPropertyName("clientRequestId")]
    public string ClientRequestId { get; set; } = null!;

    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = null!;

    [JsonPropertyName("eTag")]
    public string ETag { get; set; } = null!;

    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = null!;

    [JsonPropertyName("contentLength")]
    public int ContentLength { get; set; }

    [JsonPropertyName("blobType")]
    public string BlobType { get; set; } = null!;

    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;

    [JsonPropertyName("sequencer")]
    public string Sequencer { get; set; } = null!;

    [JsonPropertyName("storageDiagnostics")]
    public Storagediagnostics StorageDiagnostics { get; set; } = null!;
}

public class Storagediagnostics
{
    [JsonPropertyName("batchId")]
    public string BatchId { get; set; } = null!;
}
