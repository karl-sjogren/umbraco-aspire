import { onCLS, onFCP, onINP, onLCP, onTTFB } from 'web-vitals';
import { InstrumentationBase } from '@opentelemetry/instrumentation';
import { trace, context } from '@opentelemetry/api';
import { hrTime } from '@opentelemetry/core';

export default class WebVitalsInstrumentation extends InstrumentationBase {
  onReport(metric, parentSpanContext) {
    const now = hrTime();

    // Should possibly be a metric isntead of a span?
    const webVitalsSpan = trace
      .getTracer('web-vitals-instrumentation')
      .startSpan(metric.name, { startTime: now }, parentSpanContext);

    webVitalsSpan.setAttributes({
      'web_vital.name': metric.name,
      'web_vital.id': metric.id,
      'web_vital.navigationType': metric.navigationType,
      'web_vital.delta': metric.delta,
      'web_vital.rating': metric.rating,
      'web_vital.value': metric.value,
      'web_vital.entries': JSON.stringify(metric.entries),
    });

    webVitalsSpan.end();
  }

  enable() {
    if(this.enabled) {
      return;
    }
    this.enabled = true;

    const parentSpan = trace.getTracer('web-vitals-instrumentation').startSpan('web-vitals');
    const ctx = trace.setSpan(context.active(), parentSpan);
    parentSpan.end();

    onCLS((metric) => {
      this.onReport(metric, ctx);
    });

    onFCP((metric) => {
      this.onReport(metric, ctx);
    });

    onINP((metric) => {
      this.onReport(metric, ctx);
    });

    onLCP((metric) => {
      this.onReport(metric, ctx);
    });

    onTTFB((metric) => {
      this.onReport(metric, ctx);
    });
  }
}
