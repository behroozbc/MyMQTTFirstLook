using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SharedItems
{
    public class ConfigCertificate
    {
        public static X509Certificate GetClientCrtCertificate()
        {
            var currectPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var certificationPath = Path.Combine(currectPath, "Assets/key/client.crt");
            return X509Certificate2.CreateFromCertFile(certificationPath);
        }
        public static X509Certificate2 GetCACrtCertificate()
        {
            return new X509Certificate2(File.ReadAllBytes("Assets/key/ca.crt"));
        }
        public static X509Certificate2 GetCrtWithPriavteKey()
        {
            var pubonly = new X509Certificate2(File.ReadAllBytes("Assets/key/server.crt"));
            using var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText("Assets/key/server.key"));
            return pubonly.CopyWithPrivateKey(rsa);
        }

    }
}
