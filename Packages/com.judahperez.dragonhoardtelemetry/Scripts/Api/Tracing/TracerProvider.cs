using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTelemetry.Unity
{

    public class TracerProvider : MonoBehaviour
    {
        static TracerProvider _instance;

        public const string Version = "0.1.0";
        public const string SchemaVersion = "1.10.0";

        public static TracerProvider Instance => _instance;

        public static TracerProvider Create(List<SpanProcessor> processors)
        {
            var gm = new GameObject("TracerProvider");
            var provider = gm.AddComponent<TracerProvider>();
            provider._gm = gm;
            Object.DontDestroyOnLoad(provider._gm);
            if (_instance == null)
            {
                _instance = provider;
            }
            provider.Init(processors);
            return provider;
        }

        List<TelemetryTracer> _tracers = new List<TelemetryTracer>();

        List<SpanProcessor> _processors;

        GameObject _gm;
        bool _shutdownRequested = false;

        private void Init(List<SpanProcessor> processors)
        {
            _processors = processors;
            if (_processors == null)
            {
                _processors = new List<SpanProcessor>();
            }

            for (int cnt = 0; cnt < _processors.Count; cnt++)
            {
                _processors[cnt].Init(this);
            }
        }

        public bool ForceFlush()
        {
            bool status = true;
            for (int cnt = 0; cnt < _processors.Count; cnt++)
            {
                status = _processors[cnt].ForceFlush();
            }
            return status;
        }

        public bool Shutdown()
        {
            if(_shutdownRequested)
                return false; //noop
            _shutdownRequested = true;
            bool status = true;
            for (int cnt = 0; cnt < _processors.Count; cnt++)
            {
                status = _processors[cnt].Shutdown();
            }

            if(_gm != null)
            {
                Object.Destroy(_gm);
            }

            return status;
        }

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

            //lets write to processor!

            for(int cnt = 0; cnt < _processors.Count; cnt++)
            {
                _processors[cnt].OnEnd(span);
            }
        }

        private void OnApplicationQuit()
        {
            Shutdown();
        }

        private void OnDestroy()
        {
            Shutdown();
        }

    }

}
