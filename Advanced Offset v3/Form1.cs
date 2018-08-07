using IniParser;
using IniParser.Model;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
       public Form1()
       {
           InitializeComponent();
       }


        public static string filePath = "";
        public static string fullPath = "";
        public static string GameCode = "";
        public static string fileName = "";
        public static string ROM = "";
        bool isFireRed = false;
        bool isEmerald = false;
        private FileIniDataParser fileIniData = new FileIniDataParser();

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "GBA File (*.gba)|*.gba";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filePath = ofd.FileName;
                ofd.Dispose();
                
                ROM = Path.GetFileNameWithoutExtension(filePath);
                fullPath = ofd.FileName;
                fileName = ofd.SafeFileName;
                filePath = fullPath.Replace(fileName, "");

                Rom_Label.Text = fileName;

                BinaryReader br = new BinaryReader(File.OpenRead(fullPath));
                br.BaseStream.Seek(0xAC, SeekOrigin.Begin);
                switch (Encoding.UTF8.GetString(br.ReadBytes(4)))
                {
                    case "BPEE":
                        isFireRed = false;
                        isEmerald = true;
                        GameCode = "BPEE";
                        break;
                    case "BPRE":
                        isFireRed = true;
                        isEmerald = false;
                        GameCode = "BPRE";
                        break;
                    default:
                        isFireRed = true;
                        isEmerald = false;
                        break;
                }
                //br.Close();
                br.Dispose();


                string Game;
                if (isFireRed == true)
                    Game = "FRpge.txt";
                else
                    Game = "EMpge.txt";
                
                comboBox1.SelectedIndex = comboBox1.FindStringExact(Game);
                Export_Button.Enabled = true;
                UpdateDataGrid(Game);
            }
        }

        public void UpdateDataGrid(string txtFile)
        {
            DoubleBuffered(Offset_View, true);
            Offset_View.DataSource = DataTable(txtFile);
        }

        public static void DoubleBuffered(DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        public DataTable DataTable(string txtFile)
        {
            txtFile = Application.StartupPath + "\\Offsets\\" + txtFile;
            DataTable dt = new DataTable();
            dt.Columns.Add(" ");
            dt.Columns.Add("Offset");
            dt.Columns.Add("Location");
            dt.Columns.Add("  ");

            var lines = File.ReadLines(txtFile);
            foreach (var line in lines)
            {
                DataRow dr = dt.NewRow();
                string Offset = line.Split('=').Last();
                string Name = line.Split('=').First();
                //MessageBox.Show(Offset);
                dr[" "] = Name;
                dr["Offset"] = ReadOffset(Offset);
                dr["Location"] = Offset;

                dt.Rows.Add(dr);
            }
            return dt;
        }

        public void MakeINI()
        {
            string Game;
            if (isFireRed == true)
            {
                ExtractINI("FireRed.ini");
                Game = "\\Offsets\\FRpge.txt";
            }
            else
            {
                ExtractINI("Emerald.ini");
                Game = "\\Offsets\\EMpge.txt";
            }
            string fileName = Application.StartupPath + Game;


            var lines = File.ReadLines(fileName);
            foreach (var line in lines)
            {
                string Offset = line.Split('=').Last();
                string Name = line.Split('=').First();
                Offset = PGEoffset(Offset);
                //MessageBox.Show(ROM + ".ini");
                Write(ROM + ".ini", Name, Offset);
            }

            string Item = Interaction.InputBox("Amount of Items?", "", "377");
            string Move = Interaction.InputBox("Amount of Moves?", "", "355");
            string Pokemon = Interaction.InputBox("Amount of Pokemon?", "", "412");
            string Abilities = Interaction.InputBox("Amount of Abilities?", "", "76");
            string Evolution = Interaction.InputBox("Amount of Evolutions Per Mon?", "", "5");
            string TM = Interaction.InputBox("Amount of TMs?", "", "50");

            Write(ROM + ".ini", "NumberOfItems", Item);
            Write(ROM + ".ini", "NumberOfAttacks", Move);
            Write(ROM + ".ini", "NumberOfAbilities", Abilities);
            Write(ROM + ".ini", "NumberOfEvolutions", Evolution);
            Write(ROM + ".ini", "NumberOfPokemon", Pokemon);
            Write(ROM + ".ini", "TotalTMs", TM);

        }

        public string ReadOffset(string Offset)
        {
            long Long = Convert.ToInt64(Offset, 16);
            BinaryReader br = new BinaryReader(File.OpenRead(fullPath));
            br.BaseStream.Seek(Long, SeekOrigin.Begin);
            byte[] bytes = br.ReadBytes(8);
            int vOut = BitConverter.ToInt32(bytes, 0);
            int vOut2 = vOut - 0x8000000;
            string result = "0x" + vOut2.ToString("x7").ToUpper();
            //br.Close();
            br.Dispose();
            return result;
        }

        public string PGEoffset(string Offset)
        {
            long Long = Convert.ToInt64(Offset, 16);
            BinaryReader br = new BinaryReader(File.OpenRead(fullPath));
            br.BaseStream.Seek(Long, SeekOrigin.Begin);
            byte[] bytes = br.ReadBytes(8);
            int vOut = BitConverter.ToInt32(bytes, 0);
            int vOut2 = vOut - 0x8000000;
            string result = vOut2.ToString("x7").ToUpper();
            //br.Close();
            br.Dispose();
            return result;
        }

        public string Get_Data(string ROM, string Object)
        {
            string FileLocation = filePath + ROM + ".ini";
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(FileLocation);
            Object = data[GameCode][Object];
            return Object;
        }

        public void Write(string ROM, string Object, string Value)
        {
            string FileLocation = filePath + ROM;
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(FileLocation);
            data[GameCode][Object] = NullCheck(Value);
            parser.WriteFile(FileLocation, data);
        }

        public string NullCheck(string Object)
        {
            if (Object == "")
                Object = "000000";

            return Object;
        }

        private static void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            try
            {
                Assembly assembly = Assembly.GetCallingAssembly();

                using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
                using (BinaryReader r = new BinaryReader(s))
                using (FileStream fs = new FileStream(outDirectory, FileMode.OpenOrCreate))
                using (BinaryWriter w = new BinaryWriter(fs))
                    w.Write(r.ReadBytes((int)s.Length));
            }
            catch(Exception e)
            {
                MessageBox.Show("Error exctracting file: " + resourceName + Environment.NewLine + e.ToString());
            }



        }

        private static void ExtractOffset(string FileName)
        {

            if(!File.Exists(Application.StartupPath + "\\Offsets\\" + FileName))
                Extract("AdvOffset", Application.StartupPath + "\\Offsets\\" + FileName, "Offsets", FileName);

        }

        private static void ExtractINI(string FileName)
        {
            if (File.Exists(filePath + ROM + ".ini"))
            {
                DialogResult dialogResult = MessageBox.Show("The INI already exists do you want to overwrite it?", "Warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Extract("AdvOffset", filePath + ROM + ".ini", "Offsets", FileName);
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }
            else
                Extract("AdvOffset", filePath + ROM + ".ini", "Offsets", FileName);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Directory.CreateDirectory(Application.StartupPath + @"\Offsets\");
            ExtractOffset("EMpge.txt");
            ExtractOffset("FRpge.txt");
            ExtractOffset("EMmisc.txt");
            ExtractOffset("FRmisc.txt");

            string startPath = Application.StartupPath + @"\Offsets\";
            var txtFiles = Directory.EnumerateFiles(startPath, "*.txt");
            foreach (string currentFile in txtFiles)
            {
                Console.WriteLine(Path.GetFileName(currentFile));
                comboBox1.Items.Add(Path.GetFileName(currentFile));
            }
        }

        private void Export_Button_Click(object sender, EventArgs e)
        {
            MakeINI();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "FRpge.txt" || comboBox1.Text == "EMpge.txt")
                Export_Button.Enabled = true;
            else
                Export_Button.Enabled = false;
            UpdateDataGrid(comboBox1.Text);
        }
    }
}
