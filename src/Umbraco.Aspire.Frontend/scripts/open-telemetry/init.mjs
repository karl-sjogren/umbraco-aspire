import { ConsoleSpanExporter, SimpleSpanProcessor } from '@opentelemetry/sdk-trace-base';
import { DocumentLoadInstrumentation } from '@opentelemetry/instrumentation-document-load';
import { FetchInstrumentation } from '@opentelemetry/instrumentation-fetch';
import { UserInteractionInstrumentation } from '@opentelemetry/instrumentation-user-interaction';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-proto';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { resourceFromAttributes  } from '@opentelemetry/resources';
import { SemanticResourceAttributes } from '@opentelemetry/semantic-conventions';
import { WebTracerProvider } from '@opentelemetry/sdk-trace-web';
import { ZoneContextManager } from '@opentelemetry/context-zone';
import WebVitalsInstrumentation from './web-vitals-instrumentation';

function initializeTelemetry(otlpUrl, headers, resourceAttributes) {
  const otlpOptions = {
    url: `${otlpUrl}/v1/traces`,
    headers: parseDelimitedValues(headers)
  };

  const attributes = parseDelimitedValues(resourceAttributes);
  attributes[SemanticResourceAttributes.SERVICE_NAME] = 'browser';

  const provider = new WebTracerProvider({
    contextManager: new ZoneContextManager(),
    resource: resourceFromAttributes(attributes),
    spanProcessors: [
      new SimpleSpanProcessor(new ConsoleSpanExporter()),
      new SimpleSpanProcessor(new OTLPTraceExporter(otlpOptions))
    ],
  });

  provider.register();

  // Registering instrumentations
  registerInstrumentations({
    instrumentations: [
      new DocumentLoadInstrumentation(),
      new FetchInstrumentation(),
      new UserInteractionInstrumentation({
        eventNames: ['submit', 'click']
      }),
      new WebVitalsInstrumentation()
    ],
  });
}

function parseDelimitedValues(s) {
  const headers = s.split(',');
  const result = {};

  headers.forEach(header => {
    const [key, value] = header.split('=');
    result[key.trim()] = value.trim();
  });

  return result;
}

const { endpointUrl, headers, attributes } = window.otelExporterConfiguration || {};

if(!!endpointUrl) {
  initializeTelemetry(endpointUrl, headers, attributes);
}
