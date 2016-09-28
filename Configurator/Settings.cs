using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Configurator
{
    public class ClientSetting
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class RappelzSettings
    {
        protected static List<string> clientONLY = new List<string>();
        public static List<ClientSetting> Settings = new List<ClientSetting>();

        internal static string optPath = Path.Combine(Directory.GetCurrentDirectory(), @"rappelz_v1.opt");

        protected static RappelzSettings instance;
        public static RappelzSettings Instance
        {
            get
            {
                if (instance == null) { instance = new RappelzSettings(); }

                return instance;
            }
        }

        public static void ReadOPT_v1()
        {
            try
            {
                using (StreamReader sr = new StreamReader(File.Open(optPath, FileMode.Open, FileAccess.Read)))
                {
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("GRAPHIC_") || line.StartsWith("SOUND_") || line.StartsWith("LOBBY_") || line.StartsWith("SHOW_") || line.StartsWith("PLAY_"))
                        {
                            if (line.Contains('='))
                            {
                                string[] lineBlocks = line.Split('=');

                                Settings.Add(new ClientSetting { Name = lineBlocks[0], Value = lineBlocks[1] });
                            }
                            else
                            {
                                Settings.Add(new ClientSetting { Name = line, Value = null });
                            }
                        }
                        else { if (line != "[Rappelz]") { clientONLY.Add(line); } }
                    }
                }
            }
            catch (Exception Ex) { MessageBox.Show(Ex.ToString(), "OPT Exception #1", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        public static bool Exists(string name) { return Settings.Find(s => s.Name == name) != null ? true : false; }

        public static void Update(string name, string value)
        {
            ClientSetting setting = Settings.Find(s => s.Name == name);
            if (setting != null)
                setting.Value = value;
        }

        public static bool SaveSettings(List<ClientSetting> settings)
        {
            using (StreamWriter sw = new StreamWriter(File.Open(optPath, FileMode.Open, FileAccess.Write), Encoding.Default))
            {
                sw.Write("[RAPPELZ]\n");

                for (int i = 0; i < settings.Count; i++)
                {
                    ClientSetting currentSetting = settings[i];
                    string output = String.Empty;
                    if (currentSetting.Name.Contains('[')) { output = string.Concat(currentSetting.Name, "\n"); }
                    else { output = string.Format("{0}={1}\n", currentSetting.Name, currentSetting.Value); }
                    sw.Write(output);
                }

                foreach (string line in clientONLY)
                {
                    sw.Write(string.Format("{0}\n", line));
                }
            }

            return false;
        }

    }
}
