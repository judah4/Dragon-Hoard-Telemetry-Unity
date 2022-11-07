using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenTelemetry.Unity
{
    [System.Serializable]
    public class SpansExport
    {
        public List<SpanExport> Spans;
    }

    [System.Serializable]
    public struct SpanExport
    {
        public string TraceId;
        public string SpanId;
        public string Name;
        public string ParentId;
        public long StartTime;
        public long EndTime;

        public List<AttributeExport> Attributes;
        public List<EventExport> Events;
    }

    [System.Serializable]
    public struct EventExport
    {
        public string Name;
        public long Timestamp;
        public List<AttributeExport> Attributes;

    }

    [System.Serializable]
    public struct AttributeExport
    {
        public string Key;
        public string Value;

    }

    public class JsonExporterOptions
    {
        public bool WriteToApi { get; set; }
        public string ApiUrl { get; set; }
        public string AuthHeader { get; set; }
        public bool PrivacyOptOut { get; set; }
    }

    public class JsonExporter : BaseExporter
    {
        private JsonExporterOptions _options;

        public JsonExporter(JsonExporterOptions options = null)
        {
            if(options == null)
            {
                options = new JsonExporterOptions()
                {
                };
            }
            _options = options;
        }

        public override void Export(List<TelemetrySpan> spans)
        {
            var exports = new SpansExport()
            {
                Spans = new List<SpanExport>(),
            };
        
            for (int cnt = 0; cnt < spans.Count; cnt++)
            {
                var export = SpanToExport(spans[cnt]);
                if(export.HasValue == false)
                    continue;

                exports.Spans.Add(export.Value);

            }

            if (exports == null || exports.Spans.Count == 0)
                return;

            if (_options.WriteToApi)
            {
                WriteToApi(exports);
            }

            //var data = JsonUtility.ToJson(exports, true);
            //Debug.Log(data);
        }

        private void WriteToApi(SpansExport exports)
        {
            if (_options.PrivacyOptOut || string.IsNullOrEmpty(_options.ApiUrl))
                return;

            _provider.StartCoroutine(SendToApi(exports));
        }

        IEnumerator SendToApi(SpansExport exports)
        {
            if(_options.PrivacyOptOut || string.IsNullOrEmpty(_options.ApiUrl))
                yield break;
            if (exports == null || exports.Spans.Count == 0)
                yield break;

            var data = JsonUtility.ToJson(exports, false);

            var webRequest = new UnityWebRequest(_options.ApiUrl, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            //add authorize option
            if (!string.IsNullOrEmpty(_options.AuthHeader))
            {
                webRequest.SetRequestHeader("Authorization", _options.AuthHeader);
            }

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                var text = "";
                if (webRequest.downloadHandler != null)
                {
                    text = webRequest.downloadHandler.text;
                }

                Debug.LogError(webRequest.responseCode + " e:" + webRequest.error + " body: " + text);
            }
        }

        public override void ForceFlush()
        {
            return;
        }

        public override void Shutdown()
        {
            return;
        }

        SpanExport? SpanToExport(TelemetrySpan span)
        {
            if(span.EndTime == null)
                return null;

            var export = new SpanExport()
            {
                TraceId = span.SpanContext.TraceId.ToString(),
                SpanId = span.SpanContext.SpanId.ToString(),
                Name = span.Name,
                ParentId = span.ParentSpan?.SpanId.ToString(),
                StartTime = span.StartTime.Time,
                EndTime = span.EndTime.Value.Time,
                Events = new List<EventExport>(),
                Attributes = new List<AttributeExport>(),
            };

            for (int cnt = 0; cnt < span.Events.Count; cnt++)
            {
                var ev = span.Events[cnt];
                var eExport = new EventExport()
                {
                    Name = ev.Name,
                    Timestamp = ev.Timestamp.Time,
                    Attributes = new List<AttributeExport>(),
                };
                export.Events.Add(eExport);
                ExportAttributes(ev.Attributes, eExport.Attributes);

            }

            ExportAttributes(span.Attributes, export.Attributes);

            return export;
        }

        void ExportAttributes(Dictionary<string,object> attributes, List<AttributeExport> exports)
        {
            foreach (var attr in attributes)
            {
                var aExport = new AttributeExport()
                {
                    Key = attr.Key,
                    Value = attr.Value.ToString(),
                };
                exports.Add(aExport);
            }
        }

    }
}
