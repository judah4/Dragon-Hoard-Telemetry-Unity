using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Unity
{
    public abstract class BaseExporter
    {
        public abstract void WriteTracer(TelemetryTracer tracer);

    }
}
