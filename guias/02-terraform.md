# 02 · Terraform (infraestructura como código)

**Objetivo:** levantar toda la infraestructura (API, frontend y monitoreo) con un solo `terraform apply` y saber leer y corregir errores de Terraform.

**Rol experto:** IaC / Pipelines · **Tiempo estimado:** 3–4 h

**Material:** Workshop 3 (DevOps, Git, GitHub y Terraform), cápsulas de IaC, documentación del provider `azurerm` en el Terraform Registry.

**Requisito:** [01-azure-app-service.md](01-azure-app-service.md), porque vas a automatizar lo que ya hiciste a mano.

---

## Conceptos que debes poder explicar

- [ ] El flujo `init` → `fmt` / `validate` → `plan` → `apply` → `destroy`. ¿Qué hace cada uno?
- [ ] ¿Qué es el **state**? ¿Dónde se guarda por defecto? ¿Por qué no se edita a mano ni se sube a Git?
- [ ] ¿Qué es `.terraform.lock.hcl` y por qué **sí** se sube a Git?
- [ ] Variables, `terraform.tfvars` y outputs. ¿Cuándo usar `sensitive = true`?
- [ ] Símbolos de un plan: `+`, `~`, `-`, `-/+`. ¿Cuál es el peligroso?
- [ ] ¿Qué es el **drift**?
- [ ] ¿Por qué el nombre de una Web App debe ser único en todo Azure y cómo lo resuelves en Terraform?
- [ ] El state de `infra/` es local. ¿Qué pasa si dos compañeros hacen `apply` cada uno con su state? ¿Qué es un backend remoto?

## Ejercicio 1 · Lee y corre `infra/`

La carpeta `infra/` ya tiene el Terraform de la API: `main.tf` (provider y resource group) y `app.tf` (plan, Web App, Log Analytics, diagnostic settings y App Insights). Usa `azurerm` `=5.0.0` y la región `westus`.

1. Lee `main.tf` y `app.tf` y explica en tus notas qué hace cada bloque `resource`, `data` y `output`, y qué atributo conecta cada recurso con el siguiente (por ejemplo, de dónde sale `APPLICATIONINSIGHTS_CONNECTION_STRING`).
2. Corre `terraform init`, `terraform fmt`, `terraform validate` y `terraform plan`. Lee el plan completo y cuenta cuántos recursos va a crear.
3. Haz `apply`, revisa los recursos en el portal y corre `terraform state list` y `terraform show`.
4. Responde: el resource group se declara como `resource` en `main.tf` y se vuelve a leer como `data` en `app.tf`. ¿Hace falta el `data`? ¿Qué podrías usar en su lugar?
5. Responde: ¿dónde está el state ahora? ¿Está en el repo? ¿Qué le pasaría a un compañero que corra `apply` con su propio state local?

<details>
<summary>Pista: la suscripción</summary>

`infra/` no declara `subscription_id`, así que el provider usa la suscripción activa de `az login`. Antes de hacer `apply`, confirma cuál es con `az account show`.

</details>

## Ejercicio 2 · Completa lo que falta

`infra/` cubre la API y el monitoreo. Agrega lo que todavía no tiene, haciendo `plan` + `apply` después de cada cambio:

1. El App Setting `MENSAJE_BIENVENIDA` (lo lee `/api/info`, guía 01).
2. El frontend como `azurerm_static_web_app`. Hoy el origen de CORS está escrito a mano en `app.tf`: haz que salga del recurso.
3. Outputs: URL del frontend, nombre del resource group y nombre de la Web App (la guía 03 los necesita).
4. Declara el provider `random` en `required_providers`, junto a `azurerm`. Hoy solo está `azurerm`.
5. Decide cómo evitas que el nombre de la Web App cambie en cada `destroy` + `apply` (ver el Ejercicio 3).

Usa las pistas solo si te atascas más de 15 minutos con un cambio.

<details>
<summary>Pista: nombres únicos</summary>

`infra/` usa `random_integer`, y su valor vive en el state: después de un `destroy` se genera otro. Si necesitas un nombre estable, ¿qué alternativa tienes? (por ejemplo, una variable con el sufijo fijo).

</details>

<details>
<summary>Pista: dónde va cada cosa en la Web App</summary>

El stack va en `site_config` → `application_stack`. El health check y CORS también van dentro de `site_config`. Los App Settings son un mapa aparte, `app_settings`. Revisa la documentación de `azurerm_linux_web_app` en el Registry: la sección "Arguments Reference" lista qué es obligatorio.

</details>

<details>
<summary>Pista: el health check pide otro argumento</summary>

Si defines `health_check_path`, el provider te pedirá también `health_check_eviction_time_in_min`. Lee el error, te dice exactamente qué falta.

</details>

**Verifica:** despliega `WebAppApi` sobre la Web App creada por Terraform, abre `/api/info` y corre la colección de Postman contra esa URL (`npx newman run docs/postman/WebAppApi.postman_collection.json --env-var baseUrl=https://<tu-app>.azurewebsites.net`).

## Ejercicio 3 · Destruir y recrear

1. `terraform destroy`. Cronometra.
2. `terraform apply`. Cronometra.
3. Vuelve a desplegar la API. El workflow de GitHub Actions (`.github/workflows/deploy-api.yml`) apunta a un nombre fijo (`AZURE_WEBAPP_NAME`) y usa un secret con el publish profile. ¿Qué se rompe después de recrear la infraestructura? Anota qué tienes que hacer a mano todavía: es lo que el pipeline de la guía 03 ([03-azure-devops-pipelines.md](03-azure-devops-pipelines.md)) debe automatizar o evitar.

## Ejercicio 4 · Drift

1. Cambia `MENSAJE_BIENVENIDA` desde el portal.
2. Corre `terraform plan`. Lee qué propone.
3. Decide: ¿el portal o el código es la fuente de verdad? Aplica tu decisión.
4. Repite con algo de `site_config`, por ejemplo la ruta del health check.

## Ejercicio 5 · Leer errores a propósito

Rompe el código de una forma por vez, corre `validate` o `plan` y anota el mensaje de error y cómo lo identificaste:

- [ ] Un tipo incorrecto (por ejemplo un número donde va un string)
- [ ] Una referencia a un recurso que no existe
- [ ] Un argumento con un nombre mal escrito
- [ ] Un nombre de Web App que ya existe en Azure (usa el de un compañero)
- [ ] Una región que tu suscripción no permite
- [ ] Una versión de .NET que no existe (por ejemplo `"99.0"`)

## Ejercicio 6 · Plan peligroso

Cambia algo que obligue a **reemplazar** un recurso (por ejemplo el nombre o la región del resource group) y lee el plan **sin aplicarlo**. Encuentra el `-/+` y entiende qué se perdería.

---

## Listo cuando

- [ ] La infraestructura completa se crea con un solo `terraform apply`
- [ ] Puedes leer un plan y decir en voz alta qué va a pasar
- [ ] Reconoces los 6 errores del ejercicio 5 por su mensaje

## Mis notas

- Tiempo de `apply` completo:
- Errores y su significado:
