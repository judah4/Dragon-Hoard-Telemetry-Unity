using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTelemetry.Unity
{
    public enum SpanKind
    { 
        CLIENT,
        SERVER,
        PRODUCER,
        CONSUMER,
        INTERNAL,
    }

    public enum SpanStatus
    {
        Unset, //The default status.
        Ok, //The operation has been validated by an Application developer or Operator to have completed successfully.
        Error, //The operation contains an error.
    }

    public struct Timestamp
    {
        public long Time { get; private set; }

        public static Timestamp Create()
        {
            return Create(System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }

        public static Timestamp Create(long time)
        {
            var timestamp = new Timestamp()
            {
                Time = time,
            };
            return timestamp;
        }

        public override string ToString()
        {
            return Time.ToString();
        }
    }

    public class TelemetrySpan
    {
        TelemetryTracer _tracer;
        TracerProvider _tracerProvider;

        internal TelemetryTracer Tracer => _tracer;

        public string Name { get; private set; }
        public SpanContext SpanContext { get; private set; }

        public SpanContext? ParentSpan { get; private set; }
        public SpanKind SpanKind { get; private set; }
        public Timestamp Start { get; private set; }
        public Timestamp? End { get; private set; }

        public Dictionary<string,object> Attributes { get; private set; }
        public List<SpanLink> Links { get; private set; }
        public List<SpanEvent> Events { get; private set; }
        public SpanStatus Status { get; private set; }

        public static TelemetrySpan Create(string name, SpanContext context, SpanContext? parent, SpanKind spanKind,
            Dictionary<string, object> attributes, List<SpanLink> links, Timestamp start, TelemetryTracer tracer, TracerProvider tracerProvider)
        {
            var ev = new TelemetrySpan()
            {
                Name = name,              
                SpanContext = context,
                ParentSpan = parent,
                SpanKind = spanKind,
                Attributes = attributes,
                Links = links,
                Start = start,
                Events = new List<SpanEvent>(),
                Status = SpanStatus.Unset,
                _tracer = tracer,
                _tracerProvider = tracerProvider,
            };
            return ev;
        }

        public void SetEnd()
        {
            if(End.HasValue)
                return;
            End = Timestamp.Create();

            _tracerProvider.EndSpan(this);
        }

        internal void SetStatus(SpanStatus status)
        {
            Status = status;
        }
    }

    public struct SpanEvent
    {
        public string Name { get; private set; }
        public Timestamp Timestamp { get; private set; }
        public Dictionary<string, object> Attributes { get; private set; }

        public static SpanEvent Create(string name, Timestamp timestamp)
        {
            var ev = new SpanEvent()
            {
                Name = name,
                Timestamp = timestamp,
                Attributes = new Dictionary<string, object>(),
            };
            return ev;
        }

    }

    public struct SpanLink
    {
        public SpanContext Span { get; private set; }
        public Dictionary<string, object> Attributes { get; private set; }

        public static SpanLink Create(SpanContext span)
        {
            var link = new SpanLink()
            {
                Span = span,
                Attributes = new Dictionary<string, object>(),
            };
            return link;
        }

    }

}
