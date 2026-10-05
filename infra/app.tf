# Generate a random integer to create a globally unique name
resource "random_integer" "ri" {
  min = 10000
  max = 99999
}

# Crete or add an existiing resource group in Azure
data "azurerm_resource_group" "main" {
  name = "rg-hachathon-copa"
}

resource "azurerm_service_plan" "main" {
  name                = "contacts-api-service-plan-${random_integer.ri.result}"
  resource_group_name = data.azurerm_resource_group.main.name
  location            = data.azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_linux_web_app" "api" {
  name                = "contacts-api-${random_integer.ri.result}"
  resource_group_name = data.azurerm_resource_group.main.name
  location            = azurerm_service_plan.main.location
  service_plan_id     = azurerm_service_plan.main.id
  https_only          = true

  identity {
    type = "SystemAssigned"
  }

  site_config {
    always_on = false # obligatorio en false si usas F1
    application_stack {
      dotnet_version = "10.0" # la versión que use tu proyecto
    }
  }

  app_settings = {
    ASPNETCORE_ENVIRONMENT     = "Production"
    ConnectionStrings__Default = "Data Source=/home/app.db"
  }
}

output "api_url" {
  value = "https://${azurerm_linux_web_app.api.default_hostname}"
}
