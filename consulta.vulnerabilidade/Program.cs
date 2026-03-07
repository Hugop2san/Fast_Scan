using consulta.vulnerabilidade.Application.Abstractions;
using consulta.vulnerabilidade.Application.Scans;
using consulta.vulnerabilidade.Infrastructure.AI;
using consulta.vulnerabilidade.Infrastructure.Analyzers;
using consulta.vulnerabilidade.Infrastructure.Bus;
using consulta.vulnerabilidade.Infrastructure.Extractors;
using consulta.vulnerabilidade.Infrastructure.Storage;
using Consulta.Vulnerabilidade.Application.Scans.Handlers;
using Consulta.Vulnerabilidade.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();

// EDA
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();

// Use case dispatcher
builder.Services.AddScoped<ScanDispatcher>();

// Extractor (público)
builder.Services.AddScoped<IPublicSurfaceExtractor, HttpPublicSurfaceExtractor>();

// Analyzers
builder.Services.AddScoped<IVulnerabilityAnalyzer, SecurityHeadersAnalyzer>();
builder.Services.AddScoped<IVulnerabilityAnalyzer, FingerprintAnalyzer>();
builder.Services.AddScoped<IVulnerabilityAnalyzer, BlazorHintsAnalyzer>();

// Storage local
builder.Services.AddScoped<ILocalStorage, BrowserLocalStorage>();
builder.Services.AddScoped<IScanReportRepository, ScanReportRepositoryLocalStorage>();

// AI slot (por enquanto null model)
builder.Services.AddScoped<IAiModel, NullAiModel>();

// Handlers
builder.Services.AddScoped<IEventHandler<consulta.vulnerabilidade.Domain.Scans.Events.ScanRequested>, ScanRequestedHandler>();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<consulta.vulnerabilidade.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
