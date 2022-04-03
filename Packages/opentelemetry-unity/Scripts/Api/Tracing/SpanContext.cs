using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace OpenTelemetry.Unity
{
    public struct TraceId : IEqualityComparer<TraceId>
    {
        byte[] _id;

        public static TraceId Create()
        {
            var traceId = new TraceId()
            {
                _id = System.Guid.NewGuid().ToByteArray(),
            };
            return traceId;
        }

        public override bool Equals(object obj)
        {
            if((obj is TraceId) == false)
                return false;
            var traceId = (TraceId)obj;

            return Equals(this, traceId);
        }

        public bool Equals(TraceId x, TraceId y)
        {
            return x._id == y._id;
        }

        public override int GetHashCode()
        {
            return System.BitConverter.ToString(_id).GetHashCode();
        }

        public int GetHashCode(TraceId obj)
        {
            return System.BitConverter.ToString(obj._id).GetHashCode();
        }

        public override string ToString()
        {
            return System.BitConverter.ToString(_id, 0).Replace("-", "");
        }

        public static bool operator ==(TraceId a, TraceId b)
        => a.Equals(b);
        public static bool operator !=(TraceId a, TraceId b)
        => !a.Equals(b);
    }

    public struct SpanId : IEqualityComparer<SpanId>
    {
        byte[] _id;

        public static SpanId Create()
        {
            var traceId = new SpanId()
            {
                _id = new byte[8],
            };

            var guid = System.Guid.NewGuid().ToByteArray();

            //make this generation better
            traceId._id[0] = guid[0];
            traceId._id[1] = guid[1];
            traceId._id[2] = guid[2];
            traceId._id[3] = guid[3];
            traceId._id[4] = guid[8];
            traceId._id[5] = guid[9];
            traceId._id[6] = guid[10];
            traceId._id[7] = guid[12];
            return traceId;
        }

        public override bool Equals(object obj)
        {
            if ((obj is SpanId) == false)
                return false;
            var traceId = (SpanId)obj;

            return Equals(this, traceId);
        }

        public bool Equals(SpanId x, SpanId y)
        {
            return x._id == y._id;
        }

        public override int GetHashCode()
        {
            return System.BitConverter.ToString(_id).GetHashCode();
        }

        public int GetHashCode(SpanId obj)
        {
            return System.BitConverter.ToString(_id).GetHashCode();
        }

        public override string ToString()
        {
            return System.BitConverter.ToString(_id, 0).Replace("-", "");
        }

        public static bool operator == (SpanId a, SpanId b)
        => a.Equals(b);
        public static bool operator != (SpanId a, SpanId b)
        => !a.Equals(b);
    }

    public struct SpanContext
    {
        public SpanId SpanId { get; private set; }

        public TraceId TraceId { get; private set; }

        public byte Flags { get; private set; }

        //public TraceState TraceState { get; }

        public static SpanContext Create(SpanId spanId, TraceId traceId, byte flags)
        {
            var context = new SpanContext()
            {
                SpanId = spanId,
                TraceId = traceId,
                Flags = flags,
            };
            return context;
        }

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
