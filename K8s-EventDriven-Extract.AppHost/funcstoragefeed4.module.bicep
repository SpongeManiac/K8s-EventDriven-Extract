@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource funcstoragefeed4 'Microsoft.Storage/storageAccounts@2024-01-01' = {
  name: take('funcstoragefeed4${uniqueString(resourceGroup().id)}', 24)
  kind: 'StorageV2'
  location: location
  sku: {
    name: 'Standard_GRS'
  }
  properties: {
    accessTier: 'Hot'
    allowSharedKeyAccess: false
    minimumTlsVersion: 'TLS1_2'
    networkAcls: {
      defaultAction: 'Allow'
    }
  }
  tags: {
    'aspire-resource-name': 'funcstoragefeed4'
  }
}

output blobEndpoint string = funcstoragefeed4.properties.primaryEndpoints.blob

output queueEndpoint string = funcstoragefeed4.properties.primaryEndpoints.queue

output tableEndpoint string = funcstoragefeed4.properties.primaryEndpoints.table

output name string = funcstoragefeed4.name