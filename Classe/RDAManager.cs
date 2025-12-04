using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHub.Classe
{
    internal class RDAManager
    {
        private string appDirectory;
        private string filePath;
        private string filePathE;

        public RDAManager() 
        {
            appDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RemoteHubData");

            if (!Directory.Exists(appDirectory))
            {
                Directory.CreateDirectory(appDirectory);
            }

            filePathE = System.IO.Path.Combine(appDirectory, "RDA.dat");
            filePath = System.IO.Path.Combine(appDirectory, "RDA_decrypted.dat");

            if (File.Exists(filePathE))
            {
                CryptoUtils encryptionManager = new CryptoUtils();
                encryptionManager.DecryptFromFile(filePathE, filePath);
            }
            else
            {
                CreateFile();
                Chiffrement();
            }
        }

        private void CreateFile()
        {
            File.WriteAllText(filePathE, string.Empty);
            File.WriteAllText(filePath, string.Empty);
        }

        private void Chiffrement()
        {
            CryptoUtils encryptionManager = new CryptoUtils();
            encryptionManager.EncryptionToFile(filePath, filePathE);
        }

        #region Traitements des données
        public bool FileExists()
        {
            return File.Exists(filePath);
        }

        public List<RDAEntry> GetAllRDA()
        {
            var sites = new List<RDAEntry>();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { "||" }, StringSplitOptions.None);
                    if (parts.Length == 7)
                    {
                        sites.Add(new RDAEntry
                        {
                            Name = parts[0],
                            Address = parts[1],
                            Username = parts[2],
                            Password = parts[3],
                            Software = parts[4],
                            Features = parts[5],
                            Icon = parts[6]
                        });
                    }
                }
            }
            return sites;
        }
        #endregion

        #region Manipulations des données
        public void AddRDA(string Name, string Address, string Username, string Password, string Software, string Features, string Icon)
        {
            var line = $"{Name}||{Address}||{Username}||{Password}||{Software}||{Features}||{Icon}";
            File.AppendAllLines(filePath, new[] { line });
            Chiffrement();
        }

        public void DeleteRDA(RDAEntry entry)
        {
            if (File.Exists(filePath))
            {
                List<string> lines = File.ReadAllLines(filePath).ToList();
                lines = lines.Where(line => !line.StartsWith(entry.Name + "||")).ToList();
                File.WriteAllLines(filePath, lines);
                Chiffrement();
            }
        }

        public void ModifyRDA(RDAEntry oldEntry, RDAEntry newEntry)
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath).ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    var parts = lines[i].Split(new[] { "||" }, StringSplitOptions.None);
                    if (parts.Length == 7 &&
                        parts[0] == oldEntry.Name &&
                        parts[1] == oldEntry.Address &&
                        parts[2] == oldEntry.Username &&
                        parts[3] == oldEntry.Password &&
                        parts[4] == oldEntry.Software &&
                        parts[5] == oldEntry.Features &&
                        parts[6] == oldEntry.Icon)
                    {
                        lines[i] = $"{newEntry.Name}||{newEntry.Address}||{newEntry.Username}||{newEntry.Password}||{newEntry.Software}||{newEntry.Features}||{newEntry.Icon}";
                        break;
                    }
                }
                File.WriteAllLines(filePath, lines);
                Chiffrement();
            }
        }
        #endregion
    }
}
