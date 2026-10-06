# Generate a random integer to create a globally unique name
resource "random_integer" "ri" {
  min = 10000
  max = 99999
}

# Crete or add an existiing resource group in Azure
resource "azurerm_resource_group" "rg-hackathon-copa" {
  name     = "rg-hackathon-copa"
  location = "westus"
}

resource "azurerm_service_plan" "main" {
  name                = "contacts-api-service-plan-${random_integer.ri.result}"
  resource_group_name = azurerm_resource_group.rg-hackathon-copa.name
  location            = azurerm_resource_group.rg-hackathon-copa.location
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_linux_web_app" "api" {
  name                = "contacts-api-${random_integer.ri.result}"
  resource_group_name = azurerm_resource_group.rg-hackathon-copa.name
  location            = azurerm_service_plan.main.location
  service_plan_id     = azurerm_service_plan.main.id
  https_only          = true

  identity {
    type = "SystemAssigned"
  }

  site_config {
    health_check_path                 = "/healthz"
    health_check_eviction_time_in_min = 3
    always_on                         = false # obligatorio en false si usas F1
    application_stack {
      dotnet_version = "10.0"
    }

    cors {
      allowed_origins = ["https://purple-sky-0dc1e101e.3.azurestaticapps.net"]
    }
  }

  app_settings = {
    ASPNETCORE_ENVIRONMENT     = "Production"
    ConnectionStrings__Default = "Data Source=/home/app.db"

    APPLICATIONINSIGHTS_CONNECTION_STRING = azurerm_application_insights.app_insights.connection_string
  }
}

# Log Analytics Workspace
resource "azurerm_log_analytics_workspace" "workspace" {
  name                = "monitoring-log-analytics"
  location            = azurerm_resource_group.rg-hackathon-copa.location
  resource_group_name = azurerm_resource_group.rg-hackathon-copa.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

# Sends the Web App logs to Log Analytics (queried with KQL).
resource "azurerm_monitor_diagnostic_setting" "api_logs" {
  name                       = "api-logs-to-log-analytics"
  target_resource_id         = azurerm_linux_web_app.api.id
  log_analytics_workspace_id = azurerm_log_analytics_workspace.workspace.id

  enabled_log {
    category = "AppServiceHTTPLogs"
  }

  enabled_log {
    category = "AppServiceConsoleLogs"
  }

  enabled_log {
    category = "AppServiceAppLogs"
  }

  enabled_metric {
    category = "AllMetrics"
  }
}

# Sets up Application Insights to monitor web application performance.
resource "azurerm_application_insights" "app_insights" {
  name                = "app-insights"
  location            = azurerm_resource_group.rg-hackathon-copa.location
  resource_group_name = azurerm_resource_group.rg-hackathon-copa.name
  workspace_id        = azurerm_log_analytics_workspace.workspace.id
  application_type    = "web"
}

output "api_url" {
  value = "https://${azurerm_linux_web_app.api.default_hostname}"
}
