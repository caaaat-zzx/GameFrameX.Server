﻿using System.Diagnostics;
using Prometheus;

namespace GameFrameX.Monitor;

/// <summary>
/// 监控帮助类
/// </summary>
public static class MetricsHelper
{
    private static KestrelMetricServer _server;

    /// <summary>
    /// 停止监控
    /// </summary>
    public static void Stop()
    {
        _server.Stop();
    }

    /// <summary>
    /// 启动监控
    /// </summary>
    /// <param name="port">对外访问的端口</param>
    public static void Start(int port = 0)
    {
        if (port <= 0)
        {
            return;
        }

        Metrics.SuppressDefaultMetrics(new SuppressDefaultMetricOptions
        {
            SuppressEventCounters = true,
            SuppressMeters = true,
            SuppressProcessMetrics = true,
        });
        _server = new KestrelMetricServer(port);
        _server.Start();

        var totalSleepTime = Metrics.CreateCounter("sample_sleep_seconds_total", "Total amount of time spent sleeping.");

        _ = Task.Run(async delegate
        {
            while (true)
            {
                // DEFAULT EXEMPLAR: We expose the trace_id and span_id for distributed tracing, based on Activity.Current.
                // Activity.Current is often automatically inherited from incoming HTTP requests if using OpenTelemetry tracing with ASP.NET Core.
                // Here, we manually create and start an activity for sample purposes, without relying on the platform managing the activity context.
                // See https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-concepts
                using var activity = new Activity("Pausing before record processing").Start();
                var sleepStopwatch = Stopwatch.StartNew();
                await Task.Delay(TimeSpan.FromSeconds(1));

                // The trace_id and span_id from the current Activity are exposed as the exemplar by default.
                totalSleepTime.Inc(sleepStopwatch.Elapsed.TotalSeconds);
            }
        });
        Console.WriteLine($"Open http://localhost:{port}/metrics?accept=application/openmetrics-text in a web browser.");
    }
}