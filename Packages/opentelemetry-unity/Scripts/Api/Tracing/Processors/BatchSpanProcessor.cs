using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTelemetry.Unity
{
    public class BatchSpanProcessor : SpanProcessor
    {

        bool _shutdown = false;

        List<TelemetrySpan> _batchSpans;

        int _maxQueueSize = 2048;
        int _scheduledDelayMillis = 5000;
        int _exportTimeoutMillis = 30000;
        int _maxExportBatchSize = 512;
        private Coroutine _background;

        public BatchSpanProcessor(BaseExporter exporter, int maxQueueSize = 2048, int scheduledDelayMillis = 5000, int exportTimeoutMillis = 30000, int maxExportBatchSize = 512) : base(exporter)
        {
            _maxQueueSize = maxQueueSize;
            _scheduledDelayMillis = scheduledDelayMillis;
            _exportTimeoutMillis = exportTimeoutMillis;
            _maxExportBatchSize = maxExportBatchSize;
            _batchSpans = new List<TelemetrySpan>(_maxExportBatchSize);
        }

        public override void OnEnd(TelemetrySpan span)
        {
            _openSpans.Remove(span);
            if(_sendList.Count >= _maxQueueSize)
                return;
            _sendList.Add(span);
        }

        public override bool ForceFlush()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            do
            {
                SendBatch();
                _exporter.ForceFlush();

                if(stopwatch.ElapsedMilliseconds >= _exportTimeoutMillis)
                {
                    return false; //timeout
                }

            } while(_sendList.Count > 0);
            
            return true;
        }

        public override bool Shutdown()
        {
            _shutdown = true;
               var flushStatus = ForceFlush();
            if(_background != null)
                _tracerProvider.StopCoroutine(_background);
            return flushStatus;
        }

        protected override void OnInit()
        {
            StartBackground();
        }

        void StartBackground()
        {
            if(_background != null)
                return;
            _background = _tracerProvider.StartCoroutine(BackgroundProcessor());
        }

        private IEnumerator BackgroundProcessor()
        {
            var timeWaiter = new WaitForSeconds(_scheduledDelayMillis / 1000f);
            while(!_shutdown)
            {

                SendBatch();

                yield return timeWaiter;
            }
        }

        void SendBatch()
        {
            try
            {
                _batchSpans.Clear();
                if (_sendList.Count <= _maxExportBatchSize)
                {
                    _batchSpans.AddRange(_sendList);
                    _sendList.Clear();
                }
                else
                {
                    for (int cnt = _maxExportBatchSize - 1; cnt >= 0; cnt--)
                    {
                        _batchSpans.Insert(0, _sendList[cnt]);
                        _sendList.RemoveAt(cnt);
                    }
                }

                _exporter.Export(_batchSpans);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
