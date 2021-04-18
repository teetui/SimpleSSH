using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleSSH
{
    class Login
    {
        public string Host = string.Empty;
        public string UserID = string.Empty;
        public string Port = "22";
        public string Password = string.Empty;
        private bool HasKeyFile = false;
        private SshClient client;

        public void SetKeyFile(StorageFile keyFile)
        {
            Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFileToken", keyFile);
            HasKeyFile = true;
        }

        public async Task<StorageFile> GetKeyFile()
        {
            StorageFile keyFile = await Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.GetFileAsync("PickedFileToken");

            return keyFile;
        }

        public bool Connect()
        {
            if (Host == string.Empty || UserID == string.Empty)
                return false;
            if (Password == string.Empty && HasKeyFile == false)
                return false;

            try
            {
                ConnectionInfo connectionInfo;
                if (HasKeyFile)
                    connectionInfo = new ConnectionInfo(Host, UserID, 
                        new PrivateKeyAuthenticationMethod(UserID, new PrivateKeyFile(GetKeyFile().Result.Path)));
                else
                    connectionInfo = new ConnectionInfo(Host, UserID,
                        new PasswordAuthenticationMethod(UserID, Password));

                client = new SshClient(connectionInfo);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return true;
        }

        public string Command(string command)
        {
            SshCommand result;
            if (Connect())
            {
                client.Connect();
                result = client.RunCommand(command);
                client.Disconnect();
            }
            else
                return "Connection failed.";

            return result.Result;
        }
    }
}
