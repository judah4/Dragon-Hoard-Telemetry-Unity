using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTelemetry.Unity
{

    public class TracerProvider
    {
        public const string Version = "0.1.0";
        public const string SchemaVersion = "1.10.0";

        List<BaseExporter> _exporters = new List<BaseExporter>();
        List<TelemetryTracer> _tracers = new List<TelemetryTracer>();

        public TelemetryTracer GetTracer(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                name = "OpenTelemetryUnity";
            }

            var tracer = new TelemetryTracer(name, this);
            _tracers.Add(tracer);
            return tracer;
        }
        
        internal void EndSpan(TelemetrySpan span)
        {
            span.SetStatus(SpanStatus.Ok);

            //lets write to exporter!

            for(int cnt = 0; cnt < _exporters.Count; cnt++)
            {
                _exporters[cnt].WriteTracer(span.Tracer);
            }
        }

    }

}
