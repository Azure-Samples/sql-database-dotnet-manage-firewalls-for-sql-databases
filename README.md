---
page_type: sample
languages:
- csharp
products:
- azure
extensions:
- services: Sql
- platforms: dotnet
---

# Getting started on managing firewalls for sql databases in C# #

 Azure Storage sample for managing SQL Database -
  - Create a SQL Server along with 2 firewalls.
  - Add another firewall in the SQL Server
  - List all firewalls.
  - Get a firewall.
  - Update a firewall.
  - Delete a firewall.
  - Add and delete a firewall as part of update of SQL Server
  - Delete Sql Server


## Running this Sample ##

To run this sample:

Set the environment variable `AZURE_AUTH_LOCATION` with the full path for an auth file. See [how to create an auth file](https://github.com/Azure/azure-libraries-for-net/blob/master/AUTH.md).

    git clone https://github.com/Azure-Samples/sql-database-dotnet-manage-firewalls-for-sql-databases.git

    cd sql-database-dotnet-manage-firewalls-for-sql-databases

    dotnet build

    bin\Debug\net452\ManageSqlFirewallRules.exe

## More information ##

[Azure Management Libraries for C#](https://github.com/Azure/azure-sdk-for-net/tree/Fluent)
[Azure .Net Developer Center](https://azure.microsoft.com/en-us/develop/net/)
If you don't have a Microsoft Azure subscription you can get a FREE trial account [here](http://go.microsoft.com/fwlink/?LinkId=330212)

---

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.