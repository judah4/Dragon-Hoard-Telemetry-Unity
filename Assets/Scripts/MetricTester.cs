using OpenTelemetry.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTelemetry.Unity.Examples 
{ 

    public class MetricTester : MonoBehaviour
    {
        [SerializeField]
        private SimpleTelemetryConfig _config;

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        public void Init()
        {

            var jsonOptions = new JsonExporterOptions()
            {
                WriteToApi = false,
                WriteToFile = true,
            };
            if(_config)
            {
                jsonOptions.WriteToApi = _config.WriteToApi;
                jsonOptions.ApiUrl = _config.ApiUrl;
                jsonOptions.AuthHeader = _config.AuthHeader;
            }

            var tracerProvider = TracerProvider.Create(new List<SpanProcessor>() {
                new SpanProcessor(new DebugExporter()),
                new BatchSpanProcessor(new JsonExporter(jsonOptions)),
            });

            var tracer = tracerProvider.GetTracer(nameof(MetricTester));

            var span = tracer.GetSpan("Test Span");
            span.Attributes.Add("foo", "bar");

            var span2 = tracer.GetSpan("Another Span", span.SpanContext);
            span2.Events.Add(SpanEvent.Create("Wow, event", Timestamp.Create()));
            span2.End();

            span.End();

        }
    }
}
