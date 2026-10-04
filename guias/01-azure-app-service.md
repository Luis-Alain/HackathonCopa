# 01 · Azure y App Service

**Objetivo:** desplegar una API .NET y un frontend en Azure, y saber dónde mirar cuando fallan.

**Rol experto:** Azure / App Service · **Tiempo estimado:** 5–6 h

**Material:** Workshop 1 (Fundamentos de Azure), Workshop 2 (APIs, frontend y backend), cápsulas de Azure y App Service.

---

## Conceptos que debes poder explicar

Antes de practicar, responde cada pregunta con tus palabras. Si no puedes, vuelve al workshop.

- [ ] ¿Qué relación hay entre suscripción, resource group y recurso?
- [ ] ¿Qué diferencia hay entre un **App Service Plan** y una **Web App**? ¿Quién paga?
- [ ] ¿Qué incluye el tier F1 frente al B1? ¿Por qué Linux y no Windows?
- [ ] ¿Cómo llega un **App Setting** al código .NET? ¿Qué pasa con una clave como `Seccion__Clave`?
- [ ] ¿Qué es el **runtime stack** y qué es el **startup command**?
- [ ] ¿Para qué sirve `WEBSITES_PORT` y por qué normalmente **no** hace falta con el stack .NET integrado?
- [ ] ¿Qué es Kudu y cómo se entra (`<app>.scm.azurewebsites.net`)?
- [ ] ¿Qué hace el **Health check** de App Service y qué pasa si solo hay una instancia?
- [ ] ¿Qué diferencia hay entre configurar CORS en App Service y configurarlo en el código? ¿Por qué no en ambos?

## Ejercicio 1 · Tu propia API

Crea una carpeta `lab/api` en este repo con una minimal API.

1. Usa `dotnet new` con una plantilla mínima de ASP.NET Core.
2. Agrega un endpoint `/health` que responda 200.
3. Agrega un endpoint `/api/info` que lea un valor de configuración llamado `MENSAJE_BIENVENIDA` y lo devuelva en JSON.
4. Si `MENSAJE_BIENVENIDA` no existe, el endpoint debe **fallar con 500** (lo usarás en los ejercicios de troubleshooting).
5. Agrega un endpoint `/api/items` que devuelva una lista fija de datos.
6. Pruébalo local con `dotnet run` y `curl`.

<details>
<summary>Pista: health checks</summary>

ASP.NET Core trae health checks integrados: busca `AddHealthChecks` y `MapHealthChecks`.

</details>

<details>
<summary>Pista: leer configuración</summary>

Inyecta `IConfiguration` en el handler del endpoint. Las variables de entorno se leen igual que las claves de `appsettings.json`.

</details>

**Verifica:** `curl localhost:<puerto>/api/info` devuelve tu mensaje, y si quitas el valor responde 500.

## Ejercicio 2 · Desplegar desde el portal

1. Crea un resource group `rg-hack-<tunombre>`.
2. Crea una Web App Linux, .NET 10, plan B1.
3. Publica la API (desde VS Code con la extensión de Azure, o subiendo un zip).
4. Configura `MENSAJE_BIENVENIDA` como App Setting.
5. Abre `https://<app>.azurewebsites.net/api/info`.

**Verifica:** ves el mensaje que configuraste en el portal, no el de tu `appsettings.json`.

## Ejercicio 3 · Desplegar con la CLI

1. Borra la Web App del ejercicio anterior.
2. Despliégala de nuevo usando solo `az webapp up`.
3. Cronometra el proceso.

<details>
<summary>Pista</summary>

`az webapp up --help`. Fíjate en los parámetros `--runtime`, `--sku` y `--resource-group`. Lista los runtimes válidos con `az webapp list-runtimes --os linux`.

</details>

## Ejercicio 4 · Frontend estático

1. Crea `lab/frontend` con un `index.html` y un `.js` que llamen con `fetch` a `/api/items` de tu API en Azure y muestren el resultado.
2. Despliégalo como **Static Web App** (tier Free) o como una segunda Web App.
3. La URL de la API debe estar en **un solo lugar** del código del frontend, para que sea fácil cambiarla.

> **Ojo:** usa HTML + JS, no Blazor Server. Blazor Server llama a la API desde el servidor, no desde el navegador, así que nunca vas a ver un error de CORS, y ese error es justo lo que quieres practicar.

<details>
<summary>Pista: regiones de Static Web Apps</summary>

Static Web Apps solo está disponible en algunas regiones (por ejemplo `eastus2`, `centralus`, `westus2`, `westeurope`, `eastasia`). Puede estar en una región distinta a la de la API.

</details>

## Ejercicio 5 · Romper y arreglar CORS

1. Abre el frontend en el navegador con DevTools (F12) → Console y Network.
2. Observa el error de CORS. Cópialo en tus notas **exacto**.
3. Arréglalo agregando el origen del frontend en la configuración CORS de App Service.
4. Ahora quítalo con la CLI y vuelve a agregarlo con la CLI.

**Verifica:** el frontend muestra los items y la consola no tiene errores.

## Ejercicio 6 · App Settings en vivo

1. Cambia `MENSAJE_BIENVENIDA` desde el portal.
2. Mide cuánto tarda en reflejarse. ¿La app se reinició?
3. Haz lo mismo con `az webapp config appsettings set`.

## Ejercicio 7 · Herramientas de diagnóstico

Recorre cada herramienta y anota para qué te sirve:

- [ ] **Log stream:** genera tráfico y mira los logs en vivo (portal y `az webapp log tail`)
- [ ] **Kudu:** entra, abre la consola (Bash/SSH) y encuentra dónde están los archivos desplegados
- [ ] **Diagnose and solve problems:** abre "Availability and Performance" y lee qué muestra
- [ ] **Health check:** actívalo con `/health` y luego con una ruta que no existe. ¿Qué pasa?
- [ ] **Configuration → General settings:** ubica runtime stack y startup command

## Ejercicio 8 · Leer C# ajeno

Abre `BlazorApp1/Program.cs` (la plantilla de este repo) y explica cada línea en tus notas: qué servicio registra y qué middleware agrega. En la competencia vas a leer código que no escribiste.

---

## Listo cuando

- [ ] Despliegas la API desde cero en **menos de 15 min** sin mirar notas
- [ ] Sabes arreglar un CORS desde el portal y desde la CLI
- [ ] Sabes abrir Log stream y Kudu sin buscar dónde están

## Mis notas

- Error de CORS exacto:
- Tiempo de despliegue con `az webapp up`:
- Comandos que tuve que buscar:
