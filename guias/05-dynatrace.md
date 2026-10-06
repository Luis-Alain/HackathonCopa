# 05 · Dynatrace

**Objetivo:** navegar Dynatrace con soltura y consultar logs con DQL. Dynatrace es organizador del evento, así que es casi seguro que habrá retos sobre él.

**Rol experto:** Observabilidad · **Tiempo estimado:** 5–6 h (incluye el curso)

**Material:** Workshop 6 (Dynatrace Playground, University y Essentials), Guía del Estudiante, PDF "Conceptos Clave de Observabilidad en Dynatrace", curso Dynatrace Essentials (9 lecciones, ~4 h 30 min).

---

## Paso 1 · Curso Essentials

Haz el curso completo en Dynatrace University. Divide el equipo para que nadie se atrase, pero **todos** lo terminan.

- [ ] Lecciones 1–3
- [ ] Lecciones 4–6
- [ ] Lecciones 7–9

Al terminar cada bloque, escribe aquí 3 ideas clave con tus palabras.

## Conceptos que debes poder explicar

- [ ] ¿Qué es OneAgent y qué detecta automáticamente?
- [ ] ¿Qué es un **Problem** y cómo **Davis AI** agrupa varios síntomas en uno solo?
- [ ] ¿Qué muestra **Smartscape**? (host → proceso → servicio → aplicación)
- [ ] ¿Qué es una **traza distribuida** y qué es un span?
- [ ] Diferencia entre las vistas de Hosts/Infraestructura, Services, Distributed Traces, Logs y Problems
- [ ] Qué significan Application Security, User Experience (RUM) y Business Observability (temas de la charla del día 1)

## Ejercicio 1 · Recorrido del Playground

Para cada vista, anota la ruta exacta en el menú para llegar rápido el día de la competencia:

| Vista              | Cómo llego | Para qué la usaría |
| ------------------ | ---------- | ------------------ |
| Problems           |            |                    |
| Services           |            |                    |
| Distributed Traces |            |                    |
| Logs               |            |                    |
| Hosts / Kubernetes |            |                    |
| Smartscape         |            |                    |
| Notebooks (DQL)    |            |                    |

## Ejercicio 2 · Problems y causa raíz

1. Encuentra un Problem abierto en el Playground.
2. Explica su causa raíz **en 2 frases**, como si se lo dijeras a un jurado.
3. Sigue los enlaces desde el Problem hasta el servicio o proceso afectado.

## Ejercicio 3 · Trazas distribuidas

1. Abre una traza de un servicio con varias llamadas.
2. Identifica el span más lento.
3. ¿El tiempo se va en el código propio o en una llamada a otro servicio o base de datos?

## Ejercicio 4 · DQL

Escribe estas consultas en un Notebook. Los nombres de campos cambian según el entorno, así que **descubre los nombres reales** antes de filtrar. Guarda las que funcionen en `guias/chuletas/dql.md`.

1. Los últimos 50 logs de nivel error.
2. Cantidad de logs por nivel.
3. Los mensajes de error más repetidos.
4. Cantidad de errores por hora, como serie de tiempo.
5. Logs que contengan un texto (por ejemplo `Exception` o `timeout`).
6. Los Problems más recientes.

<details>
<summary>Pista: descubrir los campos</summary>

Empieza con `fetch logs | limit 5` y mira qué columnas devuelve antes de escribir filtros.

</details>

<details>
<summary>Pista: comandos</summary>

`fetch`, `filter`, `fields`, `summarize`, `sort`, `limit`, `makeTimeseries`, y la función `contains()`. Los Problems de Davis están en su propia fuente de datos; búscala en la documentación de DQL.

</details>

## Ejercicio 5 · Mismo fallo, dos herramientas

Elige un fallo del [04-observabilidad-app-insights-kql.md](04-observabilidad-app-insights-kql.md) y escribe cómo lo diagnosticarías en App Insights y cómo lo harías en Dynatrace. ¿Cuál es más rápido para qué tipo de problema?

---

## Listo cuando

- [ ] Curso Essentials terminado
- [ ] Vas de "la app falla" a "este servicio, este error, desde esta hora" en **menos de 5 min**
- [ ] Tienes 6 consultas DQL que funcionan en el Playground

## Mis notas

- Ideas clave del curso:
- Nombres reales de campos en los logs:
