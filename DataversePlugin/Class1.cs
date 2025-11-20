using System;
using Microsoft.Xrm.Sdk;

public class SamplePlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        // Standard pattern to get the execution context
        IPluginExecutionContext context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));

        // Tracing service (useful for plugin logs)
        ITracingService tracing = (ITracingService)
            serviceProvider.GetService(typeof(ITracingService));

        // Org service factory to perform CRUD if needed
        IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)
            serviceProvider.GetService(typeof(IOrganizationServiceFactory));
        IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

        tracing.Trace("SamplePlugin: Execute started.");

        // Guard clauses
        if (context == null)
        {
            tracing.Trace("Context is null, exiting.");
            return;
        }

        // Example: only run on Create of contact
        if (context.MessageName != "Create" || context.PrimaryEntityName != "contact")
        {
            tracing.Trace($"Skipping: Message={context.MessageName}, Entity={context.PrimaryEntityName}");
            return;
        }

        // Get Target entity if present
        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity target)
        {
            tracing.Trace("Target found.");

            if (target.Attributes.Contains("mobilephone"))
            {
                var phone = target["mobilephone"]?.ToString();
                if (!string.IsNullOrEmpty(phone))
                {
                    // Example logic: set a description on the same entity before save
                    target["description"] = $"Processed phone: {phone}";
                    tracing.Trace($"Set description to: Processed phone: {phone}");
                }
            }
        }

        tracing.Trace("SamplePlugin: Execute finished.");
    }
}
