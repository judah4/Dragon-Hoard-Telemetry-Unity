using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTelemetry.Unity
{


    public class TelemetryTracer
    {
        public string Name { get; }

        public TraceId TraceId { get; }

        public TelemetryTracer(string name)
        {
            Name = name;
            TraceId = TraceId.Create();
        }


        /*
             The span name. This is a required parameter.

    The parent Context or an indication that the new Span should be a root Span. The API MAY also have an option for implicitly using the current Context as parent as a default behavior. This API MUST NOT accept a Span or SpanContext as parent, only a full Context.

    The semantic parent of the Span MUST be determined according to the rules described in Determining the Parent Span from a Context.

    SpanKind, default to SpanKind.Internal if not specified.

    Attributes. Additionally, these attributes may be used to make a sampling decision as noted in sampling description. An empty collection will be assumed if not specified.

    Whenever possible, users SHOULD set any already known attributes at span creation instead of calling SetAttribute later.

    Links - an ordered sequence of Links, see API definition here.

    Start timestamp, default to current time. This argument SHOULD only be set when span creation time has already passed. If API is called at a moment of a Span logical start, API user MUST NOT explicitly set this argument.
        */
        public TelemetrySpan GetSpan(string name, SpanContext? parent = null, SpanKind spanKind = SpanKind.INTERNAL,
            Dictionary<string, object> attributes = null, List<SpanLink> links = null, Timestamp? timestamp = null)
        {
            if(timestamp.HasValue == false)
                timestamp = Timestamp.Create();
            if(attributes == null)
                attributes = new Dictionary<string, object>();
            if(links == null)
                links = new List<SpanLink>();

            var span = TelemetrySpan.Create(name, SpanContext.Create(SpanId.Create(), TraceId, 0), parent, spanKind, attributes,
                links, timestamp.Value );

            return span;
        }
    }

}