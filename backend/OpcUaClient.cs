// OpcUaClient.cs
using System;
using System.IO;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

public static class OpcUaClient
{
    private static Session? _session;

    public static async Task ConnectAndSubscribe()
    {
        string endpointUrl = "opc.tcp://localhost:4840";
        Console.WriteLine($"Connecting to {endpointUrl} ...");

        var config = new ApplicationConfiguration
        {
            ApplicationName = "EdgeOpcClient",
            ApplicationUri  = $"urn:{System.Net.Dns.GetHostName()}:EdgeOpcClient",
            ApplicationType = ApplicationType.Client,

            SecurityConfiguration = new SecurityConfiguration
            {
                AutoAcceptUntrustedCertificates = true,
                RejectSHA1SignedCertificates    = false,

                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType   = "Directory",
                    StorePath   = "pki/own",
                    SubjectName = "CN=EdgeOpcClient"
                }
            },

            ClientConfiguration = new ClientConfiguration()
        };

        Directory.CreateDirectory(config.SecurityConfiguration.ApplicationCertificate.StorePath);

        var application = new ApplicationInstance
        {
            ApplicationName          = "EdgeOpcClient",
            ApplicationType          = ApplicationType.Client,
            ApplicationConfiguration = config
        };

        await application.CheckApplicationInstanceCertificate(
            silent: true,
            minimumKeySize: 2048
        );

        config.CertificateValidator = new CertificateValidator();
        config.CertificateValidator.CertificateValidation += (s, e) => e.Accept = true;

        var selectedEndpoint = CoreClientUtils.SelectEndpoint(endpointUrl, false);
        var endpoint         = new ConfiguredEndpoint(null, selectedEndpoint);

        _session = await Session.Create(
            config,
            endpoint,
            false,
            "EdgeOpcSession",
            60000,
            new UserIdentity(new AnonymousIdentityToken()),
            null
        );

        Console.WriteLine("Connected!");
        Console.WriteLine("Subscription starting...\n");

        StartSubscription();
    }

    private static void StartSubscription()
    {
        var subscription = new Subscription(_session.DefaultSubscription)
        {
            PublishingInterval = 1000
        };

        AddMonitoredItem(subscription, "CurrentTemperature", "ns=4;i=6211");
        AddMonitoredItem(subscription, "TargetTemperature",  "ns=4;i=6217");
        AddMonitoredItem(subscription, "Overheated",         "ns=4;i=6214");

        _session.AddSubscription(subscription);
        subscription.Create();

        Console.WriteLine("Subscription active. Listening...\n");
    }

    private static void AddMonitoredItem(Subscription sub, string displayName, string nodeId)
    {
        var item = new MonitoredItem(sub.DefaultItem)
        {
            DisplayName       = displayName,
            StartNodeId       = new NodeId(nodeId),
            SamplingInterval  = 1000,
            QueueSize         = 10,
            DiscardOldest     = true
        };

        item.Notification += OnMonitoredItemNotification;
        sub.AddItem(item);
    }

    private static void OnMonitoredItemNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
    {
        foreach (var value in item.DequeueValues())
        {
            var model = TwinStore.Get();

            if (item.DisplayName == "CurrentTemperature")
                model.CurrentTemperature = Convert.ToDouble(value.Value);

            if (item.DisplayName == "TargetTemperature")
                model.TargetTemperature = Convert.ToDouble(value.Value);

            if (item.DisplayName == "Overheated")
                model.Overheated = Convert.ToBoolean(value.Value);

            model.Timestamp = DateTime.Now;
            TwinStore.Update(model);

            Console.WriteLine($"{model.Timestamp:HH:mm:ss} {item.DisplayName} = {value.Value}");
        }
    }
}
