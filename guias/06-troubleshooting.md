# 06 · Troubleshooting: romper y arreglar

**Objetivo:** resolver fallos al azar en menos de 10 minutos cada uno, con evidencia. Es lo más parecido a la competencia.

**Todos los roles** · **Tiempo estimado:** 3 h

**Material:** Workshop 5 (verlo dos veces, es el más cercano a los retos) y el Runbook del [README](../README.md#runbook-de-troubleshooting).

**Requisito:** todo funcionando en Azure (`WebAppApi` + `frontend/` + App Insights) y las guías 01, 02 y 04.

---

## Cómo se juega

Se trabaja en parejas. Una persona **rompe** y la otra **diagnostica**, y en cada ronda se intercambian.

1. Quien rompe elige un fallo de la lista sin decir cuál, lo aplica y anota la hora.
2. Quien diagnostica sigue el **método** (abajo) en voz alta.
3. Se detiene el reloj cuando el fallo está arreglado **y verificado**.
4. Se anota la ronda en la hoja de registro.

## El método (siempre en este orden)

1. **Síntoma:** reproducir el fallo (navegador, `curl`, estado del pipeline).
2. **Evidencia:** Log stream, App Insights (Failures o KQL), Dynatrace, logs del job.
3. **Hipótesis:** una sola, la que explica la evidencia.
4. **Fix:** el cambio mínimo.
5. **Verificación:** reproducir de nuevo y confirmar en la telemetría que el error desapareció.
6. **Registro:** una línea de qué era y cómo se arregló.

Si saltas del paso 1 al 4 adivinando, la ronda no cuenta aunque aciertes.

## Fallos para romper

Quien rompe **debe saber cómo deshacerlo** antes de aplicarlo.

**Configuración**

- [ ] Borrar un App Setting que la API necesita
- [ ] `EXTERNAL_API_URL` con host o puerto incorrecto
- [ ] Quitar el origen del frontend de CORS
- [ ] Cambiar la URL de la API en el frontend
- [ ] Vaciar `APPLICATIONINSIGHTS_CONNECTION_STRING`

**Plataforma**

- [ ] Ruta de health check incorrecta
- [ ] Startup command que apunta a una DLL que no existe
- [ ] Runtime stack con una versión de .NET distinta a la compilada
- [ ] Detener la Web App

**Código**

- [ ] Excepción no controlada (por ejemplo un `null` que se usa)
- [ ] Endpoint lento (delay artificial)
- [ ] Endpoint que devuelve 200 pero con datos equivocados

**Infraestructura y pipeline**

- [ ] Cambio a mano en el portal que genera drift con Terraform
- [ ] Error en el código Terraform
- [ ] Pipeline roto (ver [03-azure-devops-pipelines.md](03-azure-devops-pipelines.md))

<details>
<summary>Pista para quien rompe: cambios rápidos con la CLI</summary>

Casi todo se rompe con `az webapp config appsettings`, `az webapp cors`, `az webapp config set` y `az webapp stop`. Revisa el `--help` de cada uno.

</details>

## Hoja de registro

| #   | Fallo (lo llena quien rompe) | Diagnosticó | Tiempo | Evidencia clave | ¿Dónde se perdió tiempo? |
| --- | ---------------------------- | ----------- | ------ | --------------- | ------------------------ |
| 1   |                              |             |        |                 |                          |
| 2   |                              |             |        |                 |                          |
| 3   |                              |             |        |                 |                          |
| 4   |                              |             |        |                 |                          |
| 5   |                              |             |        |                 |                          |
| 6   |                              |             |        |                 |                          |
| 7   |                              |             |        |                 |                          |
| 8   |                              |             |        |                 |                          |

## Después de cada sesión

- [ ] ¿Qué fallo tardó más? ¿Por qué?
- [ ] ¿Falta alguna fila en el Runbook del README? Agrégala.
- [ ] ¿Falta algún comando o consulta en las chuletas? Agrégalo.

---

## Listo cuando

- [ ] Cada miembro resolvió **3 fallos al azar en menos de 10 min** cada uno
- [ ] El Runbook del README refleja lo que aprendieron
