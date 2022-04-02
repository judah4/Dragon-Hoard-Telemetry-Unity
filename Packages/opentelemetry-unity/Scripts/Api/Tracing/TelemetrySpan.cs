using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTelemetry.Unity
{ 
    public class TelemetrySpan
    {
        public string Name { get; }
        public SpanContext SpanContext { get; }

        public TelemetrySpan ParentSpan { get; }
        public SpanKind SpanKind { get; }
        public Timestamp Start { get; }
        public Timestamp End { get; }

        public List<SpanAttr> Attributes { get; }
        public List<Link> Links { get; }
        public List<SpanEvent> Events { get; }
        public SpanStatus Status { get; }

    }
}
