# Issues de backlog - novas analises de conteudo publico

## ISSUE 001 - Perfil executivo publico (CEO e lideranca)
- Objetivo: mapear dados publicos de executivos (CEO, CTO, CISO) expostos no site e correlacionar risco de engenharia social.
- Escopo inicial:
  - Coletar paginas "Sobre", "Leadership", "Team", "Imprensa".
  - Extrair nomes, cargos, emails publicos e links para redes sociais.
  - Gerar alerta de risco quando houver combinacao "nome + cargo + email direto".
- Criterios de aceite:
  - Scanner retorna achado com severidade LOW/MEDIUM para exposicao excessiva.
  - Evidencia inclui URL e trecho encontrado.

## ISSUE 002 - Descoberta de superfícies publicas via robots.txt e sitemap.xml
- Objetivo: ampliar descoberta de endpoints publicos sem fuzzing agressivo.
- Escopo inicial:
  - Ler `/robots.txt` e `/sitemap.xml`.
  - Extrair caminhos indexados e caminhos bloqueados.
  - Sinalizar pastas sensiveis citadas (ex.: `/admin`, `/backup`, `/internal`).
- Criterios de aceite:
  - Lista de paths descobertos anexada ao relatorio.
  - Achado criado para caminhos sensiveis publicamente referenciados.

## ISSUE 003 - Analise de TLS e certificado
- Objetivo: avaliar maturidade minima de criptografia no endpoint publico.
- Escopo inicial:
  - Validade do certificado (expiracao e emissor).
  - Compatibilidade TLS minima e cadeia de certificado.
  - Sinalizar certificado perto de expirar.
- Criterios de aceite:
  - Relatorio inclui resumo TLS por host.
  - Achado MEDIUM para certificado expirado ou invalido.

## ISSUE 004 - DNS publico e reputacao basica do dominio
- Objetivo: enriquecer contexto com sinais publicos de DNS.
- Escopo inicial:
  - Consultar A/AAAA/CNAME/MX/TXT.
  - Verificar SPF/DMARC/DKIM quando aplicavel.
  - Alertar ausencia de politicas minimas de email.
- Criterios de aceite:
  - Relatorio lista registros relevantes.
  - Achado para SPF/DMARC ausente no dominio raiz.

## ISSUE 005 - Inventario de scripts de terceiros
- Objetivo: identificar dependencia de scripts externos e risco de supply chain.
- Escopo inicial:
  - Extrair `script src` externos do HTML.
  - Classificar por dominio (analytics, tag manager, ads, desconhecido).
  - Sinalizar dominios raros ou nao categorizados.
- Criterios de aceite:
  - Relatorio mostra lista de third-parties detectados.
  - Achado LOW quando houver scripts de origem nao confiavel.

## ISSUE 006 - Exposicao de metadados e comentarios sensiveis
- Objetivo: detectar vazamento de informacao em HTML publico.
- Escopo inicial:
  - Buscar comentarios HTML com palavras-chave (`todo`, `senha`, `token`, `internal`).
  - Detectar metadados com versao de plataforma.
  - Sinalizar links para ambientes de homologacao/dev.
- Criterios de aceite:
  - Evidencia no relatorio com trecho sanitizado.
  - Achado MEDIUM quando encontrar credenciais aparentes.

## ISSUE 007 - Fingerprint de tecnologias front-end
- Objetivo: ampliar fingerprint publico alem de headers HTTP.
- Escopo inicial:
  - Identificar frameworks por marcadores no HTML/JS (React, Angular, Vue etc.).
  - Sinalizar versoes expostas em arquivos estaticos quando disponiveis.
  - Gerar recomendacao de reducao de banner/fingerprint.
- Criterios de aceite:
  - Relatorio com stack detectada e confianca.
  - Achado LOW para exposicao desnecessaria de versao.

## ISSUE 008 - Score de risco por categoria
- Objetivo: separar score tecnico em categorias para facilitar priorizacao.
- Escopo inicial:
  - Categorias: headers, fingerprint, DNS/TLS, conteudo publico.
  - Exibir score parcial por categoria.
  - Definir pesos configuraveis por ambiente.
- Criterios de aceite:
  - Relatorio apresenta score total e score por categoria.
  - Mudanca nao quebra contratos atuais de `ScanReport`.
