using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OpenTelemetry.Unity
{
    public class DebugExporter : BaseExporter
    {
        public override void Export(List<TelemetrySpan> spans)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int cnt = 0; cnt < spans.Count; cnt++)
            {
                WriteSpan(stringBuilder, spans[cnt]);
            }

            Debug.Log(stringBuilder.ToString());
        }

        public override void ForceFlush()
        {
            return;
        }

        public override void Shutdown()
        {
            return;
        }

        void WriteSpan(StringBuilder stringBuilder, TelemetrySpan span)
        {
            stringBuilder.AppendLine($"Span:{span.Name} - TraceId:{span.SpanContext.TraceId}, Id:{span.SpanContext.SpanId}");
            if(span.ParentSpan.HasValue)
            {
                stringBuilder.AppendLine($"ParentId:{span.ParentSpan.Value.SpanId}");
            }
            stringBuilder.AppendLine($"Start:{span.StartTime}");
            stringBuilder.AppendLine($"End:{span.StartTime}");
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
