using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OpenTelemetry.Unity
{
    public class DebugExporter
    {
        

        public void WriteTracer(TelemetryTracer tracer)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Trace:{tracer.Name} - Id:{tracer.TraceId}");

            for(int cnt = 0; cnt < tracer.Spans.Count; cnt++)
            {
                WriteSpan(stringBuilder, tracer.Spans[cnt]);
            }

            Debug.Log(stringBuilder.ToString());
        }

        void WriteSpan(StringBuilder stringBuilder, TelemetrySpan span)
        {
            stringBuilder.AppendLine($"Span:{span.Name} - Id:{span.SpanContext.SpanId}");
            if(span.ParentSpan.HasValue)
            {
                stringBuilder.AppendLine($"ParentId:{span.ParentSpan.Value.SpanId}");
            }
            stringBuilder.AppendLine($"Start:{span.Start}");
            stringBuilder.AppendLine($"End:{span.End}");
            for(int cnt = 0; cnt < span.Events.Count; cnt++) 
            {
                var ev = span.Events[cnt];
                stringBuilder.AppendLine($"Event:{ev.Name}");
                stringBuilder.AppendLine($"Time:{ev.Timestamp}");
            }

            foreach(var attr in span.Attributes)
            {
                stringBuilder.AppendLine($"Attr:{attr.Key}, {attr.Value}");
            }

        }



    }
}
