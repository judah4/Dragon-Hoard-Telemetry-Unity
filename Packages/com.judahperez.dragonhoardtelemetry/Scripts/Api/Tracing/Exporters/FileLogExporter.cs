using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenTelemetry.Unity
{

    public class FileLogExporterOptions
    {
        public string FileName { get; set; } = $"exports/spanexports{System.DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss")}.log";
    }


    public class FileLogExporter : BaseExporter
    {
        private FileLogExporterOptions _options;

        public FileLogExporter(FileLogExporterOptions options = null)
        {
            if (options == null)
            {
                options = new FileLogExporterOptions()
                {
                };
            }
            _options = options;
        }

        public override void Export(List<TelemetrySpan> spans)
        {


            if (spans == null || spans.Count == 0)
                return;

            WriteToFile(spans);

        }

        private void WriteToFile(List<TelemetrySpan> exports)
        {
            try
            {
                if (string.IsNullOrEmpty(_options.FileName))
                    return;

                var fullPath = System.IO.Path.Combine(Application.persistentDataPath, _options.FileName);
                var subPath = System.IO.Path.GetDirectoryName(fullPath);
                System.IO.Directory.CreateDirectory(subPath);

                using (var writer = System.IO.File.AppendText(fullPath))
                {
                    foreach (var span in exports)
                    {
                        var data = SpanToExport(span);
                        if(data == null)
                            return;

                        writer.WriteLine(data);
                    }
                }

            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
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

        string SpanToExport(TelemetrySpan span)
        {
            if (span.EndTime == null)
                return null;

            var stringBuilder = new StringBuilder();

            stringBuilder.Append("01-");
            stringBuilder.Append(span.SpanContext.TraceId);
            stringBuilder.Append("-");
            stringBuilder.Append(span.SpanContext.SpanId);
            stringBuilder.Append("-00 ");
            stringBuilder.Append("parent:");
            stringBuilder.Append(span.ParentSpan?.SpanId);
            stringBuilder.Append(" start:");
            stringBuilder.Append(span.StartTime.Time);
            stringBuilder.Append(" end:");
            stringBuilder.Append(span.EndTime.Value.Time);

            ExportAttributes(span.Attributes, stringBuilder);

            for (int cnt = 0; cnt < span.Events.Count; cnt++)
            {
                var ev = span.Events[cnt];

                stringBuilder.Append(" evt:");
                stringBuilder.Append(ev.Name);
                stringBuilder.Append(" time:");
                stringBuilder.Append(ev.Timestamp.Time);
                ExportAttributes(ev.Attributes, stringBuilder);

            }

            return stringBuilder.ToString();
        }

        void ExportAttributes(Dictionary<string, object> attributes, StringBuilder stringBuilder)
        {
            if(attributes.Count < 1)
                return;

            stringBuilder.Append(" atr:");
            foreach (var attr in attributes)
            {
                stringBuilder.Append("(");
                stringBuilder.Append(attr.Key);
                stringBuilder.Append(",");
                stringBuilder.Append(attr.Value);
                stringBuilder.Append(")");

            }
        }

    }
}
