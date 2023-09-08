// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager.Samples.Common;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager;
using Azure.ResourceManager.Sql;
using System;
using Azure.ResourceManager.Network.Models;

namespace ManageSqlFirewallRules
{
    public class Program
    {
        private static ResourceIdentifier? _resourceGroupId = null;
        
        /**
         * Azure Storage sample for managing SQL Database -
         *  - Create a SQL Server along with 2 firewalls.
         *  - Add another firewall in the SQL Server
         *  - List all firewalls.
         *  - Get a firewall.
         *  - Update a firewall.
         *  - Delete a firewall.
         *  - Add and delete a firewall as part of update of SQL Server
         *  - Delete Sql Server
         */
        public static async Task RunSample(ArmClient client)
        {            
            try
            {
                //Get default subscription
                SubscriptionResource subscription = await client.GetDefaultSubscriptionAsync();

                //Create a resource group in the EastUS region
                string rgName = Utilities.CreateRandomName("rgSQLServer");
                Utilities.Log("Creating resource group...");
                var rgLro = await subscription.GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, rgName, new ResourceGroupData(AzureLocation.EastUS));
                ResourceGroupResource resourceGroup = rgLro.Value;
                _resourceGroupId = resourceGroup.Id;
                Utilities.Log($"Created a resource group with name: {resourceGroup.Data.Name} ");

                // ============================================================
                // Create a SQL Server, with 2 firewall rules.
                Utilities.Log("Create a SQL server with 2 firewall rules adding a single IP Address and a range of IP Addresses");
                string sqlServerName = Utilities.CreateRandomName("sqlserver");
                Utilities.Log("Creating SQL Server...");
                SqlServerData sqlData = new SqlServerData(AzureLocation.EastUS)
                {
                    AdministratorLogin = "sqladmin" + sqlServerName,
                    AdministratorLoginPassword = Utilities.CreatePassword()
                };
                var sqlServerLro = await resourceGroup.GetSqlServers().CreateOrUpdateAsync(WaitUntil.Completed, sqlServerName, sqlData);
                SqlServerResource sqlServer = sqlServerLro.Value;
                Utilities.Log($"Created a SQL Server with name: {sqlServer.Data.Name} ");

                string rangeFirewallRuleName = Utilities.CreateRandomName("rangefirewallrule-");
                Utilities.Log("Creating 2 firewall rules...");
                SqlFirewallRuleData rangeFirewallRuleData = new SqlFirewallRuleData()
                {
                    StartIPAddress = "10.2.0.1",
                    EndIPAddress = "10.2.0.10"
                };
                var rangeFirewallRuleLro = await sqlServer.GetSqlFirewallRules().CreateOrUpdateAsync(WaitUntil.Completed, rangeFirewallRuleName, rangeFirewallRuleData);
                SqlFirewallRuleResource rangeFirewallRule = rangeFirewallRuleLro.Value;
                Utilities.Log($"Created range ip firewall rule with name {rangeFirewallRule.Data.Name}");

                string singleFirewallRuleName = Utilities.CreateRandomName("singlefirewallrule-");
                SqlFirewallRuleData singleFirewallRuleData = new SqlFirewallRuleData()
                {
                    StartIPAddress = "10.0.0.1",
                    EndIPAddress = "10.0.0.1"
                };
                var sinleFirewallRuleLro = await sqlServer.GetSqlFirewallRules().CreateOrUpdateAsync(WaitUntil.Completed, singleFirewallRuleName, singleFirewallRuleData);
                SqlFirewallRuleResource singleFirewallRule = sinleFirewallRuleLro.Value;
                Utilities.Log($"Created single ip firewall rule with name {singleFirewallRule.Data.Name}");

                // ============================================================
                // List and delete all firewall rules.
                Utilities.Log("Listing all firewall rules in SQL Server.");
                var firewallRules = await sqlServer.GetSqlFirewallRules().GetAllAsync().ToEnumerableAsync();
                foreach (var firewallRule in firewallRules)
                {
                    // Print information of the firewall rule.
                    Utilities.Log($"Listing a firewall rule with name: {firewallRule.Data.Name}");

                    // Delete the firewall rule.
                    Utilities.Log($"Deleting a firewall rule ...");
                    await firewallRule.DeleteAsync(WaitUntil.Completed);
                    Utilities.Log($"Deleted a firewall rule with name: {firewallRule.Data.Name}");
                }

                // ============================================================
                // Add new firewall rules.
                Utilities.Log("Creating a firewall rule in existing SQL Server");
                string newfirewallName = Utilities.CreateRandomName("newfirewallrule");
                SqlFirewallRuleData newfirewallData = new SqlFirewallRuleData()
                {
                    StartIPAddress = "10.10.10.1",
                    EndIPAddress = "10.10.10.10"
                };
                var newfirewallLro = await sqlServer.GetSqlFirewallRules().CreateOrUpdateAsync(WaitUntil.Completed, newfirewallName, newfirewallData);
                SqlFirewallRuleResource newfirewallRule = newfirewallLro.Value;
                Utilities.Log($"Created a new firewall rule for SQL Server with name: {newfirewallRule.Data.Name}");

                Utilities.Log("Get a particular firewall rule in SQL Server");
                SqlFirewallRuleResource getFirewallRuleResult = await sqlServer.GetSqlFirewallRuleAsync(newfirewallRule.Data.Name);
                Utilities.Log($"Get result with id: {getFirewallRuleResult.Data.Id} and name {getFirewallRuleResult.Data.Name}");

                Utilities.Log("Deleting and adding new firewall rules as part of SQL Server update...");
                SqlFirewallRuleData updateFirewallRuleData = new SqlFirewallRuleData()
                {
                    StartIPAddress = "121.12.12.1",
                    EndIPAddress = "121.12.12.10"
                };
                var updateFirewallRule = await newfirewallRule.UpdateAsync(WaitUntil.Completed,updateFirewallRuleData);
                Utilities.Log($"Updated a firewall rule parameter StartIPAddress: {updateFirewallRule.Value.Data.StartIPAddress} and EndIPAddress: {updateFirewallRule.Value.Data.EndIPAddress}");

                var firewallRuleList = await sqlServer.GetSqlFirewallRules().GetAllAsync().ToEnumerableAsync();
                foreach (var sqlFirewallRule in firewallRuleList)
                {
                    // Print information of the firewall rule.
                    Utilities.Log($"Print information of the fire wall rule with id: {sqlFirewallRule.Data.Id} and name: {sqlFirewallRule.Data.Name}");
                }

                // Delete the SQL Server.
                Utilities.Log("Deleting a Sql Server...");
                await sqlServer.DeleteAsync(WaitUntil.Completed);
            }
            finally
            {
                try
                {
                    if (_resourceGroupId is not null)
                    {
                        Utilities.Log($"Deleting Resource Group...");
                        await client.GetResourceGroupResource(_resourceGroupId).DeleteAsync(WaitUntil.Completed);
                        Utilities.Log($"Deleted Resource Group: {_resourceGroupId.Name}");
                    }
                }
                catch (Exception e)
                {
                    Utilities.Log(e);
                }
            }
        }

        public static async Task Main(string[] args)
        {
            try
            {
                //=================================================================
                // Authenticate

                var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
                var tenantId = Environment.GetEnvironmentVariable("TENANT_ID");
                var subscription = Environment.GetEnvironmentVariable("SUBSCRIPTION_ID");
                ClientSecretCredential credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                ArmClient client = new ArmClient(credential, subscription);

                await RunSample(client);
            }
            catch (Exception e)
            {
                Utilities.Log(e);
            }
        }
    }
}