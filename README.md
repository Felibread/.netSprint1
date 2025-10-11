# WeatherApp Acessível (Clean Architecture, .NET 8)

Aplicativo e API que fornecem informações de clima com foco em simplicidade, confiabilidade e acessibilidade. O projeto foi estruturado em Clean Architecture para facilitar manutenção, testes e evolução.

## 1. Definição do Projeto

- **Objetivo do Projeto**: Tornar a consulta do tempo simples e acessível para todas as pessoas, incluindo pessoas com deficiência. O app oferece previsão atual e geração de alertas inteligentes (ex.: aviso antecipado de chuva) com uma experiência inclusiva.
- **Problema**: Aplicativos atuais costumam ter interfaces complexas, excesso de dados, e pouca compatibilidade com tecnologias assistivas (leitores de tela, descrições de imagens, comandos de voz).
- **Solução**: API e camadas de domínio/aplicação que priorizam clareza, modelos fortes e regras de negócio para gerar previsões/alertas, prontas para serem consumidas por um cliente móvel ou web acessível.

### Escopo (MVP)
- Cadastro e busca de localidades.
- Consulta de leitura climática atual por localidade.
- Avaliação de alertas inteligentes (chuva, temperatura extrema) a partir da última leitura disponível.
- API documentada com Swagger.

### Requisitos Funcionais
- **RF1**: Cadastrar localidade informando nome, latitude e longitude.
- **RF2**: Buscar localidades por nome (prefixo/contém) com limite configurável.
- **RF3**: Obter leitura climática atual de uma localidade.
- **RF4**: Avaliar e retornar alertas sugeridos (chuva/temperatura extrema) para uma localidade.

### Requisitos Não Funcionais
- **RNF1**: Clean Architecture com baixo acoplamento entre camadas.
- **RNF2**: Persistência via EF Core (SQLite por padrão).
- **RNF3**: API REST com documentação Swagger/OpenAPI.
- **RNF4**: Código legível, validado e com tratamento de erros simples via `Result`.
- **RNF5**: Acessibilidade como diretriz de design para clientes que consumirem a API (descrições, textos claros, fluxos simples).

## 2. Arquitetura (Clean Architecture)

Camadas e responsabilidades:

- **Domain (`src/WeatherApp.Domain`)**
  - Entidades: `Location`, `WeatherReading`, `Alert`.
  - Value Objects: `Coordinates`, `Probability`.
  - Enums: `AlertType`, `TemperatureUnit`, `WeatherCondition`.
  - Serviços de domínio: `IAlertPolicyService`/`AlertPolicyService` (regras para gerar alertas).
  - Contratos de repositório: `ILocationRepository`, `IWeatherReadingRepository`.

- **Application (`src/WeatherApp.Application`)**
  - DTOs: `LocationDto`, `WeatherReadingDto`, `AlertDto`.
  - Serviços (casos de uso): `LocationService`, `WeatherService`, `AlertService`.
  - Abstração de UoW: `IUnitOfWork`.
  - Tipo utilitário de retorno: `Result<T>`.

- **Infrastructure (`src/WeatherApp.Infrastructure`)**
  - EF Core: `AppDbContext` e configurações de mapeamento (`Configurations/*`).
  - Repositórios concretos: `LocationRepository`, `WeatherReadingRepository`.
  - `UnitOfWork` para persistência.
  - Provider: SQLite por padrão (pode trocar por SQL Server/PostgreSQL alterando pacotes/UseXxx).

- **API (`src/WeatherApp.Api`)**
  - Minimal APIs com Swagger.
  - Injeta serviços, repositórios, DbContext e políticas de alerta.

Diagrama do banco (inspirado na imagem fornecida) foi traduzido para as entidades/valores acima. Nomes das colunas seguem convenções da imagem quando aplicável (`Localizacao`, `Clima`, `Alerta`).

## 3. Configuração e Execução

### Pré-requisitos
- .NET SDK 8 (o repositório inclui `dotnet-install.sh` para instalação local).

### Instalar SDK local (se necessário)
```bash
bash dotnet-install.sh --channel 8.0 --install-dir "$HOME/dotnet"
export PATH="$HOME/dotnet:$PATH"
```

### Restaurar e compilar
```bash
$HOME/dotnet/dotnet restore src/WeatherApp.sln
$HOME/dotnet/dotnet build src/WeatherApp.sln -c Debug
```

### Configurar conexão
A API usa SQLite por padrão. Configure em `src/WeatherApp.Api/appsettings.json`:
```json
{
  "ConnectionStrings": { "Default": "Data Source=weather.db" }
}
```

### Executar a API
```bash
$HOME/dotnet/dotnet run --project src/WeatherApp.Api/WeatherApp.Api.csproj --urls http://localhost:5187
```
Acesse a documentação: `http://localhost:5187/swagger`.

### Integração com OpenWeather
- Crie uma conta e obtenha a chave em `https://openweathermap.org/api`.
- Configure em `src/WeatherApp.Api/appsettings.json`:
```json
{
  "OpenWeather": { "ApiKey": "SUA_CHAVE" }
}
```
- Atualize a leitura atual a partir do OpenWeather e persista:
```bash
curl -X POST http://localhost:5187/api/weather/refresh/{locationId}
```

## 4. Endpoints Principais

- `GET /api/locations/search?q={texto}&limit={n}`: busca localidades.
- `POST /api/locations` body:
```json
{ "name": "São Paulo", "latitude": -23.55, "longitude": -46.63 }
```
- `GET /api/weather/current/{locationId}`: leitura atual.
- `GET /api/alerts/{locationId}`: avalia alertas a partir da leitura atual.

Respostas seguem o envelope de `Result` nas camadas internas, mas os endpoints retornam HTTP adequado (`200/201/400/404`).

## 5. Migrações de Banco de Dados

Este esqueleto já mapeia entidades, mas não inclui migrações geradas. Para criá-las:
```bash
$HOME/dotnet/dotnet tool install --global dotnet-ef --version 8.* || true
export PATH="$HOME/.dotnet/tools:$PATH"
$HOME/dotnet/dotnet ef migrations add Initial --project src/WeatherApp.Infrastructure/WeatherApp.Infrastructure.csproj --startup-project src/WeatherApp.Api/WeatherApp.Api.csproj --context WeatherApp.Infrastructure.Persistence.AppDbContext
$HOME/dotnet/dotnet ef database update --project src/WeatherApp.Infrastructure/WeatherApp.Infrastructure.csproj --startup-project src/WeatherApp.Api/WeatherApp.Api.csproj --context WeatherApp.Infrastructure.Persistence.AppDbContext
```

## 6. Acessibilidade (Diretrizes para o cliente)

- Linguagem clara, sem jargões e com hierarquia de informação.
- Suporte a leitores de tela: rótulos, descrições, foco visível.
- Contraste adequado e tamanhos de fonte ajustáveis.
- Notificações/alertas com textos objetivos e tempo configurável.
- Preferir comandos de voz/atalhos quando aplicável.

## 7. Próximos Passos
- Integrar cliente HTTP para provedor de clima externo (OpenWeather/Open-Meteo) com caching.
- Adicionar migrações e dados seed.
- Criar testes de unidade para serviços de domínio e aplicação.
- Acrescentar autenticação/limites de rate se necessário.

## 8. Estrutura de Pastas
```
src/
  WeatherApp.Domain/
  WeatherApp.Application/
  WeatherApp.Infrastructure/
  WeatherApp.Api/
```

## 9. Licença
MIT (ou a definir).
