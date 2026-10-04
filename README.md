# Plan Hackathon Copa 2026

Oct 3, 2026 · @Luis Alain

Objetivo: ganar el reto del Oct 9, 2026 en la sede Santiago resolviendo más problemas y más rápido que los demás equipos. La clave es dominar el troubleshooting en Azure con evidencia (App Insights, KQL, Dynatrace) y desplegar con Terraform.

## Lo que sabemos del reto

El stack es Azure + Dynatrace. Quedan 5 días de preparación (sábado 3 a miércoles 7).

| Dato               | Detalle                                                                                                         |
| ------------------ | --------------------------------------------------------------------------------------------------------------- |
| Sede               | Hotel Gran David, Salón Villa Magna, Santiago de Veraguas                                                       |
| Fechas             | Jue 8 oct: charlas · Vie 9 oct: competencia                                                                     |
| Tema oficial       | DevOps, APIs y Observabilidad                                                                                   |
| Organizan          | Copa Airlines, Microsoft, Dynatrace, Business IT, Ciudad del Saber                                              |
| Equipo             | Exactamente 4 personas, presencial                                                                              |
| Premio (1er lugar) | Boletos ida y vuelta + hospedaje para los 4, y entradas a un congreso técnico regional de Microsoft o Dynatrace |
| Extra              | Finalistas pueden acceder a pasantías y empleos con empresas patrocinadoras                                     |

**Pistas del formato.** El taller 5 trató Health Checks, variables de configuración, CORS, errores HTTP 500 y diagnóstico basado en evidencia. El día 1 incluye "Del código a la nube" (.NET APIs, frontend, Git, Azure DevOps, Terraform, recursos Azure) y "Operación bajo presión: de los logs a la solución". En la edición anterior hubo tablero de puntuación en vivo.

**Inferencia:** los retos serán apps rotas en Azure que hay que diagnosticar y arreglar con evidencia, más despliegues con Terraform/pipelines. Gana quien resuelve más retos, más rápido.

**Recursos oficiales (usar todos):**

- 6 workshops grabados: Azure, APIs, DevOps + Terraform, Observabilidad + KQL, Troubleshooting en Azure, Dynatrace ([workshops](https://hackathoncopa.com/workshops))
- Cápsulas Informativas Microsoft: 17 cápsulas de Azure, App Service, IaC, KQL y diagnóstico ([recursos](https://hackathoncopa.com/recursos))
- Guía del Estudiante Dynatrace + curso Dynatrace Essentials (9 lecciones, \~4 h 30 min)
- PDF "Conceptos Clave de Observabilidad en Dynatrace"

## Roles del equipo

Todos aprenden lo básico de todo; cada uno es **el experto** de su área y el primero en atacar los retos de ese tipo. Asignar hoy mismo.

| Rol                 | Domina                                                               | En la competencia                    | Persona |
| ------------------- | -------------------------------------------------------------------- | ------------------------------------ | ------- |
| Azure / App Service | Portal, `az` CLI, App Settings, CORS, Health Check, Log stream, Kudu | Arregla configuración y despliegues  |         |
| Observabilidad      | App Insights, KQL, Dynatrace (DQL, servicios, trazas, Problems)      | Encuentra la causa con evidencia     |         |
| IaC / Pipelines     | Terraform (azurerm), Azure DevOps Pipelines, Git                     | Despliega y corrige infraestructura  |         |
| Código              | C# / .NET APIs, frontend JS, HTTP                                    | Corrige bugs que los demás localizan |         |

El rol de Observabilidad es el que más puntos decide!

## Guías por tema

El calendario de abajo dice **cuándo**; las guías dicen **qué hacer**, paso a paso, con pistas y criterios de "listo cuando". Cada uno construye su propio lab en `lab/` siguiendo las guías.

| Guía                                                                     | Tema                                         | Experto            | Cuándo           |
| ------------------------------------------------------------------------ | -------------------------------------------- | ------------------ | ---------------- |
| [00 · Setup](guias/00-setup.md)                                          | Herramientas, cuentas, límites de la suscripción | Todos          | Sábado, primero  |
| [01 · Azure y App Service](guias/01-azure-app-service.md)                | Desplegar API + frontend, CORS, diagnóstico  | Azure              | Sábado           |
| [02 · Terraform](guias/02-terraform.md)                                  | Lab completo como código, drift, errores     | IaC / Pipelines    | Domingo          |
| [03 · Git y Azure DevOps](guias/03-azure-devops-pipelines.md)            | Pipeline build + deploy, pipelines rotos     | IaC / Pipelines    | Domingo          |
| [04 · App Insights y KQL](guias/04-observabilidad-app-insights-kql.md)   | Telemetría, consultas, síntoma vs causa      | Observabilidad     | Lunes            |
| [05 · Dynatrace](guias/05-dynatrace.md)                                  | Essentials, Problems, trazas, DQL            | Observabilidad     | Martes           |
| [06 · Troubleshooting](guias/06-troubleshooting.md)                      | Romper y arreglar en parejas, cronometrado   | Todos              | Lunes y martes   |
| [07 · Simulacro](guias/07-simulacro.md)                                  | Ensayo de competencia + logística            | Todos              | Miércoles        |

## Setup previo (hoy, 1 h)

Sin esto no se practica nada. Cada miembro del equipo lo completa en su laptop. Detalle en [guias/00-setup.md](guias/00-setup.md).

> **Hacer hoy:** las organizaciones nuevas de Azure DevOps no tienen agentes hospedados gratis hasta que Microsoft aprueba una solicitud de paralelismo (2–3 días hábiles). Además, Azure for Students limita las regiones donde se pueden crear recursos: hay que descubrir cuál funciona antes de practicar.

**Cuentas**

- [x] Azure for Students activado (crédito gratis con correo universitario, sin tarjeta)
- [x] Organización en Azure DevOps (dev.azure.com) con un proyecto de práctica
- [ ] Solicitud de paralelismo gratis de Azure DevOps enviada
- [x] Dynatrace Playground + Dynatrace University (seguir la Guía del Estudiante)
- [ ] Grupo de chat del equipo + repo compartido en GitHub para chuletas y lab

**Herramientas instaladas**

- [x] VS Code + extensiones: Azure Tools, HashiCorp Terraform, C# Dev Kit
- [x] .NET SDK 10 (`dotnet --list-sdks`); confirmar que App Service ofrece `DOTNETCORE:10.0` en la región
- [ ] Azure CLI (`az login` funcionando)
- [ ] Terraform (`terraform -version`)
- [x] Git + Docker Desktop
- [x] Node.js (para el frontend)

**Verificación rápida:** `az account show` muestra la suscripción correcta, `terraform init` corre en una carpeta vacía con el provider azurerm y se conoce una región donde se pueden crear recursos.

## Sábado 3 — Azure + App Service (5–6 h)

Meta del día: desplegar una API .NET y un frontend en App Service y saber dónde mirar cuando fallan.

**Guía:** [01 · Azure y App Service](guias/01-azure-app-service.md)

**Repasar**

- Workshop 1 (Fundamentos de Azure) y Workshop 2 (APIs, frontend y backend) a 1.5x
- Cápsulas Microsoft sobre Azure y App Service

**Aprender**

- Jerarquía: suscripción → resource group → recurso
- App Service Plan vs Web App; tiers (F1/B1); Linux vs Windows
- App Settings y Connection Strings: cómo llegan como variables de entorno
- Startup command, puerto (`WEBSITES_PORT`), runtime stack
- Herramientas de diagnóstico: Log stream, Diagnose and solve problems, Kudu (`.scm.azurewebsites.net`), Health check
- CORS en App Service vs CORS en el código de la API
- Lectura básica de C#: `Program.cs`, controllers/minimal APIs, `appsettings.json`, inyección de configuración

**Practicar**

- [ ] Crear una minimal API .NET (`dotnet new webapi`) con `/health` y un endpoint que lea una variable de entorno
- [ ] Desplegarla desde el portal y luego con `az webapp up`
- [ ] Crear un frontend estático que llame a la API y desplegarlo (Static Web App o segunda Web App)
- [ ] Provocar y resolver un error de CORS entre ambos
- [ ] Cambiar un App Setting y ver el efecto en vivo
- [ ] Ver logs en Log stream y entrar a Kudu

**Listo cuando:** puedes desplegar la API desde cero en menos de 15 min sin mirar notas.

## Domingo 4 — Terraform + Azure DevOps (5–6 h)

Meta del día: levantar toda la infraestructura del lab con Terraform y desplegar la API con un pipeline.

**Guías:** [02 · Terraform](guias/02-terraform.md) y [03 · Git y Azure DevOps](guias/03-azure-devops-pipelines.md)

**Repasar**

- Workshop 3 (DevOps, Git, GitHub y Terraform)
- Cápsulas Microsoft sobre IaC

**Aprender**

- Flujo: `init` → `fmt`/`validate` → `plan` → `apply` → `destroy`
- Provider `azurerm` y bloque `features {}`; autenticación con `az login`
- Recursos clave: `azurerm_resource_group`, `azurerm_service_plan`, `azurerm_linux_web_app`, `azurerm_application_insights`, `azurerm_log_analytics_workspace`
- `app_settings` y `site_config` dentro de la Web App (aquí se rompen muchas cosas)
- Variables, `terraform.tfvars`, outputs; qué es el state y por qué no se edita a mano
- Leer un `plan`: `+` crear, `~` modificar, `-/+` reemplazar
- Errores típicos: nombre global ya usado, región sin cuota, provider sin registrar, drift entre portal y código
- Azure DevOps: repos, `azure-pipelines.yml`, stages/jobs/steps, service connection, tareas `DotNetCoreCLI` y `AzureWebApp`

**Practicar**

- [ ] Escribir Terraform para: Resource Group + Service Plan + Linux Web App (.NET 10) + Log Analytics + Application Insights conectado por `APPLICATIONINSIGHTS_CONNECTION_STRING`
- [ ] `apply`, verificar en el portal, `destroy`, volver a `apply`
- [ ] Cambiar un `app_setting` en el portal y ver el drift en `plan`
- [ ] Romper el código Terraform a propósito (tipo incorrecto, referencia mala) y leer el error
- [ ] Crear un pipeline en Azure DevOps: build → publish → deploy a la Web App
- [ ] Hacer fallar el pipeline (ruta de proyecto mal) y diagnosticar en los logs del job

**Listo cuando:** el lab completo se crea con un solo `terraform apply` y un push dispara el despliegue.

## Lunes 5 — Observabilidad en Azure (6 h, el día más importante)

Meta del día: encontrar la causa de cualquier fallo de la app en menos de 10 minutos usando solo evidencia.

**Guías:** [04 · App Insights y KQL](guias/04-observabilidad-app-insights-kql.md) y [06 · Troubleshooting](guias/06-troubleshooting.md)

**Repasar**

- Workshop 4 (Observabilidad, Application Insights, Logs y KQL)
- Workshop 5 (Troubleshooting de aplicaciones en Azure) — verlo dos veces, es el más cercano a los retos
- Cápsulas Microsoft sobre KQL y diagnóstico

**Aprender**

- Tres pilares: métricas, logs, trazas; y cómo se conectan por `operation_Id`
- Tablas de App Insights: `requests`, `exceptions`, `dependencies`, `traces`, `customEvents`
- KQL: `where`, `project`, `summarize`, `count()`, `percentile()`, `bin()`, `order by`, `take`, `join`, `render timechart`
- Vistas del portal: Failures, Performance, Application Map, Live Metrics, Transaction search
- Diferencia entre síntoma (500, lentitud) y causa (excepción, dependencia caída, config faltante)

**Practicar: ejercicios de romper y arreglar**

Un compañero rompe la app sin decir qué; el otro la diagnostica con evidencia y cronometra.

- [ ] Borrar un App Setting que la API necesita → 500 en un endpoint
- [ ] Connection string con contraseña o host incorrecto → dependencia fallando
- [ ] Quitar el origen del frontend de CORS → error en la consola del navegador
- [ ] Ruta de health check incorrecta → instancia marcada como no saludable
- [ ] Startup command o puerto incorrecto → la app no arranca (503)
- [ ] Excepción no controlada en el código → aparece en `exceptions`
- [ ] Endpoint lento (delay artificial) → detectarlo con `percentile(duration, 95)`
- [ ] App Insights desconectado (connection string vacía) → no llega telemetría

**Listo cuando:** cada miembro resuelve 3 fallos al azar en menos de 10 min cada uno y tienes 10 consultas KQL guardadas en la chuleta.

## Martes 6 — Dynatrace (5–6 h)

Meta del día: navegar Dynatrace con soltura y consultar logs con DQL. Dynatrace es organizador: casi seguro habrá retos sobre él.

**Guía:** [05 · Dynatrace](guias/05-dynatrace.md)

**Repasar**

- Workshop 6 (Dynatrace Playground, University y Essentials)
- PDF "Conceptos Clave de Observabilidad en Dynatrace"

**Aprender**

- Curso Dynatrace Essentials completo (9 lecciones, \~4 h 30 min) en Dynatrace University
- Navegación: Hosts/Infraestructura (VMs y Kubernetes), Services, Distributed Traces, Logs, Problems
- Davis AI: cómo agrupa síntomas en un Problem y señala la causa raíz
- Smartscape: dependencias entre servicios, procesos y hosts
- DQL: `fetch logs`, `filter`, `fields`, `summarize`, `sort`, `limit`, `makeTimeseries`
- Temas de la charla del día 1: Application Security, User Experience (RUM), Business Observability

**Practicar (en el Playground)**

- [ ] Encontrar un Problem abierto y explicar su causa raíz en 2 frases
- [ ] Seguir una traza distribuida de punta a punta e identificar el span más lento
- [ ] Escribir 5 consultas DQL: errores por servicio, logs por nivel, top mensajes de error, conteo por hora, filtro por texto
- [ ] Comparar el mismo diagnóstico en Dynatrace y en App Insights

**Listo cuando:** puedes ir de "la app falla" a "este servicio, este error, desde esta hora" en Dynatrace en menos de 5 min.

## Miércoles 7 — Simulacro + logística (4 h + descanso)

Meta del día: ensayar la competencia como equipo y llegar descansados. Nada nuevo se aprende hoy.

**Guía:** [07 · Simulacro](guias/07-simulacro.md)

**Simulacro (90 min, cronometrado)**

- [ ] Una persona prepara 5 fallos distintos en el lab (mezcla de config, código, Terraform y pipeline)
- [ ] Los otros tres los resuelven en paralelo aplicando los roles y el método de diagnóstico
- [ ] Registrar tiempo por fallo y dónde se perdió tiempo
- [ ] Revisión de 20 min: qué cambiamos para el viernes

**Cerrar materiales**

- [ ] Chuleta final unificada (esta doc, sección Chuletas) en el teléfono y descargada offline
- [ ] Repo del lab con el Terraform y la API funcionando, por si sirve de referencia

**Logística**

- [ ] Laptops cargadas, con todas las herramientas probadas y sesiones de `az login` y Dynatrace activas
- [ ] Regleta, extensiones, cargadores, cédula
- [ ] Plan de llegada al Hotel Gran David antes de las 8:00
- [ ] Dormir 8 horas

## Jueves 8 — Día de charlas

Las charlas son el mapa de los retos del viernes. Tratarlas como clase con examen al día siguiente.

- Dividir el equipo: dos toman notas en las charlas técnicas, dos anotan nombres de herramientas, comandos y errores que mencionen
- Prestar máxima atención a "Del código a la nube" (11:15) y "Operación bajo presión" (4:15): replican el flujo de los retos
- Preguntar a los mentores de Microsoft y Dynatrace sobre lo que más dudas dejó el simulacro
- "Instrucciones para el día 2" (5:30): anotar reglas de puntuación, límites de tiempo y entregables exactos
- En la noche: 30 min de repaso de la chuleta con lo nuevo de las charlas, y a dormir

## Viernes 9 — Estrategia y tácticas de competencia

Gana el equipo que acumula más puntos, no el que resuelve el reto más difícil. Velocidad + orden + cero bloqueos.

**Primeros 15 minutos**

1. Leer **todos** los retos en voz alta, con puntos y dificultad.
2. Clasificar cada uno: rápido (< 15 min), medio, difícil.
3. Repartir por rol y empezar por los rápidos de mayor puntaje.

**Método en cada problema**

1. **Síntoma:** reproducir el fallo (navegador, `curl`, estado del pipeline).
2. **Evidencia:** Log stream, App Insights Failures/KQL, Dynatrace Problems/DQL, logs del job.
3. **Hipótesis:** una sola, la que explica la evidencia.
4. **Fix:** el cambio mínimo (App Setting, código, Terraform).
5. **Verificación:** reproducir de nuevo y confirmar en la telemetría que el error desapareció.
6. **Registro:** captura + una línea de qué era y cómo se arregló.

**Reglas del equipo**

- Regla de 20 minutos: atascado = rotar el reto o pedir mentor
- Un solo dueño por recurso: nadie cambia lo que otro está tocando sin avisar
- Cambios en infraestructura por Terraform si el reto lo exige; si no, el portal es más rápido
- Checkpoint cada hora: puntos actuales, qué falta, reasignar
- Comer y tomar agua: el cerebro cansado comete errores tontos

**Si hay presentación o jurado**

- Mostrar el antes/después con evidencia (gráfica de errores cayendo, consulta KQL/DQL)
- Explicar la causa raíz, no solo el fix
- Mencionar cómo se evitaría en el futuro (alerta, health check, validación en el pipeline)

## Runbook de troubleshooting

Los fallos más probables, ordenados por frecuencia en apps de App Service. Usar como primera hipótesis.

| Síntoma                                    | Dónde mirar primero                                          | Causa probable                                                      | Fix                                                               |
| ------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------------------- | ----------------------------------------------------------------- |
| HTTP 500 en un endpoint                    | App Insights → Failures / `exceptions`                       | App Setting faltante, null reference, config mal leída              | Agregar/corregir App Setting; arreglar el código                  |
| HTTP 503 / la app no arranca               | Log stream, Diagnose and solve problems                      | Startup command, puerto, runtime stack incorrecto                   | Corregir `site_config` o startup command; `WEBSITES_PORT` (solo contenedores) |
| Error CORS en el navegador                 | Consola del navegador (F12), configuración CORS              | Origen del frontend no permitido                                    | Agregar el origen en App Service CORS o en el código, no en ambos |
| Frontend no llega a la API                 | Network tab, variable de URL de la API                       | URL de API vieja o mal escrita en el frontend                       | Corregir la variable y redesplegar                                |
| Instancia "unhealthy"                      | Health check en el portal, `requests` a la ruta              | Ruta de health check incorrecta o endpoint que falla                | Corregir la ruta o el endpoint `/health`                          |
| Dependencia fallando (BD, API externa)     | App Insights → `dependencies`, Application Map               | Connection string incorrecta, firewall, host caído                  | Corregir connection string; permitir IP/servicio                  |
| Respuestas lentas                          | Performance, `percentile(duration, 95)`, trazas en Dynatrace | Consulta lenta, dependencia lenta, plan pequeño                     | Optimizar o escalar el plan                                       |
| No llega telemetría                        | App Settings, Live Metrics                                   | `APPLICATIONINSIGHTS_CONNECTION_STRING` vacía o errónea             | Configurar la connection string correcta                          |
| `terraform apply` falla                    | Mensaje de error del plan                                    | Nombre global repetido, provider no registrado, tipo inválido       | Cambiar nombre/sufijo; `az provider register`; corregir tipo      |
| Drift: el portal no coincide con Terraform | `terraform plan`                                             | Alguien cambió algo a mano                                          | Decidir la fuente de verdad y aplicar                             |
| Pipeline falla                             | Logs del job en Azure DevOps                                 | Ruta del proyecto, versión de .NET, service connection sin permisos | Corregir YAML o permisos                                          |
| Problem abierto en Dynatrace               | Problems → causa raíz de Davis                               | El servicio o proceso que Davis señala                              | Seguir la evidencia hasta el componente y arreglarlo              |

## Chuletas

Comandos y consultas para copiar el día de la competencia. Reemplazar `<app>` y `rg-hack` por los nombres reales.

### Azure CLI

```bash
az login
az account set --subscription "<subscription-id>"
az group create -n rg-hack -l eastus

# Desplegar rápido una API .NET
az webapp up --name <app> --resource-group rg-hack --runtime "DOTNETCORE:10.0" --sku B1

# Diagnóstico
az webapp log tail -n <app> -g rg-hack
az webapp show -n <app> -g rg-hack --query state
az webapp config show -n <app> -g rg-hack --query "{stack:linuxFxVersion, health:healthCheckPath, startup:appCommandLine}"
az webapp config appsettings list -n <app> -g rg-hack -o table

# Arreglos
az webapp config appsettings set -n <app> -g rg-hack --settings KEY=value
az webapp cors add -n <app> -g rg-hack --allowed-origins https://<frontend>.azurewebsites.net
az webapp restart -n <app> -g rg-hack
```

### Terraform (lab base, azurerm 4.x)

```hcl
terraform {
  required_providers {
    azurerm = { source = "hashicorp/azurerm", version = "~> 4.0" }
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
}

variable "subscription_id" {}
variable "prefix" { default = "hackcopa" }

resource "azurerm_resource_group" "rg" {
  name     = "rg-${var.prefix}"
  location = "eastus"
}

resource "azurerm_log_analytics_workspace" "law" {
  name                = "law-${var.prefix}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
}

resource "azurerm_application_insights" "ai" {
  name                = "ai-${var.prefix}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  workspace_id        = azurerm_log_analytics_workspace.law.id
  application_type    = "web"
}

resource "azurerm_service_plan" "plan" {
  name                = "plan-${var.prefix}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_linux_web_app" "api" {
  name                = "api-${var.prefix}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.plan.id

  site_config {
    application_stack { dotnet_version = "10.0" }
    health_check_path                 = "/health"
    health_check_eviction_time_in_min = 2
    cors { allowed_origins = ["https://front-${var.prefix}.azurewebsites.net"] }
  }

  app_settings = {
    APPLICATIONINSIGHTS_CONNECTION_STRING = azurerm_application_insights.ai.connection_string
  }
}

output "api_url" { value = "https://${azurerm_linux_web_app.api.default_hostname}" }
```

```bash
terraform init && terraform fmt && terraform validate
terraform plan -out tfplan
terraform apply tfplan
terraform state list
terraform destroy
```

### KQL (Application Insights)

En Log Analytics las mismas tablas se llaman `AppRequests`, `AppExceptions`, `AppDependencies`, `AppTraces`.

```kql
// Errores por endpoint y código
requests | where timestamp > ago(1h) and success == false
| summarize count() by name, resultCode | order by count_ desc

// Excepciones más frecuentes
exceptions | where timestamp > ago(1h)
| summarize count() by type, outerMessage | order by count_ desc

// Últimas excepciones con su operation_Id
exceptions | order by timestamp desc | take 20
| project timestamp, type, outerMessage, operation_Id

// Dependencias fallando (BD, APIs externas)
dependencies | where success == false
| summarize count() by target, type, resultCode

// Endpoints más lentos (p95)
requests | summarize p95 = percentile(duration, 95) by name | order by p95 desc

// Errores en el tiempo
requests | summarize count() by bin(timestamp, 5m), success | render timechart

// Logs de error de la app
traces | where severityLevel >= 3 | order by timestamp desc | take 50

// Todo lo que pasó en una petición
union requests, dependencies, exceptions, traces
| where operation_Id == "<operation-id>" | order by timestamp asc
```

### DQL (Dynatrace)

Los nombres de campos varían por entorno: confirmarlos en el Playground el martes y corregir aquí.

```dql
// Últimos errores
fetch logs | filter loglevel == "ERROR" | sort timestamp desc | limit 50

// Conteo por nivel
fetch logs | summarize count(), by:{loglevel}

// Buscar texto en los logs
fetch logs | filter contains(content, "Exception") | fields timestamp, content | limit 20

// Errores en el tiempo
fetch logs | filter loglevel == "ERROR" | makeTimeseries count(), interval:5m

// Problems de Davis
fetch dt.davis.problems | sort timestamp desc | limit 20
```

## Fuentes

- [Hackathon Copa 2026 — Inicio](https://hackathoncopa.com/)
- [Agenda](https://hackathoncopa.com/agenda)
- [Workshops de preparación](https://hackathoncopa.com/workshops)
- [Recursos de preparación](https://hackathoncopa.com/recursos)
- [Preguntas frecuentes](https://hackathoncopa.com/faqs)
- [Formato de la edición 2025 (tablero en vivo)](https://somosimpactopositivo.com/panama-en-hackathon-copa-airlines-aws/)
