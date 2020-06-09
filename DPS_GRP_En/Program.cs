using Microsoft.Azure.Devices.Provisioning.Service;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DPS_GRP_En
{
    class Program
    {
        static readonly string ServiceConnectionString = "HostName=demo-iot-hub-dps.azure-devices-provisioning.net;SharedAccessKeyName=provisioningserviceowner;SharedAccessKey=Goumkc6QaN4eqDoWIEdbKgWwXlX7DgP/tefyx7Q+dfg=";

        private const string SampleRegistrationId = "sample-individual-csharp";
      
        private const string OptionalDeviceId = "myCSharpDevice";
        private const ProvisioningStatus OptionalProvisioningStatus = ProvisioningStatus.Enabled;
        private const string X509RootCertPathVar = "{X509 Certificate Location}";
        private const string SampleEnrollmentGroupId = "sample-group-csharp";
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("IoT Device Group Provisioning example");

                SetGroupRegistrationDataAsync().GetAwaiter().GetResult();

                Console.WriteLine("Done, hit enter to exit.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }
        public static async Task SetGroupRegistrationDataAsync()
        {
            Console.WriteLine("Starting SetGroupRegistrationData");

            using (ProvisioningServiceClient provisioningServiceClient =
                    ProvisioningServiceClient.CreateFromConnectionString(ServiceConnectionString))
            {
                Console.WriteLine("\nCreating a new enrollmentGroup...");

                var certificate = new X509Certificate2(X509RootCertPathVar);

                Attestation attestation = X509Attestation.CreateFromRootCertificates(certificate);

                EnrollmentGroup enrollmentGroup = new EnrollmentGroup(SampleEnrollmentGroupId, attestation);

                Console.WriteLine(enrollmentGroup);
                Console.WriteLine("\nAdding new enrollmentGroup...");

                EnrollmentGroup enrollmentGroupResult =
                    await provisioningServiceClient.CreateOrUpdateEnrollmentGroupAsync(enrollmentGroup).ConfigureAwait(false);

                Console.WriteLine("\nEnrollmentGroup created with success.");
                Console.WriteLine(enrollmentGroupResult);
            }
        }
    }
}
