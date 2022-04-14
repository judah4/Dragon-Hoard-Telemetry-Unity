using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OpenTelemetry.Unity
{
    public class InMemoryExporter : BaseExporter
    {
        public List<TelemetrySpan> Spans = new List<TelemetrySpan>();

        public override void Export(List<TelemetrySpan> spans)
        {
            if(spans.Count < 1)
                return;

            Spans.AddRange(spans);

        }

        public override void ForceFlush()
        {
            return;
        }

        public override void Shutdown()
        {
            return;
        }

    }
}
