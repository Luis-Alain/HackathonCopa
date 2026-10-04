# 00 · Setup del entorno

**Objetivo:** que cada laptop del equipo pueda crear recursos en Azure, correr Terraform y usar Dynatrace. Sin esto no se puede practicar nada más.

**Tiempo estimado:** 1 h por persona.

---

## 1. Herramientas de línea de comandos

Instala lo que falte y verifica cada una con su comando.

| Herramienta | Verificación             | Resultado esperado                   |
| ----------- | ------------------------ | ------------------------------------ |
| .NET SDK    | `dotnet --list-sdks`     | Aparece `10.0.x`                     |
| Azure CLI   | `az version`             | Imprime un JSON con `azure-cli`      |
| Terraform   | `terraform -version`     | `Terraform v1.x`                     |
| Git         | `git --version`          | Cualquier versión reciente           |
| Docker      | `docker info`            | No da error de conexión              |
| Node.js     | `node -v`                | v18 o superior                       |

<details>
<summary>Pista: cómo instalar az y terraform en macOS</summary>

Ambos están en Homebrew. Terraform no está en el repositorio principal de Homebrew; está en el "tap" oficial de HashiCorp. Busca en la documentación oficial de HashiCorp "Install Terraform".

</details>

## 2. Extensiones de VS Code

- [ ] Azure Tools (incluye App Service y Resources)
- [ ] HashiCorp Terraform
- [ ] C# Dev Kit
- [ ] Kusto (KQL), opcional, para resaltar consultas en archivos `.kql`

## 3. Iniciar sesión en Azure

1. Ejecuta `az login` y completa el inicio de sesión en el navegador.
2. Lista tus suscripciones y ubica la de **Azure for Students**.
3. Fíjala como activa.
4. Confirma con `az account show` que el `name` es la suscripción correcta.

<details>
<summary>Pista</summary>

`az account list -o table` y `az account set --subscription <id>`.

</details>

## 4. Verificar qué puedes crear

Azure for Students tiene **restricciones**: no todas las regiones están permitidas y algunos proveedores de recursos no vienen registrados.

1. Comprueba qué runtimes .NET ofrece App Service en Linux. Busca `DOTNETCORE:10.0` en la salida.
2. Revisa si estos proveedores están registrados: `Microsoft.Web`, `Microsoft.Insights`, `Microsoft.OperationalInsights`.
3. Crea un resource group de prueba en `eastus` y luego bórralo. Si te da `RequestDisallowedByPolicy` o `RequestDisallowedByAzure`, prueba con otra región y **anota cuál funcionó** en la sección de notas de abajo.

<details>
<summary>Pistas</summary>

- `az webapp list-runtimes --os linux`
- `az provider show -n Microsoft.Web --query registrationState`
- `az provider register -n <proveedor>`

</details>

> Si `DOTNETCORE:10.0` no aparece en tu región, el equipo debe acordar trabajar con .NET 8 y tener ese SDK instalado.

## 5. Terraform funcionando

1. Crea una carpeta temporal fuera del repo.
2. Escribe un `main.tf` mínimo que solo declare el provider `azurerm`.
3. Corre `terraform init`. Debe descargar el provider sin errores.
4. Borra la carpeta.

## 6. Azure DevOps (hacer hoy, tarda días)

Las organizaciones nuevas de Azure DevOps **no tienen agentes hospedados gratis** hasta que Microsoft aprueba una solicitud, y eso tarda de 2 a 3 días hábiles.

- [ ] Crear la organización en dev.azure.com y un proyecto de práctica
- [ ] Plan B: aprender a registrar un **agente autohospedado** en tu laptop (ver [03-azure-devops-pipelines.md](03-azure-devops-pipelines.md))

## 7. Dynatrace

- [ ] Cuenta en Dynatrace University
- [ ] Acceso al Dynatrace Playground (seguir la Guía del Estudiante)
- [ ] Inscrito en el curso Dynatrace Essentials

## 8. Equipo

- [ ] Grupo de chat creado
- [ ] Los 4 miembros con acceso de escritura a este repo
- [ ] Roles asignados en el README

---

## Listo cuando

- [ ] `az account show` muestra la suscripción correcta
- [ ] `terraform init` descarga `azurerm` sin errores
- [ ] Sabes en qué región puedes crear recursos
- [ ] La solicitud de paralelismo de Azure DevOps está enviada

## Mis notas

- Región que funciona:
- Versión de .NET acordada:
- Problemas encontrados:
