# 04 · Observabilidad con Application Insights y KQL

**Objetivo:** encontrar la causa de un fallo con evidencia de la telemetría, no adivinando. Es el tema que más puntos decide.

**Rol experto:** Observabilidad (pero **todos** deben dominarlo) · **Tiempo estimado:** 4–5 h

**Material:** Workshop 4 (Observabilidad, Application Insights, Logs y KQL), Workshop 5 (Troubleshooting en Azure), cápsulas de KQL y diagnóstico.

**Requisito:** API desplegada con App Insights conectado ([02-terraform.md](02-terraform.md)). `infra/` ya crea el recurso y pone su connection string en los App Settings; el SDK dentro del código lo agregas en el Ejercicio 1.

---

## Conceptos que debes poder explicar

- [ ] Los tres pilares: **métricas, logs y trazas**. Un ejemplo de cada uno en tu API.
- [ ] Qué guarda cada tabla: `requests`, `exceptions`, `dependencies`, `traces`, `customEvents`
- [ ] Cómo se relacionan las filas de distintas tablas con `operation_Id`
- [ ] Por qué en Log Analytics las mismas tablas se llaman `AppRequests`, `AppExceptions`, etc.
- [ ] La diferencia entre **síntoma** (500, lentitud) y **causa** (excepción, dependencia caída, configuración faltante)
- [ ] Cuánto tarda la telemetría en aparecer y qué vista es casi en tiempo real

## Ejercicio 1 · Instrumentar tu API

1. Agrega el paquete de Application Insights para ASP.NET Core a tu API.
2. Regístralo en `Program.cs`.
3. Agrega un `ILogger` y escribe un `LogError` antes de que `/api/info` falle.
4. Agrega un endpoint `/api/externo` que llame con `HttpClient` a una URL configurable (App Setting `EXTERNAL_API_URL`). Esto genera filas en `dependencies`.
5. Despliega y genera tráfico.

<details>
<summary>Pista: si la app no arranca después de agregar App Insights</summary>

Algunas versiones del SDK lanzan una excepción al iniciar si `APPLICATIONINSIGHTS_CONNECTION_STRING` está vacía. Pruébalo local sin la variable. Si pasa, piensa cómo registrar App Insights solo cuando la variable existe, para que la app funcione aunque no haya telemetría.

</details>

<details>
<summary>Pista: generar tráfico</summary>

Un bucle en la terminal con `curl` a tus endpoints cada segundo durante un par de minutos basta. Escríbelo tú; lo vas a reusar en todos los ejercicios.

</details>

## Ejercicio 2 · Recorrer el portal

Con tráfico corriendo, abre cada vista y anota qué pregunta responde:

- [ ] **Live Metrics**
- [ ] **Failures**: entra a una excepción y llega hasta la línea de código
- [ ] **Performance**: encuentra el endpoint más lento
- [ ] **Application Map**: ¿aparece tu dependencia externa?
- [ ] **Transaction search**: busca una petición y abre su detalle completo
- [ ] **Logs**: donde vas a escribir KQL

## Ejercicio 3 · KQL desde cero

Escribe cada consulta **tú mismo**. Comprueba el resultado contra lo que ves en el portal. Guarda las que funcionen en un archivo `guias/chuletas/kql.md` del repo.

1. Las últimas 20 peticiones con nombre, código de respuesta y duración.
2. Cantidad de peticiones fallidas por endpoint y código en la última hora.
3. Las excepciones más frecuentes por tipo y mensaje.
4. Las últimas excepciones con su `operation_Id`.
5. Dependencias que fallan, agrupadas por destino.
6. El percentil 95 de duración por endpoint, ordenado de más lento a más rápido.
7. Peticiones por intervalos de 5 minutos separadas en éxito/fallo, como gráfico de tiempo.
8. Logs de nivel error o superior.
9. Todo lo que pasó en una sola petición (las 4 tablas, filtrando por un `operation_Id`).
10. Qué endpoint empezó a fallar primero y a qué hora.

<details>
<summary>Pista: operadores que vas a necesitar</summary>

`where`, `project`, `summarize`, `count()`, `percentile()`, `bin()`, `order by`, `take`, `union`, `join`, `render timechart`, `ago()`. Busca cada uno en la documentación de KQL.

</details>

<details>
<summary>Pista: consulta 7</summary>

`summarize` puede agrupar por más de una columna a la vez, y una de ellas puede ser `bin(timestamp, 5m)`.

</details>

<details>
<summary>Pista: consulta 9</summary>

`union` junta tablas con columnas distintas. Filtra después del `union`.

</details>

## Ejercicio 4 · De síntoma a causa

Para cada caso, provócalo tú y encuentra la evidencia **solo con App Insights**. Anota la consulta o la vista que te llevó a la causa.

| Provoca esto                                   | Síntoma que verás | Evidencia que lo prueba |
| ---------------------------------------------- | ----------------- | ----------------------- |
| Borra `MENSAJE_BIENVENIDA`                     |                   |                         |
| Apunta `ConnectionStrings__Default` a una carpeta que no existe (`Data Source=/home/noexiste/app.db`) |                   |                         |
| Pon `EXTERNAL_API_URL` con un host inexistente |                   |                         |
| Agrega un `Task.Delay` de 3 s a un endpoint    |                   |                         |
| Lanza una excepción no controlada              |                   |                         |
| Vacía `APPLICATIONINSIGHTS_CONNECTION_STRING`  |                   |                         |

Pregunta para el último caso: si no llega telemetría, ¿con qué otra herramienta lo diagnosticas?

---

## Listo cuando

- [ ] Tienes 10 consultas KQL guardadas que funcionan
- [ ] Llenaste la tabla del ejercicio 4
- [ ] Vas de "falla un endpoint" a "esta excepción, en esta línea" en menos de 5 min

## Mis notas

- Vista del portal más útil:
- Consulta que más me costó:
