using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenTelemetry.Unity
{
    public abstract class BaseExporter
    {
        protected TracerProvider _provider;
        internal void Init(TracerProvider provider)
        {
            _provider = provider;
        }

        public abstract void Export(List<TelemetrySpan> spans);

        public abstract void ForceFlush();

        public abstract void Shutdown();

    }
}
