# WeatherApp Acessível (Clean Architecture + ASP.NET Core MVC) - Sprint 2

Aplicação .NET 8 que combina uma camada Web MVC com uma Web API hipertextual para monitoramento de clima, gestão de localidades e geração de alertas inteligentes. A solução segue Clean Architecture para manter o domínio independente da infraestrutura.

## INTEGRANTES
Natália Santos - RM560306
Alex ribeiro - RM557356
Felipe damasceno -RM559433

## 1. Visão Geral do Projeto

- **Objetivo**: disponibilizar consultas meteorológicas simples, acessíveis e confiáveis, com regras de negócio para emissão de alertas inteligentes (chuva, temperatura extrema).
- **Camadas entregues nesta sprint**:
  - UI MVC com layout responsivo em Bootstrap 5, rotas personalizadas e validação de ViewModels.
  - Web API com contratos REST, paginação, filtros, ordenação e navegação HATEOAS.
  - Serviços de aplicação que encapsulam regras e tratativas (`Result`/`PagedResult`).

### Funcionalidades Principais
- CRUD completo de localidades (listas com filtros, detalhes, edição e exclusão).
- Consulta de leitura climática atual por localidade e exibição amigável na UI.
- Avaliação dinâmica de alertas baseada em políticas de domínio.
- Endpoints de pesquisa para cada domínio (Locations, WeatherReadings, Alerts) com metadados de paginação e links hipertexto.

## 2. Arquitetura em Camadas

- **Domain (`src/WeatherApp.Domain`)**
  - Entidades (`Location`, `WeatherReading`, `Alert`) e Value Objects (`Coordinates`, `Probability`).
  - Serviços de domínio (`IAlertPolicyService`) responsáveis por regras de alerta.
  - Repositórios abstratos para Localizações, Leituras e Alertas.

- **Application (`src/WeatherApp.Application`)**
  - DTOs e comandos (`LocationDto`, `WeatherReadingDto`, `AlertDto`, `WeatherReadingUpsertDto`, `AlertUpsertDto`).
  - Casos de uso (`LocationService`, `WeatherService`, `AlertService`) com paginação e validações.
  - Resultados padronizados (`Result<T>`, `PagedResult<T>`) e `IUnitOfWork`.

- **Infrastructure (`src/WeatherApp.Infrastructure`)**
  - EF Core + SQLite (`AppDbContext`) com mapeamentos fluentes.
  - Repositórios concretos (`LocationRepository`, `WeatherReadingRepository`, `AlertRepository`) e `UnitOfWork`.

- **Web (`src/WeatherApp.Api`)**
  - Controllers MVC para Home/Locations com layouts, formulários e validações.
  - Controllers API (`LocationsController`, `WeatherReadingsController`, `AlertsController`) com rotas versionadas e respostas HATEOAS.
  - Swagger configurado para documentação automática das rotas.

## 3. Configuração e Execução

### Pré-requisitos
- .NET SDK 8 (use `dotnet-install.sh` caso não possua o SDK localmente).

```bash
bash dotnet-install.sh --channel 8.0 --install-dir "$HOME/dotnet"
export PATH="$HOME/dotnet:$PATH"
```

### Restaurar e Compilar

```bash
$HOME/dotnet/dotnet restore src/WeatherApp.sln
$HOME/dotnet/dotnet build src/WeatherApp.sln -c Debug
```

> Observação: em ambientes sem acesso à internet o `restore` pode exigir cache prévio dos pacotes NuGet. Caso encontre erros de rede, execute os comandos acima em um ambiente conectado para preencher o cache local.

### Configurar a Conexão

`appsettings.json` já aponta para SQLite (`weather.db`). Ajuste se desejar outro provider:

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=weather.db"
  }
}
```

### Executar

```bash
$HOME/dotnet/dotnet run --project src/WeatherApp.Api/WeatherApp.Api.csproj --urls http://localhost:5187
(pode ser outra porta, basta substituit pela certa)
```

- UI MVC: `http://localhost:5187`
- Swagger UI: `http://localhost:5187/swagger`

## 4. Camada Web (MVC)

- Rotas convencionais + rotas personalizadas (`/locais`, `/locais/detalhes/{id}`, `/sobre`).
- Layout `_Layout` com cabeçalho, navegação e rodapé responsivos.
- Views fortemente tipadas com `ViewModels` e validação via DataAnnotations + jQuery Validation.
- Listagem paginada de localidades com filtros de nome/latitude/longitude e ordenação.
- Tela de detalhes exibindo leitura meteorológica atual e alertas gerados em tempo real.

## 5. Web API com HATEOAS

Rotas expostas (todas com suporte a paginação, ordenação `sortBy`, `ascending` e filtros específicos):

- `GET /api/locations/search`
- `POST /api/locations`
- `PUT /api/locations/{id}`
- `DELETE /api/locations/{id}`
- `GET /api/weather-readings/search`
- `GET /api/weather-readings/current/{locationId}`
- `POST /api/weather-readings`
- `PUT /api/weather-readings/{id}`
- `DELETE /api/weather-readings/{id}`
- `GET /api/alerts/search`
- `GET /api/alerts/evaluate/{locationId}`
- `POST /api/alerts`
- `PUT /api/alerts/{id}`
- `DELETE /api/alerts/{id}`

As respostas de busca retornam objeto `PagedResponse<T>` com:

```json
{
  "data": [ /* itens */ ],
  "pagination": { "pageNumber": 1, "pageSize": 10, "totalItems": 42, "totalPages": 5 },
  "links": [
    { "rel": "self", "href": "...", "method": "GET" },
    { "rel": "next", "href": "...", "method": "GET" }
  ]
}
```

## 6. Migrações de Banco de Dados

Para criar/aplicar migrações:

```bash
$HOME/dotnet/dotnet tool install --global dotnet-ef --version 8.* || true
export PATH="$HOME/.dotnet/tools:$PATH"
$HOME/dotnet/dotnet ef migrations add Initial \
  --project src/WeatherApp.Infrastructure/WeatherApp.Infrastructure.csproj \
  --startup-project src/WeatherApp.Api/WeatherApp.Api.csproj \
  --context WeatherApp.Infrastructure.Persistence.AppDbContext
$HOME/dotnet/dotnet ef database update \
  --project src/WeatherApp.Infrastructure/WeatherApp.Infrastructure.csproj \
  --startup-project src/WeatherApp.Api/WeatherApp.Api.csproj \
  --context WeatherApp.Infrastructure.Persistence.AppDbContext
```

## 7. Acessibilidade

- Componentes com rótulos semânticos e feedback visual/sonoro.
- Bootstrap 5 e ícones pensados para contraste adequado.
- Mensagens curtas e objetivas para erros/sucessos (`TempData`).

## 8. Próximos Passos

- Integrar provedor externo de clima e persistir histórico de leituras.
- Criar testes automatizados (xUnit) para serviços e políticas de domínio.
- Implementar autenticação/autorizações e políticas de rate limiting.
- Gerar migrações iniciais e dados seed para facilitar demonstrações.

## 9. Estrutura de Pastas

```
src/
  WeatherApp.Domain/
  WeatherApp.Application/
  WeatherApp.Infrastructure/
  WeatherApp.Api/   <-- MVC + Web API + Views Razor
```

