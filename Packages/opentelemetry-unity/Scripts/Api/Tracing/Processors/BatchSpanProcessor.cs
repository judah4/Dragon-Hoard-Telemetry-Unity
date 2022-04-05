using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTelemetry.Unity
{
    public class BatchSpanProcessor : SpanProcessor
    {
        public BatchSpanProcessor(BaseExporter exporter, int maxQueueSize = 2048, int scheduledDelayMillis = 5000, int exportTimeoutMillis = 30000, int maxExportBatchSize = 512) : base(exporter)
        {
            
        }
    }
}
