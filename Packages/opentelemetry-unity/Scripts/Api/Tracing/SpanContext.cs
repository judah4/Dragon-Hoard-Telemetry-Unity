using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace OpenTelemetry.Unity
{
    public struct TraceId : IEqualityComparer<TraceId>
    {
        uint _id;

        public static TraceId Create()
        {
            var traceId = new TraceId()
            {
                _id = (uint)(Random.value * uint.MaxValue),
            };
            return traceId;
        }

        public bool Equals(TraceId x, TraceId y)
        {
            return x._id == y._id;
        }

        public int GetHashCode(TraceId obj)
        {
            return (int)_id;
        }

        public override string ToString()
        {
            return _id.ToString("X8");
        }

        public static bool operator ==(TraceId a, TraceId b)
        => a.Equals(b);
        public static bool operator !=(TraceId a, TraceId b)
        => !a.Equals(b);
    }
    public struct SpanId : IEqualityComparer<SpanId>
    {
        ushort _id;
        public static SpanId Create()
        {
            var id = new SpanId()
            {
                _id = (ushort)(Random.value * ushort.MaxValue),
            };
            return id;
        }

        public bool Equals(SpanId x, SpanId y)
        {
            return x._id == y._id;
        }

        public int GetHashCode(SpanId obj)
        {
            return _id;
        }

        public override string ToString()
        {
            return _id.ToString("X8");
        }
    }

    public struct SpanContext
    {
        public SpanId SpanId { get; }

        public TraceId TraceId { get; }

        public byte Flags { get; }

        public TraceState TraceState { get; }

        public bool IsValid()
        {
            return (SpanId != new SpanId() && TraceId != new TraceId());
                
        }

        public bool IsRemote()
        {
            return false;
        }
    }
}
