# Fast Scan (MVP)

Scanner web em Blazor Server para analise passiva de vulnerabilidades em conteudo publico de um site.

## Objetivo
- Receber uma URL.
- Coletar superficie publica (status HTTP, headers e HTML).
- Executar analisadores deterministicos.
- Opcionalmente enriquecer achados com IA.
- Persistir resultado localmente e mostrar score + achados na UI.

## Stack
- .NET 8
- Blazor Server (Razor Components Interactive Server)
- Bootstrap (estilo base)
- Event Bus in-memory (EDA local)

## Como rodar
1. Tenha o .NET 8 SDK instalado.
2. Na raiz do projeto, execute:

```bash
dotnet restore
dotnet build
dotnet run
```

3. Acesse a URL exibida no terminal (normalmente `https://localhost:xxxx`).

## Fluxo EDA (quem chama quem)
1. `Home.razor` chama `ScanDispatcher.RequestScanAsync(url, useAi)`.
2. `ScanDispatcher` publica `ScanRequested` no `IEventBus`.
3. `InMemoryEventBus` resolve e executa `IEventHandler<ScanRequested>`.
4. `ScanRequestedHandler`:
   - extrai superficie publica (`IPublicSurfaceExtractor`);
   - roda todos os `IVulnerabilityAnalyzer`;
   - opcionalmente chama `IAiModel`;
   - salva em `IScanReportRepository`;
   - publica `ScanCompleted`.
5. `Home.razor` consulta `GetLastAsync` e `GetRecentAsync` para renderizar resultado.

## Estrutura do projeto
- `Components/`
  - UI (pagina Home, rotas e layouts).
- `Application/`
  - contratos e orquestracao de caso de uso (`ScanDispatcher`, handlers).
- `Domain/`
  - entidades/eventos (`ScanReport`, `ScanFinding`, `ScanRequested`).
- `Infrastructure/`
  - implementacoes tecnicas (event bus, extractor HTTP, analyzers, storage local, IA null).
- `docs/`
  - backlog de proximas demandas.

## Analisadores atuais
- `SecurityHeadersAnalyzer`
  - valida headers de seguranca (CSP, HSTS, X-Frame-Options etc.).
- `FingerprintAnalyzer`
  - sinaliza exposicao de tecnologias via headers.

## Persistencia
- Repositorio atual: `ScanReportRepositoryLocalStorage`.
- Armazenamento: `localStorage` do navegador via `IJSRuntime`.

## Proximos passos
- Backlog completo em `docs/ISSUES_BACKLOG.md`.
- Foco sugerido: descoberta passiva (`robots/sitemap`) e exposicao de lideranca (CEO/CTO/CISO).
