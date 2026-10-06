# 03 · Git y Azure DevOps Pipelines

**Objetivo:** que un push a `main` compile y despliegue la API automáticamente, y saber diagnosticar un pipeline que falla.

**Rol experto:** IaC / Pipelines · **Tiempo estimado:** 2–3 h

**Material:** Workshop 3, documentación de Azure Pipelines (YAML schema), tareas `DotNetCoreCLI@2` y `AzureWebApp@1`.

**Requisito:** tener la Web App creada ([02-terraform.md](02-terraform.md)) y la solicitud de paralelismo enviada ([00-setup.md](00-setup.md)).

---

## Conceptos que debes poder explicar

- [ ] Estructura de un pipeline YAML: `trigger`, `pool`, `variables`, `stages` → `jobs` → `steps`
- [ ] Diferencia entre un agente **hospedado por Microsoft** y uno **autohospedado**
- [ ] ¿Qué es una **service connection** y qué permisos necesita?
- [ ] ¿Qué es un **artifact** y por qué se separa build de deploy?
- [ ] Azure DevOps puede usar este repo de GitHub como origen. ¿Qué autorización pide?
- [ ] ¿Qué diferencia hay entre autenticar con un **publish profile** (como hace el workflow actual) y con una **service connection**? ¿Cuál es más seguro y por qué?

## Punto de partida: tu workflow de GitHub Actions

El repo ya despliega la API con GitHub Actions: [`.github/workflows/deploy-api.yml`](../.github/workflows/deploy-api.yml). Léelo antes de empezar y descríbelo en tus notas: qué lo dispara, qué hace cada job, cómo se autentica contra Azure y qué comprueba al final.

El hackathon usa Azure DevOps, así que los ejercicios 3 y 4 consisten en **llevar ese mismo flujo a Azure Pipelines**.

## Ejercicio 1 · Git en equipo

Si alguien del equipo no domina Git, hagan esto juntos:

1. Cada uno crea una rama, cambia algo distinto y abre un Pull Request en GitHub.
2. Provoquen un conflicto a propósito (dos personas editan la misma línea) y resuélvanlo.
3. Acuerden una regla para el día de la competencia (por ejemplo: commits pequeños directo a `main` con un mensaje claro).

## Ejercicio 2 · Service connection

1. En el proyecto de Azure DevOps crea una service connection de tipo **Azure Resource Manager** apuntando a tu resource group.
2. Anota qué método de autenticación usaste.

<details>
<summary>Si falla por permisos</summary>

Algunas cuentas universitarias no permiten crear "app registrations" en el directorio. Lee el mensaje de error, prueba las otras opciones de autenticación que ofrece el asistente y anota cuál funcionó. Si ninguna funciona, pregúntalo a los mentores el jueves.

</details>

## Ejercicio 3 · Pipeline de build

Traduce el job `build` de `deploy-api.yml`.

1. Crea `azure-pipelines.yml` en la raíz del repo.
2. Debe dispararse solo con cambios en `WebAppApi/`.
3. Pasos: instalar el SDK .NET 10, hacer `publish` de `WebAppApi/WebAppApi.csproj` en Release y publicar el resultado como artifact.
4. Haz push y mira el job correr.

<details>
<summary>Pista: tareas útiles</summary>

`UseDotNet@2` para fijar la versión del SDK y `DotNetCoreCLI@2` con `command: publish` y `zipAfterPublish: true`. Para el artifact, el paso `publish:` del YAML.

</details>

<details>
<summary>Pista: equivalencias GitHub Actions → Azure Pipelines</summary>

| GitHub Actions | Azure Pipelines |
|---|---|
| `on.push.branches` / `paths` | `trigger.branches` / `trigger.paths.include` |
| `runs-on: ubuntu-latest` | `pool.vmImage: ubuntu-latest` (o tu agente autohospedado) |
| `jobs.<id>` y `needs` | `stages` → `jobs` y `dependsOn` |
| `actions/setup-dotnet` | `UseDotNet@2` |
| `dotnet publish` | `DotNetCoreCLI@2` con `command: publish` |
| `upload-artifact` / `download-artifact` | pasos `publish:` / `download:` |
| `azure/webapps-deploy` con publish profile | `AzureWebApp@1` con una service connection |
| `secrets.*` | variables secretas o variable groups |
| `environment` | Environments de Azure DevOps |
| paso con `curl` (smoke test) | paso `script:` |

</details>

## Ejercicio 4 · Pipeline de deploy

1. Agrega un segundo stage que dependa del build.
2. Descarga el artifact y despliégalo a tu Web App con `AzureWebApp@1` (tipo Linux).
3. Cambia el texto de algún endpoint, haz push y verifica el cambio en Azure **sin tocar el portal**.
4. Agrega al final un paso que llame a `/healthz` y haga fallar el pipeline si no responde `200`, como el smoke test del workflow de GitHub.

## Ejercicio 5 · Romper el pipeline

Un compañero rompe el pipeline sin decirte cómo; tú lo diagnosticas leyendo **solo los logs del job**. Ideas para quien rompe:

- [ ] Ruta del `.csproj` mal escrita
- [ ] Versión de .NET que no coincide con el `TargetFramework`
- [ ] Nombre de la Web App equivocado
- [ ] Service connection con un nombre que no existe
- [ ] Error de compilación en el código
- [ ] En el workflow de GitHub: el secret `AZURE_WEBAPP_PUBLISH_PROFILE` ausente, o Basic Authentication desactivada en la Web App

Para cada uno, anota en qué paso falló y la línea del log que lo delató.

## Plan B · Agente autohospedado

Si Microsoft no aprueba el paralelismo a tiempo:

1. En Organization Settings → Agent pools, sigue las instrucciones para agregar un agente en macOS.
2. Regístralo en tu laptop y córrelo.
3. Cambia el `pool` del YAML para usar ese agente.

---

## Listo cuando

- [ ] Un push a `main` despliega la API sin intervención manual
- [ ] Diagnosticas un pipeline roto en menos de 5 min leyendo los logs
- [ ] Sabes qué hacer si no hay agentes hospedados

## Mis notas

- Método de autenticación que funcionó:
- Errores de pipeline y la línea que los delató:
