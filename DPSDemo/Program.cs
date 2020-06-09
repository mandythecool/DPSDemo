using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Provisioning.Service;
using System.Security.Cryptography.X509Certificates;

namespace DPSDemo
{
    class Program
    {
        private static string ProvisioningConnectionString = "HostName=demo-iot-hub-dps.azure-devices-provisioning.net;SharedAccessKeyName=provisioningserviceowner;SharedAccessKey=Goumkc6QaN4eqDoWIEdbKgWwXlX7DgP/tefyx7Q+dfg=";
        private static string EnrollmentGroupId = "enrollmentgroup1";
        private static string X509RootCertPath = @"C:\Users\msh8cob\Desktop\CACertificates\RootCA.cer";
        static void Main(string[] args)
        {
            RunSample().GetAwaiter().GetResult();
            Console.WriteLine("\nHit <Enter> to exit ...");
            Console.ReadLine();
        }

        public static async Task RunSample()
        {
            Console.WriteLine("Starting sample...");

            using (ProvisioningServiceClient provisioningServiceClient =
                    ProvisioningServiceClient.CreateFromConnectionString(ProvisioningConnectionString))
            {
                #region Create a new enrollmentGroup config
                Console.WriteLine("\nCreating a new enrollmentGroup...");
                var certificate = new X509Certificate2(X509RootCertPath);
                Attestation attestation = X509Attestation.CreateFromRootCertificates(certificate);
                EnrollmentGroup enrollmentGroup =
                        new EnrollmentGroup(
                                EnrollmentGroupId,
                                attestation)
                        {
                            ProvisioningStatus = ProvisioningStatus.Enabled
                        };
                Console.WriteLine(enrollmentGroup);
                #endregion

                #region Create the enrollmentGroup
                Console.WriteLine("\nAdding new enrollmentGroup...");
                EnrollmentGroup enrollmentGroupResult =
                    await provisioningServiceClient.CreateOrUpdateEnrollmentGroupAsync(enrollmentGroup).ConfigureAwait(false);
                Console.WriteLine("\nEnrollmentGroup created with success.");
                Console.WriteLine(enrollmentGroupResult);
                #endregion

            }
        }
    }
}