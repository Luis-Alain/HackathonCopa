import {
  to = azurerm_resource_group.main
  id = "/subscriptions/${var.subscription_id}/resourceGroups/rg-hachathon-copa"
}

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=5.0.0"
    }
  }
}

# Configure the Microsoft Azure Provider
provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "main" {
  name     = "rg-hachathon-copa"
  location = "westus"
}
