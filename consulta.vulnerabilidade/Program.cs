using consulta.vulnerabilidade.Application.Abstractions;
using consulta.vulnerabilidade.Application.AgentFix.Abstractions;
using consulta.vulnerabilidade.Application.AgentFix.Handlers;
using consulta.vulnerabilidade.Application.AgentFix.Handles;
using consulta.vulnerabilidade.Application.AgentFix.Requests;
using consulta.vulnerabilidade.Application.Scans;
using consulta.vulnerabilidade.Domain.AgentFix;
using consulta.vulnerabilidade.Infrastructure.AgentFix.Agent;
using consulta.vulnerabilidade.Infrastructure.AgentFix.Github;
using consulta.vulnerabilidade.Infrastructure.AgentFix.OpenAI;
using consulta.vulnerabilidade.Infrastructure.AgentFix.Policy;
using consulta.vulnerabilidade.Infrastructure.AgentFix.Runtime;
using consulta.vulnerabilidade.Infrastructure.AgentFix.Skills;
using consulta.vulnerabilidade.Infrastructure.AgentFix.Storage;
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
builder.Services.AddScoped<IEventBus, InMemoryEventBus>();

// Use case dispatcher
builder.Services.AddScoped<ScanDispatcher>();

// Extractor (publico)
builder.Services.AddScoped<IPublicSurfaceExtractor, HttpPublicSurfaceExtractor>();

// Analyzers
builder.Services.AddScoped<IVulnerabilityAnalyzer, SecurityHeadersAnalyzer>();
builder.Services.AddScoped<IVulnerabilityAnalyzer, FingerprintAnalyzer>();

// Storage local
builder.Services.AddScoped<ILocalStorage, BrowserLocalStorage>();
builder.Services.AddScoped<IScanReportRepository, ScanReportRepositoryLocalStorage>();

// AI slot
builder.Services.AddScoped<IAiModel, NullAiModel>();

// Handlers
builder.Services.AddScoped<IEventHandler<consulta.vulnerabilidade.Domain.Scans.Events.ScanRequested>, ScanRequestedHandler>();
builder.Services.AddScoped<RequestFixHandler>();
builder.Services.AddScoped<ProcessFixJobHandler>();

// Agent options
builder.Services.Configure<GitHubApiOptions>(
    builder.Configuration.GetSection(GitHubApiOptions.SectionName));
builder.Services.Configure<OpenAiOptions>(
    builder.Configuration.GetSection(OpenAiOptions.SectionName));

// Agent runtime
builder.Services.AddSingleton<IFixJobRepository, InMemoryFixJobRepository>();
builder.Services.AddScoped<IGitProvider, GitHubApiProvider>();
builder.Services.AddHttpClient<IOpenAiClient, OpenAiHttpClient>();
builder.Services.AddScoped<ISkill, SecurityHeadersSkill>();
builder.Services.AddScoped<ISkill, CorsSkill>();
builder.Services.AddScoped<ISkill, DependencyUpgradeSkill>();
builder.Services.AddScoped<IFixAgent, OpenAiFixAgent>();
builder.Services.AddSingleton<IAgentToolPolicy, AgentToolPolicy>();
builder.Services.AddSingleton<FixJobQueue>();
builder.Services.AddHostedService<FixJobWorker>();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<consulta.vulnerabilidade.Components.App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/api/fix-jobs", async (
    CreateFixJobRequest request,
    RequestFixHandler handler,
    FixJobQueue queue,
    IAgentToolPolicy policy,
    CancellationToken ct) =>
{
    if (!policy.CanUseRoute("POST", "/api/fix-jobs"))
        return Results.Forbid();

    if (string.IsNullOrWhiteSpace(request.ScanId)
        || string.IsNullOrWhiteSpace(request.FindingId)
        || string.IsNullOrWhiteSpace(request.Repository))
    {
        return Results.BadRequest("scanId, findingId e repository sao obrigatorios.");
    }

    var command = new RequestFixCommand(
        request.ScanId,
        request.FindingId,
        request.Repository,
        string.IsNullOrWhiteSpace(request.BaseBranch) ? "main" : request.BaseBranch,
        string.IsNullOrWhiteSpace(request.Severity) ? "medium" : request.Severity);

    var result = await handler.HandleAsync(command, ct);

    if (result.Status == FixJobStatus.Queued.ToString() && Guid.TryParse(result.JobId, out var parsed))
    {
        await queue.EnqueueAsync(new FixJobId(parsed), ct);
    }

    return Results.Accepted($"/api/fix-jobs/{result.JobId}", result);
});

app.MapGet("/api/fix-jobs/{id}", async (
    string id,
    IFixJobRepository repo,
    IAgentToolPolicy policy,
    CancellationToken ct) =>
{
    if (!policy.CanUseRoute("GET", $"/api/fix-jobs/{id}"))
        return Results.Forbid();

    if (!Guid.TryParse(id, out var parsed))
        return Results.BadRequest("JobId invalido.");

    var job = await repo.GetByIdAsync(new FixJobId(parsed), ct);
    if (job is null)
        return Results.NotFound();

    return Results.Ok(new
    {
        jobId = job.Id.ToString(),
        status = job.Status.ToString(),
        prUrl = job.CreatedPrUrl,
        errorMessage = job.ErrorMessage
    });
});

app.Run();

public sealed record CreateFixJobRequest(
    string ScanId,
    string FindingId,
    string Repository,
    string BaseBranch = "main",
    string Severity = "medium");
