# 01 · Azure y App Service

**Objetivo:** desplegar una API .NET y un frontend en Azure, y saber dónde mirar cuando fallan.

**Rol experto:** Azure / App Service · **Tiempo estimado:** 5–6 h

**Material:** Workshop 1 (Fundamentos de Azure), Workshop 2 (APIs, frontend y backend), cápsulas de Azure y App Service.

## Qué hay en este repo

Todas las guías usan estas carpetas:

| Carpeta | Contenido |
|---|---|
| `WebAppApi/` | La API .NET 10 que vas a desplegar: contactos, mensajes entre contactos y health check en `/healthz`. Usa EF Core con SQLite |
| `docs/` | Documentación de la API y la colección de Postman (`docs/postman/`) |
| `infra/` | Terraform de la API, ya escrito (guía 02) |
| `.github/workflows/` | Despliegue con GitHub Actions (guía 03) |
| `frontend/` | El frontend HTML + JS. **Todavía no existe:** lo creas en el Ejercicio 4 |

Antes de empezar, lee el [README del proyecto](../README.md): explica cómo correr la API, qué hace cada endpoint y qué está hecho y qué falta.

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

## Ejercicio 1 · WebAppApi en local

La API ya existe en `WebAppApi/`. Primero hazla correr en tu máquina, y luego agrégale un endpoint de configuración que usarás en los ejercicios de troubleshooting.

**Parte A · Correrla**

1. Desde `WebAppApi/`, arranca la API con `dotnet run --launch-profile http` (queda en `http://localhost:5033`). Al arrancar crea `app.db` y aplica las migraciones sola (`MigrateAsync` en `Program.cs`); `dotnet ef` solo hace falta para crear migraciones nuevas.
2. Prueba los endpoints con los archivos `.http`:

   | Archivo | Qué prueba |
   |---|---|
   | `WebAppApi/WebAppApi.http` | health check y OpenAPI |
   | `WebAppApi/features/contacts/contacts.http` | contactos y sus errores |
   | `WebAppApi/features/messages/messages.http` | una conversación entre dos contactos y sus errores |

   Se ejecutan desde el editor: en VS Code con la extensión **REST Client**, o en Visual Studio 2022 (17.12 o superior). En cada archivo, ejecuta las requests **de arriba hacia abajo**, porque las de abajo usan ids creados por las de arriba.

**Parte B · Endpoint `/api/info`**

1. Agrega a `WebAppApi` un endpoint `GET /api/info` que lea un valor de configuración llamado `MENSAJE_BIENVENIDA` y lo devuelva en JSON.
2. Si `MENSAJE_BIENVENIDA` no existe, el endpoint debe **fallar con 500**. Lo usarás en los ejercicios de troubleshooting y en la guía 04.
3. Agrega la request a `WebAppApi.http` y documenta el endpoint en `docs/`.

<details>
<summary>Pista: leer configuración</summary>

Inyecta `IConfiguration` en el handler del endpoint. Las variables de entorno se leen igual que las claves de `appsettings.json`.

</details>

**Verifica:** `curl localhost:5033/api/info` devuelve tu mensaje, y si quitas el valor responde 500.

## Referencia · Las pestañas al crear una Web App

Al crear una Web App en el portal (**Create a resource → Web App**), el asistente tiene varias pestañas. Esta tabla resume qué elegir **para practicar**. Los nombres exactos pueden variar un poco entre versiones del portal.

| Pestaña | Qué elegir | Por qué |
|---|---|---|
| **Basics** | ver abajo | |
| **Database** | **No crear base de datos** | ver "¿Necesito una base de datos?" |
| **Deployment** | Continuous deployment: **Disable**. Basic authentication: **Disable** | desplegarás a mano (VS Code, zip, `az webapp up`); los pipelines llegan en la guía 03 |
| **Networking** | Public access: **On**. Network injection / VNet integration: **Off** | el frontend llama a la API desde el navegador, así que debe ser pública |
| **Monitor + secure** | Application Insights: **Yes** (crea uno nuevo). Microsoft Defender: **Off** | App Insights es la base de la guía 04 (KQL); Defender cuesta y no lo necesitas |
| **Tags** | `proyecto = hackathon-copa`, `owner = <tunombre>`, `env = practica` | sirven para filtrar costos y para saber qué borrar al final |

### Basics

- **Subscription / Resource group:** tu suscripción de Azure for Students y `rg-hack-<tunombre>`.
- **Name:** único en todo Azure; forma parte de la URL.
- **Secure unique default hostname:** si está activado, la URL tendrá un sufijo aleatorio y la región (`<app>-<hash>.<region>-01.azurewebsites.net`) en vez de `<app>.azurewebsites.net`. Cualquiera de las dos funciona; **anota la URL real** que te da el portal.
- **Publish:** Code. **Runtime stack:** .NET 10 (LTS). **Operating System:** Linux.
- **Region:** la que te funcionó en la guía 00.
- **Pricing plan:** B1 para practicar health checks y Log stream sin los límites del F1. Revisa el costo, que se descuenta de tu crédito.
- **Zone redundancy:** Disabled (no aplica en B1).

### Deployment: ¿por qué desactivar Basic authentication?

Con Basic authentication activada, App Service genera usuario y contraseña de publicación (el *publish profile*). Desactivarla es lo más seguro: VS Code, `az webapp up` y Azure DevOps usan tu login de Azure (Microsoft Entra ID), no esas credenciales.

Actívala **solo si** usas un método que pide el publish profile, por ejemplo **Download publish profile** o una GitHub Action configurada con él. Se puede cambiar después en **Configuration → General settings → SCM Basic Auth Publishing Credentials**.

### ¿Necesito una base de datos?

**No en el asistente.** La pestaña **Database** crea una base administrada (Azure SQL o PostgreSQL) junto con una red virtual y un endpoint privado. Eso implica:

- **más recursos y más costo**, que salen de tu crédito;
- **otro proveedor de EF Core:** tu API usa `UseSqlite`. Para Azure SQL tendrías que cambiar a `UseSqlServer`, instalar otro paquete y **regenerar las migraciones**, porque las de SQLite no sirven en SQL Server.

Para practicar, usa **SQLite dentro del mismo App Service** (ver Ejercicio 2b). Considera Azure SQL solo si el reto lo exige, o si varias instancias necesitan compartir los mismos datos.

## Ejercicio 2 · Desplegar desde el portal

1. Crea un resource group `rg-hack-<tunombre>`.
2. Crea una Web App Linux, .NET 10, plan B1, siguiendo la tabla de la referencia anterior.
3. Publica `WebAppApi` (desde VS Code con la extensión de Azure, o subiendo un zip de `dotnet publish`).
4. Configura `MENSAJE_BIENVENIDA` como App Setting.
5. Abre `https://<app>.azurewebsites.net/api/info`.

**Verifica:** ves el mensaje que configuraste en el portal, no el de tu `appsettings.json`.

## Ejercicio 2b · Tu API con SQLite en Azure

Despliega `WebAppApi` (contactos y mensajes) y haz que la base de datos funcione en Azure.

1. Abre `WebAppApi/Program.cs` y localiza dónde se crea y migra la base al arrancar. ¿Qué necesita esa llamada para funcionar en Azure?
2. Despliega la API tal como está (con `Data Source=app.db`) y llama a `GET /api/contacts`. Anota qué ocurre y qué dice **Log stream**.
3. Decide **dónde** vive el archivo `.db`. En App Service Linux, solo `/home` persiste entre reinicios y despliegues. `/home/site/wwwroot` también persiste, pero cada despliegue lo reemplaza.
4. Configura la ruta con un App Setting que reemplace a `ConnectionStrings:Default` (recuerda la regla del `__` de la sección de conceptos).
5. Repite el paso 2. Luego corre la colección de Postman contra Azure:
   ```bash
   npx newman run docs/postman/WebAppApi.postman_collection.json --env-var baseUrl=https://<tu-app>.azurewebsites.net
   ```

<details>
<summary>Pista: qué puede pasar en el paso 2</summary>

Las migraciones ya se aplican solas, así que no deberías ver `no such table`. Los síntomas posibles son un error al abrir o escribir el archivo (por ejemplo `unable to open database file` o base de datos de solo lectura) o datos que desaparecen en el siguiente despliegue. Anota cuál ves y explica por qué.

</details>

<details>
<summary>Pista: la ruta del archivo</summary>

`ConnectionStrings__Default` = `Data Source=/home/app.db`. SQLite crea el archivo, pero **no** las carpetas: si usas `/home/data/app.db`, la carpeta `data` debe existir (créala desde Kudu o SSH).

</details>

<details>
<summary>Nota: migraciones al arrancar</summary>

`Program.cs` crea un scope después de `builder.Build()`, pide el `AppDbContext` y llama a `Database.MigrateAsync()`.

Para una sola instancia está bien. Con varias instancias, todas intentarían migrar a la vez; en ese caso se usan *migration bundles* o un paso del pipeline.

</details>

**Verifica:** la colección de Postman pasa completa contra Azure. Reinicia la app (**Overview → Restart**) y comprueba que los contactos siguen ahí.

## Ejercicio 3 · Desplegar con la CLI

1. Borra la Web App del ejercicio anterior.
2. Despliégala de nuevo usando solo `az webapp up`, ejecutado desde la carpeta `WebAppApi/`.
3. Cronometra el proceso.

<details>
<summary>Pista</summary>

`az webapp up --help`. Fíjate en los parámetros `--runtime`, `--sku` y `--resource-group`. Lista los runtimes válidos con `az webapp list-runtimes --os linux`.

</details>

## Ejercicio 4 · Frontend estático

1. Crea la carpeta `frontend/` en la raíz del repo, con un `index.html` y un `.js` que llamen con `fetch` a `GET /api/contacts` de tu API en Azure y muestren la lista.
   - **Extra:** al elegir dos contactos, muestra su conversación (`GET /api/contacts/{id}/conversations/{otherId}`) y un formulario para enviar un mensaje (`POST /api/messages`). Los cuerpos y respuestas están en `docs/messages.md`.
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

**Verifica:** el frontend muestra los contactos y la consola no tiene errores.

## Ejercicio 6 · App Settings en vivo

1. Cambia `MENSAJE_BIENVENIDA` desde el portal.
2. Mide cuánto tarda en reflejarse. ¿La app se reinició?
3. Haz lo mismo con `az webapp config appsettings set`.

## Ejercicio 7 · Herramientas de diagnóstico

Recorre cada herramienta y anota para qué te sirve:

- [ ] **Log stream:** genera tráfico y mira los logs en vivo (portal y `az webapp log tail`)
- [ ] **Kudu:** entra, abre la consola (Bash/SSH) y encuentra dónde están los archivos desplegados
- [ ] **Diagnose and solve problems:** abre "Availability and Performance" y lee qué muestra
- [ ] **Health check:** actívalo con `/healthz` (el endpoint de `WebAppApi`) y luego con una ruta que no existe. ¿Qué pasa?
- [ ] **Configuration → General settings:** ubica runtime stack y startup command

## Ejercicio 8 · Leer C# ajeno

Abre `WebAppApi/Program.cs` y explica cada línea en tus notas: qué servicio registra y qué middleware agrega. Luego sigue una request completa, por ejemplo `POST /api/messages`:

1. `features/messages/MessageEndpoints.cs`: ¿qué handler la atiende y cómo convierte el resultado en un código HTTP?
2. `features/messages/MessageService.cs`: ¿qué valida antes de guardar y qué consultas hace a la base?
3. `features/messages/MessageModel.cs`: ¿qué validaciones corren antes de llegar al servicio?

En la competencia vas a leer código que no escribiste.

---

## Listo cuando

- [ ] Despliegas la API desde cero en **menos de 15 min** sin mirar notas
- [ ] Sabes arreglar un CORS desde el portal y desde la CLI
- [ ] Sabes abrir Log stream y Kudu sin buscar dónde están

## Mis notas

- Error de CORS exacto:
- Tiempo de despliegue con `az webapp up`:
- Comandos que tuve que buscar:
