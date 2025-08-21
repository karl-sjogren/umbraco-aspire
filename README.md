# Umbraco Aspire Example

This repository contains an example of how to set up Umbraco with Aspire
and then doing some extra stuff for fun.

## Umbraco setup

There is an Umbraco project in the `src/Umbraco.Aspire.Umbraco` folder.
This is launched via the Aspire AppHost which sets up a local sql server,
local blob storage and a local redis instance for distributed caching.

It also sets up a frontend project in the `src/Umbraco.Aspire.Frontend`
folder which also launches through Aspire.

## OpenTelemetry

A few extra metrics are reported through OpenTelemetry, these are setup
in `src/Umbraco.Aspire.OpenTelemetry`.

## Aspire commands

A few custom commands have been added to Aspire that suits my current
workflow. These are only available while running locally and not when
publishing.

### Copy blobs from Azure

Available on the storage resource.

Copies all the blobs from a blob storage account in Azure to the local
Azurite instance. The user executing the command needs to be at least
a contributor on the source storage account.

### Launch VSCode MSSQL

Available on the database resource.

Tries to open VSCode with the MSSQL extension and connect to the local
SQL Server instance. If the extensions is not installed nothing will
happen so install that one first.

<https://marketplace.visualstudio.com/items?itemName=ms-mssql.mssql>

### Download bacpac from Azure

Exports an Azure SQL Database as a bacpac file and downloads it to the
local machine. The user executing the command needs to have access
to the database using Entra ID.

### Import downloaded bacpac

Imports a previously downloaded bacpac file (see previous command) into
the local sql server instance. This will delete the current database
and then recreate it, so this might require a restart of the Umbraco
site afterwards.
