using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTelemetry.Unity
{

    public class SpanProcessor
    {
        protected BaseExporter _exporter;
        protected List<TelemetrySpan> _openSpans = new List<TelemetrySpan>();

        protected List<TelemetrySpan> _sendList = new List<TelemetrySpan>();

        protected TracerProvider _tracerProvider;

        public SpanProcessor(BaseExporter exporter)
        {
            _exporter = exporter;
        }

        public virtual void OnStart(TelemetrySpan span, SpanContext? parent)
        {
            _openSpans.Add(span);
        }

        public virtual void OnEnd(TelemetrySpan span)
        {
            _openSpans.Remove(span);
            _sendList.Clear();
            _sendList.Add(span);
            _exporter.Export(_sendList);
        }

        public virtual bool ForceFlush()
        {
            return true;
        }

        public virtual bool Shutdown()
        {
            return ForceFlush();
        }

        internal void Init(TracerProvider tracerProvider)
        {
            _tracerProvider = tracerProvider;
            //set
            _exporter.Init(tracerProvider);
            OnInit();
        }

        protected virtual void OnInit()
        {

        }
    }
}
