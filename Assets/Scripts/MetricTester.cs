using OpenTelemetry.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTelemetry.Unity.Examples 
{ 

    public class MetricTester : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void Init()
        {
            var exporter = new DebugExporter();
            var tracerProvider = new TracerProvider();

            var tracer = tracerProvider.GetTracer(nameof(MetricTester));

            var span = tracer.GetSpan("Test Span");
            span.Attributes.Add("foo", "bar");

            var span2 = tracer.GetSpan("Another Span", span.SpanContext);
            span2.Events.Add(SpanEvent.Create("Wow, event", Timestamp.Create()));
            span2.SetEnd();

            span.SetEnd();

            exporter.WriteTracer(tracer);

            //print?
        }
    }
}
