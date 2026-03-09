# Backlog de Issues - Analise de Conteudo Publico

Este backlog descreve proximas demandas para ampliar o scanner com foco em dados publicos e coleta passiva.

## ISSUE-001 - Exposicao de lideranca (CEO, CTO, CISO)
- Tipo: feature
- Prioridade: alta
- Objetivo: identificar exposicao publica de dados de lideranca que aumentem risco de engenharia social.
- Escopo:
  - Coletar paginas `about`, `leadership`, `team`, `imprensa`.
  - Extrair nome, cargo, email publico e links sociais.
  - Sinalizar combinacao `nome + cargo + email direto`.
- Criterios de aceite:
  - Achado gerado com severidade LOW ou MEDIUM.
  - Evidencia inclui URL de origem e trecho sanitizado.

## ISSUE-002 - Descoberta passiva via robots.txt e sitemap.xml
- Tipo: feature
- Prioridade: alta
- Objetivo: ampliar descoberta de superficies publicas sem fuzzing agressivo.
- Escopo:
  - Ler `robots.txt` e `sitemap.xml`.
  - Extrair paths indexados e paths desautorizados.
  - Marcar paths sensiveis (`/admin`, `/backup`, `/internal`, `/old`).
- Criterios de aceite:
  - Lista de paths encontrada no relatorio.
  - Achado criado para path sensivel exposto publicamente.

## ISSUE-003 - Analise de TLS e certificado
- Tipo: feature
- Prioridade: media
- Objetivo: avaliar postura minima de criptografia do endpoint publico.
- Escopo:
  - Ler validade, emissor e cadeia do certificado.
  - Sinalizar certificado expirado ou perto de expirar.
  - Exibir resumo TLS por host.
- Criterios de aceite:
  - Achado MEDIUM para certificado invalido/expirado.
  - Relatorio inclui expiracao e emissor.

## ISSUE-004 - DNS publico e higiene de email (SPF/DMARC/DKIM)
- Tipo: feature
- Prioridade: media
- Objetivo: enriquecer contexto de risco com sinais DNS publicos.
- Escopo:
  - Consultar A, AAAA, CNAME, MX e TXT.
  - Validar presenca de SPF, DMARC e DKIM.
  - Alertar configuracao ausente ou incompleta.
- Criterios de aceite:
  - Relatorio apresenta resumo DNS.
  - Achado para ausencia de SPF/DMARC no dominio.

## ISSUE-005 - Inventario de scripts de terceiros
- Tipo: feature
- Prioridade: media
- Objetivo: mapear dependencia de terceiros no frontend.
- Escopo:
  - Extrair `script src` externos.
  - Classificar dominio (analytics, tag manager, ads, desconhecido).
  - Sinalizar dominio raro/desconhecido.
- Criterios de aceite:
  - Lista de third-parties no relatorio.
  - Achado LOW para origem nao confiavel.

## ISSUE-006 - Metadados e comentarios sensiveis no HTML
- Tipo: feature
- Prioridade: alta
- Objetivo: identificar vazamento de informacao em conteudo publico.
- Escopo:
  - Buscar comentarios HTML com palavras-chave (`todo`, `senha`, `token`, `internal`).
  - Detectar links para ambientes de homologacao/dev.
  - Detectar versoes expostas em metadados.
- Criterios de aceite:
  - Evidencia com trecho sanitizado.
  - Achado MEDIUM quando houver indicio de dado sensivel.

## ISSUE-007 - Fingerprint de stack front-end
- Tipo: feature
- Prioridade: baixa
- Objetivo: ampliar fingerprint alem de headers HTTP.
- Escopo:
  - Identificar React/Angular/Vue e bibliotecas comuns por marcadores.
  - Detectar versoes expostas em assets quando possivel.
  - Recomendar reducao de banner/fingerprint.
- Criterios de aceite:
  - Stack detectada com nivel de confianca.
  - Achado LOW para exposicao desnecessaria de versao.

## ISSUE-008 - Score por categoria de risco
- Tipo: enhancement
- Prioridade: media
- Objetivo: melhorar priorizacao de correcoes no relatorio final.
- Escopo:
  - Separar score por categoria: headers, fingerprint, DNS/TLS, conteudo publico.
  - Exibir score parcial e score total.
  - Permitir pesos configuraveis por categoria.
- Criterios de aceite:
  - Relatorio mostra score total + scores parciais.
  - Nao quebra contrato atual de `ScanReport`.

## ISSUE-009 - Detecao de leaks em arquivos publicos comuns
- Tipo: feature
- Prioridade: media
- Objetivo: identificar exposicao acidental em arquivos acessiveis publicamente.
- Escopo:
  - Verificar `/humans.txt`, `/security.txt`, `/ads.txt`, `/crossdomain.xml`.
  - Procurar referencias a emails internos, ambientes e endpoints legacy.
  - Gerar recomendacoes de higienizacao.
- Criterios de aceite:
  - Evidencias por arquivo encontradas no relatorio.
  - Achado LOW/MEDIUM quando houver dado sensivel.

## ISSUE-010 - Risco de brand impersonation (dominios relacionados)
- Tipo: discovery
- Prioridade: baixa
- Objetivo: mapear sinais publicos de possivel impersonacao de marca.
- Escopo:
  - Levantar subdominios e dominios relacionados em links publicos.
  - Sinalizar nomes suspeitos ou typosquatting obvio.
  - Classificar apenas como sinal informativo (sem bloqueio automatico).
- Criterios de aceite:
  - Relatorio inclui lista de dominios relacionados detectados.
  - Achado INFO quando houver padrao suspeito.
