using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTelemetry.Unity
{

    public class TracerProvider
    {
        public const string Version = "0.1.0";
        public const string SchemaVersion = "1.10.0";

        public TelemetryTracer GetTracer(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                name = "OpenTelemetryUnity";
            }

            var tracer = new TelemetryTracer(name);
            return tracer;
        }
    }

}
