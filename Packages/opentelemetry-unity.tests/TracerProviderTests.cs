using NUnit.Framework;
using OpenTelemetry.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace OpenTelemetry.Unity.Tests
{
    [TestFixture]
    public class TracerProviderTests
    {

        TracerProvider _tracerProvider;

        InMemoryExporter _inMemoryExporter;

        [SetUp]
        public virtual void Setup()
        {
            _inMemoryExporter = new InMemoryExporter();

            _tracerProvider = TracerProvider.Create(new List<SpanProcessor>() {
                new SpanProcessor(new DebugExporter()),
                new SpanProcessor(_inMemoryExporter),
                new BatchSpanProcessor(new InMemoryExporter()),
            });
        }

        [TearDown]
        public virtual void TearDown()
        {
        }

        [Test]
        public void SampleSpanTest()
        {
            var tracer = _tracerProvider.GetTracer("Tester");

            var span = tracer.GetSpan("Test Span");
            span.Attributes.Add("foo", "bar");
            span.Events.Add(SpanEvent.Create("Test Event", Timestamp.Create()));
            Assert.IsNull(span.EndTime);

            span.End();

            Assert.AreEqual(1, _inMemoryExporter.Spans.Count);
            var exportedSpan = _inMemoryExporter.Spans[0];

            Assert.AreNotEqual(new SpanId(), span.SpanContext.SpanId);
            Assert.AreNotEqual(new TraceId(), span.SpanContext.TraceId);
            Assert.AreEqual(span.SpanContext.TraceId, exportedSpan.SpanContext.TraceId);
            Assert.AreEqual(span.SpanContext.SpanId, exportedSpan.SpanContext.SpanId);

            Assert.IsNotNull(exportedSpan.EndTime);
            Assert.AreEqual("Test Span", exportedSpan.Name);
            Assert.AreEqual(1, exportedSpan.Attributes.Count);
            Assert.AreEqual(1, exportedSpan.Events.Count);

        }

    }
}
