using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Provisioning.Service;
namespace DPS_IND_En
{
    class Program
    {
        static readonly string ServiceConnectionString = "HostName=vvk5cob-dps.azure-devices-provisioning.net;SharedAccessKeyName=provisioningserviceowner;SharedAccessKey=V9zV5xud9xHCyyxnamEz3R350RvXn5WWnqGKKDp+LMU=";
        //static readonly string ServiceConnectionString = "HostName=demo-iot-hub-dps.azure-devices-provisioning.net;SharedAccessKeyName=provisioningserviceowner;SharedAccessKey=Goumkc6QaN4eqDoWIEdbKgWwXlX7DgP/tefyx7Q+dfg=";
        private const string SampleRegistrationId = "mydevice4";
        //        private const string privateKey =
        //                "MIIDYjCCAkqgAwIBAgIQX1R4Qcietq1PaJDa+ZwngTANBgkqhkiG9w0BAQsFADAq" +
        //"MSgwJgYDVQQDDB9BenVyZSBJb1QgQ0EgVGVzdE9ubHkgUm9vdCBDQS0xMB4XDTIw" +
        //"MDYwMzA4MzE0OVoXDTIwMDcwMzA4NDE0OVowFDESMBAGA1UEAwwJbXlkZXZpY2U0" +
        //"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAx0lEuah0DmOz0JG5qqSr" +
        //"tQIJe7Xjz3Po08rXUmZ0oe6lglZ7a3FWOkzKCBetBnF33dvTOCTKn92DGZ77KT8n" +
        //"x8xxM6d4C8qZBjPHokVBn+Of45IIC/PXb5yp1Z/VlNGikL29Q7qJbif46JYM5IjH" +
        //"To8scHmWdGj3j1RwBzLyFryaHxlYQJQi4iqQpX6R0Ga3g819iYUn9fsv6sbfBx1Y" +
        //"MjLSufL5fwG/W/g8AhY1+lWDC69giA/PRJAvm8Bgmu9/kBF2gsXjqsxtKV8tWP2H" +
        //"OL6N9JXelYpSwd/qcWcBXihcXOAZI7Hugl0W/+63HE+jWiBMc7ETv8fDgtcBy4+0" +
        //"1QIDAQABo4GZMIGWMA4GA1UdDwEB/wQEAwIFoDAUBgNVHREEDTALgglteWRldmlj" +
        //"ZTQwHQYDVR0lBBYwFAYIKwYBBQUHAwIGCCsGAQUFBwMBMA8GA1UdEwEB/wQFMAMC" +
        //"AQAwHwYDVR0jBBgwFoAU/O2jBp9UAHqtT43J1OjoeDZNx0EwHQYDVR0OBBYEFC+Z" +
        //"z8IGYLuDPfdMbWL1sfud3x9hMA0GCSqGSIb3DQEBCwUAA4IBAQBo8MD54yAzWS4N" +
        //"RXgAuV1fYcFsk0ZA8Q+wEBKNJRvMlU72qQ2QtWg9f3wFrgMEiqEAXPu72vBBWjT8" +
        //"cIoWo6CYxzJQ88QMv2gFbY74vawnNg5VcUtDcKpit3h/Qvc/e2iuJFZMdEMLswBG" +
        //"b8l+9TubK/R1bgdZCV8OcYUFHPANYKuz6jpebtg/KSxWLBbXHEUt+Rubqd3ZiR/D" +
        //"V1vBBOx4Iq6H28IUaUN65HJ0MgQRkoHMf/wmX6OXEeWdMitEhC7SqQ0kqoOtauqW" +
        //"mAOHAiP5DLB9OTz5c8rsOATgjXZFvqr5rkiFs1Y7BayvObweZnQTUD6t5CRqVt6l" +
        //"F8cBd0FE";
        private const string OptionalDeviceId = "mydevice4";
        private const ProvisioningStatus OptionalProvisioningStatus = ProvisioningStatus.Enabled;
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("IoT Device Provisioning example");

                SetRegistrationDataAsync().GetAwaiter().GetResult();

                Console.WriteLine("Done, hit enter to exit.");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
            Console.ReadLine();
        }
        public static string GetBytesFromPEM(string pemString, string section)
        {
            var header = String.Format("-----BEGIN {0}-----", section);
            var footer = String.Format("-----END {0}-----", section);

            var start = pemString.IndexOf(header, StringComparison.Ordinal);
            if (start < 0)
                return null;

            start += header.Length;
            var end = pemString.IndexOf(footer, start, StringComparison.Ordinal) - start;

            if (end < 0)
                return null;

            pemString = pemString.Substring(start, end);
            pemString = pemString.Replace(header, string.Empty);
            pemString = pemString.Replace(footer, string.Empty);
            pemString = pemString.Substring(2, pemString.Length - 2);
            return pemString.Trim();

        }
        static async Task SetRegistrationDataAsync()
        {

            #region QueryingEnrollemnts
            Console.WriteLine("Fetching Existing Enrollments--------------------------------");
            using var serviceClient = ProvisioningServiceClient.CreateFromConnectionString(ServiceConnectionString);
            QuerySpecification qu = new QuerySpecification("SELECT * FROM enrollments");

            using (Query query = serviceClient.CreateIndividualEnrollmentQuery(qu))
            {
                while (query.HasNext())
                {
                    Console.WriteLine("\nQuerying the next enrollments...");
                    QueryResult queryResult = await query.NextAsync().ConfigureAwait(false);
                    Console.WriteLine(queryResult);
                }
            }
            Console.WriteLine("Enrollment Fetch Complete------------------------------------");
            #endregion

            #region CreateIndividualEnrollment
            Console.WriteLine("Starting SetRegistrationData");

            //Attestation attestation = X509Attestation.CreateFromClientCertificates(privateKey);


            var pem = System.IO.File.ReadAllText(@"D:\Code\Diality\CACerts\CACertificates\mydevice4-public.pem");
            string certBuffer = GetBytesFromPEM(pem, "CERTIFICATE");



            Attestation attestation = X509Attestation.CreateFromClientCertificates(certBuffer);



            IndividualEnrollment individualEnrollment = new IndividualEnrollment(SampleRegistrationId, attestation);

            individualEnrollment.DeviceId = OptionalDeviceId;
            individualEnrollment.ProvisioningStatus = OptionalProvisioningStatus;
            individualEnrollment.IotHubHostName = "vvk5cob-Iot-Hub-2.azure-devices.net";

            Console.WriteLine("\nAdding new individualEnrollment...");

            IndividualEnrollment individualEnrollmentResult =
                await serviceClient.CreateOrUpdateIndividualEnrollmentAsync(individualEnrollment).ConfigureAwait(false);

            Console.WriteLine("\nIndividualEnrollment created with success.");
            Console.WriteLine(individualEnrollmentResult);
            #endregion
        }
    }
}
