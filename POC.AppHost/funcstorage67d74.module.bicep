@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource funcstorage67d74 'Microsoft.Storage/storageAccounts@2024-01-01' = {
  name: take('funcstorage67d74${uniqueString(resourceGroup().id)}', 24)
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
    'aspire-resource-name': 'funcstorage67d74'
  }
}

output blobEndpoint string = funcstorage67d74.properties.primaryEndpoints.blob

output queueEndpoint string = funcstorage67d74.properties.primaryEndpoints.queue

output tableEndpoint string = funcstorage67d74.properties.primaryEndpoints.table

output name string = funcstorage67d74.name