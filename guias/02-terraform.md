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

## Ejercicio 1 · Primer recurso

1. Trabaja en la carpeta `infra/` de la raíz del repo. Ya tiene un `main.tf` con un resource group, pero usa el provider `azurerm` **3.x** (`~> 3.0.2`).
2. Actualízalo a `azurerm` versión 4.x con el bloque `features {}`. Después de cambiar la versión, corre `terraform init -upgrade`.
3. Crea **solo** un resource group.
4. Corre todo el flujo hasta `apply`. Revisa el recurso en el portal.
5. Corre `terraform state list` y `terraform show`.

<details>
<summary>Pista: azurerm 4.x pide la suscripción</summary>

A diferencia de la versión 3, en la 4.x el provider exige `subscription_id` (en el bloque del provider o con la variable de entorno `ARM_SUBSCRIPTION_ID`). Pásala como variable, no la escribas fija en el código.

</details>

## Ejercicio 2 · La infraestructura completa

Agrega, uno por uno, y haz `plan` + `apply` después de cada recurso:

1. `azurerm_log_analytics_workspace`
2. `azurerm_application_insights` conectado al workspace
3. `azurerm_service_plan` (Linux, B1)
4. `azurerm_linux_web_app` para `WebAppApi`, con:
   - stack .NET 10
   - health check en `/healthz`
   - App Settings: `MENSAJE_BIENVENIDA`, `ConnectionStrings__Default` (la ruta del `.db` en `/home`, ver guía 01, Ejercicio 2b) y `APPLICATIONINSIGHTS_CONNECTION_STRING` (tomada del recurso de App Insights, no copiada a mano)
   - CORS con el origen de tu frontend
5. El frontend de la carpeta `frontend/` (`azurerm_static_web_app` u otra Web App)
6. Outputs: URL de la API, URL del frontend, nombre del resource group

Usa las pistas solo si te atascas más de 15 minutos con un recurso.

<details>
<summary>Pista: nombres únicos</summary>

El provider `random` tiene un recurso `random_string`. Úsalo como sufijo en un `locals`.

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
3. Vuelve a desplegar la API. ¿Qué tienes que hacer a mano todavía? Anótalo: es lo que el pipeline debe automatizar ([03-azure-devops-pipelines.md](03-azure-devops-pipelines.md)).

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
