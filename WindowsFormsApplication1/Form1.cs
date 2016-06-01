using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualBasic;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        #region MiscStatingStrings
        OpenFileDialog ofd = new OpenFileDialog();
        string ROM;
        string gameCode;
        string gameName;
        string gameType;
        string result1 = "error";
        bool FireRed = false;
        bool Emerald = false;
        bool Incompatible = false;
        #endregion

        #region Functions
        private bool showBackgroundColours = false;
        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
        public static byte[] Combine(byte[] first, byte[] second, byte[] third)
        {
            byte[] ret = new byte[first.Length + second.Length + third.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, ret, first.Length + second.Length,
                             third.Length);
            return ret;
        }
        public static byte[] Combine(params byte[][] arrays)
        {
            byte[] ret = new byte[arrays.Sum(x => x.Length)];
            int offset = 0;
            foreach (byte[] data in arrays)
            {
                Buffer.BlockCopy(data, 0, ret, offset, data.Length);
                offset += data.Length;
            }
            return ret;
        }
        private void WriteData(byte[] BytesToWrite, long Offset)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(ROM));
            bw.BaseStream.Seek(Offset, SeekOrigin.Begin);
            bw.Write(BytesToWrite);
            bw.Close();
        }
        public string DisplayOffset2(long Offset1, long Offset2, string ROM, string result)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(ROM));
            br.BaseStream.Seek(Offset1, SeekOrigin.Begin);
            byte[] vIn = br.ReadBytes(8);
            int vOut = BitConverter.ToInt32(vIn, 0);
            int vOut5 = vOut - 0x8000000;
            br.BaseStream.Seek(Offset2, SeekOrigin.Begin);
            byte[] vIn2 = br.ReadBytes(8);
            int vOut2 = BitConverter.ToInt32(vIn2, 0);
            int vOut3 = vOut2 - 0x8000000;
            result = "0x" + vOut5.ToString("x7").ToUpper() + "//" + "0x" + vOut3.ToString("x7").ToUpper();
            return result;
        }
        public string DisplayOffset(long Offset1, string ROM, string result)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(ROM));
            br.BaseStream.Seek(Offset1, SeekOrigin.Begin);
            byte[] vIn = br.ReadBytes(8);
            int vOut = BitConverter.ToInt32(vIn, 0);
            int vOut2 = vOut - 0x8000000;
            result = "0x" + vOut2.ToString("x7").ToUpper();
            return result;
        }

        public string DisplayOffset3(long Offset1, string ROM, string result)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(ROM));
            br.BaseStream.Seek(Offset1, SeekOrigin.Begin);
            byte[] vIn = br.ReadBytes(8);
            int vOut = BitConverter.ToInt32(vIn, 0);
            int vOut2 = vOut - 0x8000000;
            result = vOut2.ToString("x7").ToUpper();
            return result;
        }

        public string DisplayHeader(string ROM, string result)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(ROM));
            br.BaseStream.Seek(0xac, SeekOrigin.Begin);
            result = Encoding.UTF8.GetString(br.ReadBytes(4));
            return result;
        }
        public string DisplayEndingHeader(string ROM, string result)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(ROM));
            br.BaseStream.Seek(0xfffffe, SeekOrigin.Begin);
            result = Encoding.UTF8.GetString(br.ReadBytes(4));
            return result;
        }
        #endregion

        #region Offsets

        #region FireRedOffsets
        long FRStats = 0x1bc;
        long FRPName = 0x144;
        long FRType = 0x309c8;
        long FRAbill = 0xd8004;
        long FRLMoves = 0x43f84;
        long FRMNames = 0x148;
        long FREggM = 0x045C50;
        long FRTMCom = 0x43c68;
        long FRTMList = 0x125a8c;
        long FREvo = 0x42f6c;
        long FRFront = 0x128;
        long FRBack = 0x12c;
        long FRPal = 0x130;
        long FRSPal = 0x134;
        long FRDex = 0x88e34;
        long FRTutCom = 0x120c30;
        long FREnY = 0x11f4c;
        long FRPlaY = 0x74634;
        long FREnAlt = 0x356f8;
        long FRIconS = 0x138;
        long FRIconP = 0x13c;
        long FRIP = 0x8ab40;
        long FRNDex = 0x4323c;
        long FRFeet = 0x105e14;
        long FRWM = 0xc0330;
        long FRTypeEff = 0x1E944;
        long FROLogo1 = 0x78a98;
        long FROLogo2 = 0x78a9c;
        long FROLogo3 = 0x78A94;
        long FROBack1 = 0x78ab0;
        long FROBack2 = 0x78ab4;
        long FROBack3 = 0x78AAC;
        long FROSprite1 = 0x78aa4;
        long FROSprite2 = 0x78aa8;
        long FROSprite3 = 0x78AA0;
        long FROFlame3 = 0x3bfbbc;
        long FRItems = 0x1C8;
        long FRItemData = 0x1C8;
        #endregion

        #region EmeraldOffsets
        long EMTitleLogo = 0xaa94c;
        long EMTitleLogoRaw = 0xaa958;
        long EMTitleBack = 0xaa95c;
        long EMTitleBackRaw = 0xaa964;
        long EMTitleSprite = 0x540048;
        long EMTitleSpriteRaw = 0xaa98c;
        long EMStats = 0x1bc;
        long EMPName = 0x144;
        long EMType = 0x166f4;
        long EMAbill = 0x1c0;
        long EMLMoves = 0x6930c;
        long EMMNames = 0x148;
        long EMEggM = 0x703F0;
        long EMTMCom = 0x6e048;
        long EMTMList = 0x1b6d10;
        long EMEvo = 0x6d140;
        long EMEMont = 0x128;
        long EMBack = 0x12c;
        long EMPal = 0x130;
        long EMSPal = 0x134;
        long EMDex = 0xbfa20;
        long EMTutCom = 0x1b2390;
        long EMEnY = 0x2dc78;
        long EMPlaY = 0xa5e8c;
        long EMEnAlt = 0x5ee44;
        long EMIconS = 0x138;
        long EMIconP = 0x13c;
        long EMIP = 0xc4208;
        long EMNDex = 0x6d448;
        long EMFeet = 0xc0dbc;
        long EMWM = 0x122e14;
        long EMWM2 = 0x122e14;
        long EMTypeEff = 0x47134;
        long EMItems = 0x1C8;
        #endregion

        #endregion
        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "GBA File (*.gba)|*.gba";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                #region DetermineROM
                string filePath = ofd.FileName;
                BinaryReader br = new BinaryReader(File.OpenRead(ofd.FileName));
                br.BaseStream.Seek(0xAC, SeekOrigin.Begin);
                switch (Encoding.UTF8.GetString(br.ReadBytes(4)))
                {
                    case "BPEE":
                        FireRed = false;
                        Emerald = true;
                        Incompatible = false;
                        break;
                    case "BPRE":
                        FireRed = true;
                        Emerald = false;
                        Incompatible = false;
                        break;
                    default:
                        FireRed = false;
                        Emerald = false;
                        Incompatible = true;
                        break;
                }
                #endregion
                #region FireRed
                if (FireRed == true)
                {
                    Open.Enabled = false;
                    AllOffsets.Enabled = true;
                    FRT.Enabled = true;
                    EMT.Enabled = false;
                    Rom.Text = "Fire Red";
                    ROM = ofd.FileName;
                    HeaderCode.Text = DisplayHeader(ROM, result1);
                    //Poke locations
                    Evo.Text = DisplayOffset(FREvo, ROM, result1);
                    TMCom.Text = DisplayOffset(FRTMCom, ROM, result1);
                    TMList.Text = DisplayOffset(FRTMList, ROM, result1);
                    Eggs.Text = DisplayOffset(FREggM, ROM, result1);
                    ANames.Text = DisplayOffset(FRMNames, ROM, result1);
                    Moves.Text = DisplayOffset(FRLMoves, ROM, result1);
                    Abillities.Text = DisplayOffset(FRAbill, ROM, result1);
                    TNames.Text = DisplayOffset(FRType, ROM, result1);
                    PNames.Text = DisplayOffset(FRPName, ROM, result1);
                    Stats.Text = DisplayOffset(FRStats, ROM, result1);
                    Front.Text = DisplayOffset(FRFront, ROM, result1);
                    Back.Text = DisplayOffset(FRBack, ROM, result1);
                    //Poke Locations 2
                    Normal.Text = DisplayOffset(FRPal, ROM, result1);
                    Shiny.Text = DisplayOffset(FRSPal, ROM, result1);
                    Dex.Text = DisplayOffset(FRDex, ROM, result1);
                    EnY.Text = DisplayOffset(FREnY, ROM, result1);
                    PlaY.Text = DisplayOffset(FRPlaY, ROM, result1);
                    EnAlt.Text = DisplayOffset(FREnAlt, ROM, result1);
                    Tutor.Text = DisplayOffset(FRTutCom, ROM, result1);
                    IconS.Text = DisplayOffset(FRIconS, ROM, result1);
                    IconP.Text = DisplayOffset(FRIconP, ROM, result1);
                    IP.Text = DisplayOffset(FRIP, ROM, result1);
                    NDex.Text = DisplayOffset(FRNDex, ROM, result1);
                    Feet.Text = DisplayOffset(FRFeet, ROM, result1);
                    TypeEff.Text = DisplayOffset(FRTypeEff, ROM, result1);

                    //Mo Offsets
                    Items.Text = DisplayOffset(FRItems, ROM, result1);
                    ItemData.Text = DisplayOffset(FRItemData, ROM, result1);

                    //Title Screen
                    FRSprite1.Text = DisplayOffset(FROSprite1, ROM, result1);
                    FRSprite2.Text = DisplayOffset(FROSprite2, ROM, result1);
                    FRSprite3.Text = DisplayOffset(FROSprite3, ROM, result1);
                    FRLogo1.Text = DisplayOffset(FROLogo1, ROM, result1);
                    FRLogo2.Text = DisplayOffset(FROLogo2, ROM, result1);
                    FRLogo3.Text = DisplayOffset(FROLogo3, ROM, result1);
                    FRBack1.Text = DisplayOffset(FROBack1, ROM, result1);
                    FRBack2.Text = DisplayOffset(FROBack2, ROM, result1);
                    FRBack3.Text = DisplayOffset(FROBack3, ROM, result1);
                    FRFlames1.Text = "NA";
                    FRFlames2.Text = "NA";
                    FRFlames3.Text = DisplayOffset(FROFlame3, ROM, result1);

                    button4.Enabled = true;
                }
                #endregion
                #region Emerald
                if (Emerald == true)
                {
                    Open.Enabled = false;
                    AllOffsets.Enabled = true;
                    EMT.Enabled = true;
                    FRT.Enabled = false;
                    Rom.Text = "Emerald";
                    ROM = ofd.FileName;
                    HeaderCode.Text = DisplayHeader(ROM, result1);
                    //Poke locations
                    Evo.Text = DisplayOffset(EMEvo, ROM, result1);
                    TMCom.Text = DisplayOffset(EMTMCom, ROM, result1);
                    TMList.Text = DisplayOffset(EMTMList, ROM, result1);
                    Eggs.Text = DisplayOffset(EMEggM, ROM, result1);
                    ANames.Text = DisplayOffset(EMMNames, ROM, result1);
                    Moves.Text = DisplayOffset(EMLMoves, ROM, result1);
                    Abillities.Text = DisplayOffset(EMAbill, ROM, result1);
                    TNames.Text = DisplayOffset(EMType, ROM, result1);
                    PNames.Text = DisplayOffset(EMPName, ROM, result1);
                    Stats.Text = DisplayOffset(EMStats, ROM, result1);
                    Front.Text = DisplayOffset(EMEMont, ROM, result1);
                    Back.Text = DisplayOffset(EMBack, ROM, result1);
                    //Poke Locations 2
                    Normal.Text = DisplayOffset(EMPal, ROM, result1);
                    Shiny.Text = DisplayOffset(EMSPal, ROM, result1);
                    Dex.Text = DisplayOffset(EMDex, ROM, result1);
                    EnY.Text = DisplayOffset(EMEnY, ROM, result1);
                    PlaY.Text = DisplayOffset(EMPlaY, ROM, result1);
                    EnAlt.Text = DisplayOffset(EMEnAlt, ROM, result1);
                    Tutor.Text = DisplayOffset(EMTutCom, ROM, result1);
                    IconS.Text = DisplayOffset(EMIconS, ROM, result1);
                    IconP.Text = DisplayOffset(EMIconP, ROM, result1);
                    IP.Text = DisplayOffset(EMIP, ROM, result1);
                    NDex.Text = DisplayOffset(EMNDex, ROM, result1);
                    Feet.Text = DisplayOffset(EMFeet, ROM, result1);
                    TypeEff.Text = DisplayOffset(EMTypeEff, ROM, result1);
                    //Mo Offsets
                    Items.Text = DisplayOffset(EMItems, ROM, result1);
                }
                #endregion
                #region Incompatible
                if (Incompatible == true)
                {
                    Rom.Text = "Incompatible";
                    ROM = ofd.FileName;
                    HeaderCode.Text = "Unsupported " + DisplayHeader(ROM, result1);
                }
                #endregion
                button3.Enabled = true;
                button1.Enabled = true;
                
                br.Close();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/Joexv/Advanced-Offset");
            }
            catch { }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.pokecommunity.com/member.php?u=351718");
            }
            catch { }
        }

        private void Rom_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox15_Enter(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (File.Exists("config.xml"))
            {
                DDXMLWrite(HeaderCode.Text, "pokebasestats", Stats.Text);
                DDXMLWrite(HeaderCode.Text, "pokenames", PNames.Text);
                DDXMLWrite(HeaderCode.Text, "typenames", TNames.Text);
                DDXMLWrite(HeaderCode.Text, "items", Items.Text);
                DDXMLWrite(HeaderCode.Text, "abilities", Abillities.Text);
                DDXMLWrite(HeaderCode.Text, "evolutions", Evo.Text);
                DDXMLWrite(HeaderCode.Text, "attacknames", ANames.Text);
                DDXMLWrite(HeaderCode.Text, "learnedmoves", Moves.Text);
                DDXMLWrite(HeaderCode.Text, "tmhmcompatibility", TMCom.Text);
                DDXMLWrite(HeaderCode.Text, "movetutorcompatibility", Tutor.Text);

                //Dont forget to add in misc numbers like number of attacks, evolutions and the such. Make sure its automatic!!!!
                //Or be lazy and make it have popups..... Probably be lazy
                string result = "";
                long NumberofItemsOff = 0x098998;
                BinaryReader br = new BinaryReader(File.OpenRead(ROM));
                br.BaseStream.Seek(NumberofItemsOff, SeekOrigin.Begin);
                byte[] vIn = br.ReadBytes(4);
                int vOut2 = BitConverter.ToInt32(vIn, 0);
                result = vOut2.ToString().ToUpper();
                DDXMLWrite(HeaderCode.Text, "numberofitems", result);

                string Type1 = Interaction.InputBox("Amount of Types?", "", "18");
                string Moves1 = Interaction.InputBox("Amount of Moves?", "", "355");
                string Abilities1 = Interaction.InputBox("Amount of Abilities?", "", "76");
                string Evolutions1 = Interaction.InputBox("Amount of Evolutions Per Mon?", "", "5");
                DDXMLWrite(HeaderCode.Text, "numberofattacks", Moves1);
                DDXMLWrite(HeaderCode.Text, "numberoftypes", Type1);
                DDXMLWrite(HeaderCode.Text, "evolutionsPerMon", Evolutions1);
                DDXMLWrite(HeaderCode.Text, "numberofabilities", Abilities1);
                string Pokemon = Interaction.InputBox("Amount of Pokemon?", "", "412");
                DDXMLWrite(HeaderCode.Text, "numberofpokes", Pokemon);
                MessageBox.Show("Document edited.");
                br.Close();

            }
            else
            {
                MessageBox.Show("File doesn't exist!");
            }
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void DDXMLCreate(string RomType)
        {


            if (File.Exists("config.xml"))
            {
                MessageBox.Show("File exists!");
            }
            else
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                XmlWriter w = XmlWriter.Create("config.xml", settings);

                if (Emerald == true)
                {
                    w.WriteStartDocument();
                    w.WriteComment("Created by A-Offset");
                    w.WriteStartElement("ROM");
                    w.WriteStartElement(RomType);
                    w.WriteElementString("numberofpokes", "412");
                    w.WriteElementString("pokebasestats", Stats.Text);
                    w.WriteElementString("pokebasestatslength", "0x1C");
                    w.WriteElementString("pokenames", PNames.Text);
                    w.WriteElementString("pokenameslength", "0xB");
                    w.WriteElementString("typenames", TNames.Text);
                    w.WriteElementString("typenameslength", "0x7");
                    w.WriteElementString("numberoftypes", "18");
                    w.WriteElementString("items", Items.Text);
                    w.WriteElementString("numberofitems", "");
                    w.WriteElementString("itemsdatalength", "0x2C");
                    w.WriteElementString("abilities", Abillities.Text);
                    w.WriteElementString("numberofabilities", "78");
                    w.WriteElementString("abiltiesnamelength", "0xD");
                    w.WriteElementString("evolutions", Evo.Text);
                    w.WriteElementString("evolutionsPerMon", "5");
                    w.WriteElementString("evolutionEntryLength", "8");
                    w.WriteElementString("evomethod", "Method 1,Method 2,Method 3,Method 4,Method 5");
                    w.WriteElementString("evotypes", "Breeding Only,Friendship,Friendship(day),Friendship(night),Level - up,Trade,Trade(Item),Item,Atk > Def,Atk = Def,Def > Atk,Wurmple->Silcoon,Wurmple->Cascoon,Nincada->Ninjask,Nincade->Shedinja,Beauty");
                    w.WriteElementString("evoparameter", "None,None,None,None,Level,None,Item,Item,Level,Level,Level,Level,Level,Level,Level,None");
                    w.WriteElementString("evonumber", "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F");
                    w.WriteElementString("attacknames", ANames.Text);
                    w.WriteElementString("numberofattacks", "355");
                    w.WriteElementString("attacksnamelength", "0xD");
                    w.WriteElementString("learnedmoves", Moves.Text);
                    w.WriteElementString("tmhmcompatibility", TMCom.Text);
                    w.WriteElementString("tmlist", "0x616040");
                    w.WriteElementString("tmlistentrylength", "2");
                    w.WriteElementString("numberoftms", "50");
                    w.WriteElementString("numberofhms", "8");
                    w.WriteElementString("movetutorcompatibility", Tutor.Text);
                    w.WriteElementString("movetutorlist", "0x61500C");
                    w.WriteElementString("mtlistentrylength", "2");
                    w.WriteElementString("numberofMTmoves", "32");
                    w.WriteElementString("JamboExpansion", "false");
                    w.WriteElementString("custombattlemusic", "false");
                    w.WriteElementString("hiddenAbility", "false");
                    w.WriteElementString("extendTMHM", "false");
                    w.WriteElementString("MTcomlength", "4");
                }

                if (FireRed == true)
                {
                    w.WriteStartDocument();
                    w.WriteComment("Created by A-Offset");
                    w.WriteStartElement("ROM");
                    w.WriteStartElement(RomType);
                    w.WriteElementString("numberofpokes", "412");
                    w.WriteElementString("pokebasestats", Stats.Text);
                    w.WriteElementString("pokebasestatslength", "0x1C");
                    w.WriteElementString("pokenames", PNames.Text);
                    w.WriteElementString("pokenameslength", "0xB");
                    w.WriteElementString("typenames", TNames.Text);
                    w.WriteElementString("typenameslength", "0x7");
                    w.WriteElementString("numberoftypes", "18");
                    w.WriteElementString("items", Items.Text);
                    w.WriteElementString("numberofitems", "");
                    w.WriteElementString("itemsdatalength", "0x2C");
                    w.WriteElementString("abilities", Abillities.Text);
                    w.WriteElementString("numberofabilities", "78");
                    w.WriteElementString("abiltiesnamelength", "0xD");
                    w.WriteElementString("evolutions", Evo.Text);
                    w.WriteElementString("evolutionsPerMon", "5");
                    w.WriteElementString("evolutionEntryLength", "8");
                    w.WriteElementString("evomethod", "Method 1,Method 2,Method 3,Method 4,Method 5");
                    w.WriteElementString("evotypes", "Breeding Only,Friendship,Friendship(day),Friendship(night),Level - up,Trade,Trade(Item),Item,Atk over Def,Atk equals Def,Def over Atk,Wurmple-Silcoon,Wurmple-Cascoon,Nincada-Ninjask,Nincade-Shedinja,Beauty");
                    w.WriteElementString("evoparameter", "None,None,None,None,Level,None,Item,Item,Level,Level,Level,Level,Level,Level,Level,None");
                    w.WriteElementString("evonumber", "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F");
                    w.WriteElementString("attacknames", ANames.Text);
                    w.WriteElementString("numberofattacks", "355");
                    w.WriteElementString("attacksnamelength", "0xD");
                    w.WriteElementString("learnedmoves", Moves.Text);
                    w.WriteElementString("tmhmcompatibility", TMCom.Text);
                    w.WriteElementString("tmlist", "0x45A80C");
                    w.WriteElementString("tmlistentrylength", "2");
                    w.WriteElementString("numberoftms", "50");
                    w.WriteElementString("numberofhms", "8");
                    w.WriteElementString("movetutorcompatibility", Tutor.Text);
                    w.WriteElementString("movetutorlist", "0x459B60");
                    w.WriteElementString("mtlistentrylength", "2");
                    w.WriteElementString("numberofMTmoves", "15");
                    w.WriteElementString("JamboExpansion", "false");
                    w.WriteElementString("custombattlemusic", "false");
                    w.WriteElementString("hiddenAbility", "false");
                    w.WriteElementString("extendTMHM", "false");
                    w.WriteElementString("MTcomlength", "2");
                }
                w.WriteEndElement();
                w.WriteEndElement();
                w.WriteEndDocument();
                w.Flush();
                w.Close();
                MessageBox.Show("Document created.");
            }

        }

        private void DDXMLWrite(string RomType, string Section, string Value)
        {
            XDocument xmlFile = XDocument.Load("config.xml");
            var query = from c in xmlFile.Elements("ROM").Elements(RomType)
                        select c;
            foreach (XElement book in query)
            {
                book.Element(Section).Value = Value;
            }
            xmlFile.Save("config.xml");
        }
        

        private void button3_Click(object sender, EventArgs e)
        {
            long NumberofItemsOff;
            DDXMLCreate(HeaderCode.Text);
            string result = "";
            if (FireRed == true)
            {
                NumberofItemsOff = 0x098998;
                BinaryReader br = new BinaryReader(File.OpenRead(ROM));
                br.BaseStream.Seek(NumberofItemsOff, SeekOrigin.Begin);
                byte[] vIn = br.ReadBytes(4);
                int vOut2 = BitConverter.ToInt32(vIn, 0);
                result = vOut2.ToString().ToUpper();
                DDXMLWrite(HeaderCode.Text, "numberofitems", result);
                br.Close();
            }

            if (Emerald == true)
            {
                NumberofItemsOff = 0x0;
                BinaryReader br = new BinaryReader(File.OpenRead(ROM));
                br.BaseStream.Seek(NumberofItemsOff, SeekOrigin.Begin);
                byte[] vIn = br.ReadBytes(4);
                int vOut2 = BitConverter.ToInt32(vIn, 0);
                result = vOut2.ToString().ToUpper();
                DDXMLWrite(HeaderCode.Text, "numberofitems", "375");
                br.Close();
            }

            string Type1 = Interaction.InputBox("Amount of Types?", "", "18");
            string Moves1 = Interaction.InputBox("Amount of Moves?", "", "355");
            string Abilities1 = Interaction.InputBox("Amount of Abilities?", "", "76");
            string Evolutions1 = Interaction.InputBox("Amount of Evolutions Per Mon?", "", "5");
            
            DDXMLWrite(HeaderCode.Text, "NumberOfAttacks=", Moves1);
            DDXMLWrite(HeaderCode.Text, "numberoftypes", Type1);
            DDXMLWrite(HeaderCode.Text, "evolutionsPerMon", Evolutions1);
            DDXMLWrite(HeaderCode.Text, "numberofabilities", Abilities1);
            string Pokemon = Interaction.InputBox("Amount of Pokemon?", "", "412");
            DDXMLWrite(HeaderCode.Text, "numberofpokes", Pokemon);

        }

        private void button4_Click(object sender, EventArgs e)
        {



            StreamWriter sw = new StreamWriter(Path.GetFileNameWithoutExtension(ROM) + ".ini", true);
            sw.WriteLine("[BPRE]");
            sw.WriteLine("ROMName=" + Path.GetFileNameWithoutExtension(ROM));

            sw.WriteLine("ItemData=" + DisplayOffset3(FRItemData, ROM, result1));
            sw.WriteLine("AttackNames=" + DisplayOffset3(FRMNames, ROM, result1));
            sw.WriteLine("TMData=" + DisplayOffset3(FRTMList, ROM, result1));
            sw.WriteLine("TotalTMsPlusHMs=58");
            sw.WriteLine("TotalTMs=50");
            sw.WriteLine("ItemIMGData = 3D4294");
            long NumberofItemsOff;
            string result = "";
            if (FireRed == true)
            {
                NumberofItemsOff = 0x098998;
                BinaryReader br = new BinaryReader(File.OpenRead(ROM));
                br.BaseStream.Seek(NumberofItemsOff, SeekOrigin.Begin);
                byte[] vIn = br.ReadBytes(4);
                int vOut2 = BitConverter.ToInt32(vIn, 0);
                result = vOut2.ToString();
                sw.WriteLine("NumberOfItems=" + result);
                br.Close();
            }

            if (Emerald == true)
            {
                NumberofItemsOff = 0x0;
                BinaryReader br = new BinaryReader(File.OpenRead(ROM));
                br.BaseStream.Seek(NumberofItemsOff, SeekOrigin.Begin);
                byte[] vIn = br.ReadBytes(4);
                int vOut2 = BitConverter.ToInt32(vIn, 0);
                result = vOut2.ToString();
                sw.WriteLine("NumberOfItems=375");
                br.Close();
            }
            sw.WriteLine("NumberOfAttacks=" + Interaction.InputBox("Amount of moves?", "", "355"));
            sw.WriteLine("MoveTutorAttacks = 459B60");
            sw.WriteLine("NumberOfMoveTutorAttacks = 16");
            sw.WriteLine("PokemonNames=" + DisplayOffset3(FRPName, ROM, result1));
            sw.WriteLine("NumberOfPokemon=" + Interaction.InputBox("Amount of Pokemon?", "", "412"));
            sw.WriteLine("NationalDexTable=" + DisplayOffset3(FRNDex, ROM, result1));
            sw.WriteLine("SecondDexTable = 251CB8");
            sw.WriteLine("PokedexData=" + DisplayOffset3(FRDex, ROM, result1));
            sw.WriteLine("NumberOfDexEntries = 387");
            sw.WriteLine("PokemonData=" + DisplayOffset3(FRStats, ROM, result1));
            sw.WriteLine("AbilityNames=" + DisplayOffset3(FRAbill, ROM, result1));
            sw.WriteLine("NumberOfAbilities=" + Interaction.InputBox("Amount of Abilities?", "", "76"));
            sw.WriteLine("Pointer2PointersToMapBanks = 5524C");
            sw.WriteLine("OriginalBankPointer0=352004");
            sw.WriteLine("OriginalBankPointer1=352018");
            sw.WriteLine("OriginalBankPointer2=352204");
            sw.WriteLine("OriginalBankPointer3=3522F4");
            sw.WriteLine("OriginalBankPointer4=3523FC");
            sw.WriteLine("OriginalBankPointer5=35240C");
            sw.WriteLine("OriginalBankPointer6=352424");
            sw.WriteLine("OriginalBankPointer7=352444");
            sw.WriteLine("OriginalBankPointer8=35246C");
            sw.WriteLine("OriginalBankPointer9=352484");
            sw.WriteLine("OriginalBankPointer10=3524A4");
            sw.WriteLine("OriginalBankPointer11=3524F4");
            sw.WriteLine("OriginalBankPointer12=35251C");
            sw.WriteLine("OriginalBankPointer13=35253C");
            sw.WriteLine("OriginalBankPointer14=352544");
            sw.WriteLine("OriginalBankPointer15=35256C");
            sw.WriteLine("OriginalBankPointer16=35257C");
            sw.WriteLine("OriginalBankPointer17=352584");
            sw.WriteLine("OriginalBankPointer18=35258C");
            sw.WriteLine("OriginalBankPointer19=352594");
            sw.WriteLine("OriginalBankPointer20=352598");
            sw.WriteLine("OriginalBankPointer21=35259C");
            sw.WriteLine("OriginalBankPointer22=3525A4");
            sw.WriteLine("OriginalBankPointer23=3525AC");
            sw.WriteLine("OriginalBankPointer24=3525B8");
            sw.WriteLine("OriginalBankPointer25=3525C0");
            sw.WriteLine("OriginalBankPointer26=3525CC");
            sw.WriteLine("OriginalBankPointer27=3525D4");
            sw.WriteLine("OriginalBankPointer28=3525D8");
            sw.WriteLine("OriginalBankPointer29=3525DC");
            sw.WriteLine("OriginalBankPointer30=3525E0");
            sw.WriteLine("OriginalBankPointer31=3525E4");
            sw.WriteLine("OriginalBankPointer32=352600");
            sw.WriteLine("OriginalBankPointer33=352614");
            sw.WriteLine("OriginalBankPointer34=352628");
            sw.WriteLine("OriginalBankPointer35=352648");
            sw.WriteLine("OriginalBankPointer36=352668");
            sw.WriteLine("OriginalBankPointer37=35267C");
            sw.WriteLine("OriginalBankPointer38=352690");
            sw.WriteLine("OriginalBankPointer39=352694");
            sw.WriteLine("OriginalBankPointer40=352698");
            sw.WriteLine("OriginalBankPointer41=35269C");
            sw.WriteLine("OriginalBankPointer42=3526A4");
            sw.WriteLine("NumberOfMapsInBank0=4");
            sw.WriteLine("NumberOfMapsInBank1=122");
            sw.WriteLine("NumberOfMapsInBank2=59");
            sw.WriteLine("NumberOfMapsInBank3=65");
            sw.WriteLine("NumberOfMapsInBank4=3");
            sw.WriteLine("NumberOfMapsInBank5=5");
            sw.WriteLine("NumberOfMapsInBank6=7");
            sw.WriteLine("NumberOfMapsInBank7=9");
            sw.WriteLine("NumberOfMapsInBank8=5");
            sw.WriteLine("NumberOfMapsInBank9=7");
            sw.WriteLine("NumberOfMapsInBank10=19");
            sw.WriteLine("NumberOfMapsInBank11=9");
            sw.WriteLine("NumberOfMapsInBank12=7");
            sw.WriteLine("NumberOfMapsInBank13=1");
            sw.WriteLine("NumberOfMapsInBank14=9");
            sw.WriteLine("NumberOfMapsInBank15=3");
            sw.WriteLine("NumberOfMapsInBank16=1");
            sw.WriteLine("NumberOfMapsInBank17=1");
            sw.WriteLine("NumberOfMapsInBank18=1");
            sw.WriteLine("NumberOfMapsInBank19=0");
            sw.WriteLine("NumberOfMapsInBank20=0");
            sw.WriteLine("NumberOfMapsInBank21=1");
            sw.WriteLine("NumberOfMapsInBank22=1");
            sw.WriteLine("NumberOfMapsInBank23=2");
            sw.WriteLine("NumberOfMapsInBank24=1");
            sw.WriteLine("NumberOfMapsInBank25=2");
            sw.WriteLine("NumberOfMapsInBank26=1");
            sw.WriteLine("NumberOfMapsInBank27=0");
            sw.WriteLine("NumberOfMapsInBank28=0");
            sw.WriteLine("NumberOfMapsInBank29=0");
            sw.WriteLine("NumberOfMapsInBank30=0");
            sw.WriteLine("NumberOfMapsInBank31=6");
            sw.WriteLine("NumberOfMapsInBank32=4");
            sw.WriteLine("NumberOfMapsInBank33=4");
            sw.WriteLine("NumberOfMapsInBank34=7");
            sw.WriteLine("NumberOfMapsInBank35=7");
            sw.WriteLine("NumberOfMapsInBank36=4");
            sw.WriteLine("NumberOfMapsInBank37=4");
            sw.WriteLine("NumberOfMapsInBank38=0");
            sw.WriteLine("NumberOfMapsInBank39=0");
            sw.WriteLine("NumberOfMapsInBank40=0");
            sw.WriteLine("NumberOfMapsInBank41=1");
            sw.WriteLine("NumberOfMapsInBank42=0");
            sw.WriteLine("MapLabelData=3F1CAC");
            sw.WriteLine("NumberOfMapLabels=109");
            sw.WriteLine("PokemonFrontSprites=" + DisplayOffset3(FRFront, ROM, result1));
            sw.WriteLine("PokemonBackSprites=" + DisplayOffset3(FRBack, ROM, result1));
            sw.WriteLine("PokemonNormalPal=" + DisplayOffset3(FRPal, ROM, result1));
            sw.WriteLine("PokemonShinyPal=" + DisplayOffset3(FRSPal, ROM, result1));
            sw.WriteLine("IconPointerTable=" + DisplayOffset3(FRIconS, ROM, result1));
            sw.WriteLine("IconPalTable=" + DisplayOffset3(FRIconP, ROM, result1));
            sw.WriteLine("CryTable=48C914"); //
            sw.WriteLine("CryTable2=48DB44"); //
            sw.WriteLine("CryConversionTable=2539D4"); //
            sw.WriteLine("FootPrintTable=" + DisplayOffset3(FRFeet, ROM, result1));
            sw.WriteLine("PokemonAttackTable=" + DisplayOffset3(FRLMoves, ROM, result1));
            sw.WriteLine("PokemonEvolutions=" + DisplayOffset3(FREvo, ROM, result1));
            sw.WriteLine("TMHMCompatibility=" + DisplayOffset3(FRTMCom, ROM, result1));
            sw.WriteLine("TMHMLenPerPoke = 8");
            sw.WriteLine("MoveTutorCompatibility=" + DisplayOffset3(FRTutCom, ROM, result1));
            sw.WriteLine("EnemyYTable=" + DisplayOffset3(FREnY, ROM, result1));
            sw.WriteLine("PlayerYTable=" + DisplayOffset3(FRPlaY, ROM, result1));
            sw.WriteLine("EnemyAltitudeTable=" + DisplayOffset3(FREnAlt, ROM, result1));
            sw.WriteLine("AttackData=250C04"); //
            sw.WriteLine("AttackDescriptionTable=4886E8"); //
            sw.WriteLine("AbilityDescriptionTable=24FB08"); //
            sw.WriteLine("AttackAnimationTable=1C68F4"); //
            sw.WriteLine("IconPals=" + DisplayOffset3(FRIP, ROM, result1));
            sw.WriteLine("JamboLearnableMovesTerm=0000FF");
            sw.WriteLine("StartSearchingForSpaceOffset=71A240");
            sw.WriteLine("FreeSpaceSearchInterval=100");
            sw.WriteLine("NumberOfEvolutionsPerPokemon=" + Interaction.InputBox("Amount of Evolutions Per Mon?", "", "5"));
            sw.WriteLine("NumberOfEvolutionTypes=15");
            sw.WriteLine("EvolutionName0 = None");
            sw.WriteLine("EvolutionName1 = Happiness");
            sw.WriteLine("EvolutionName2 = Happiness(Day)");
            sw.WriteLine("EvolutionName3 = Happiness(Night)");
            sw.WriteLine("EvolutionName4 = Level");
            sw.WriteLine("EvolutionName5 = Trade");
            sw.WriteLine("EvolutionName6 = Trade w/ Item");
            sw.WriteLine("EvolutionName7 = Item");
            sw.WriteLine("EvolutionName8 = Atk>Def");
            sw.WriteLine("EvolutionName9 = Atk=Def");
            sw.WriteLine("EvolutionName10 = Atk<Def");
            sw.WriteLine("EvolutionName11 = High Personality");
            sw.WriteLine("EvolutionName12 = Low Personality");
            sw.WriteLine("EvolutionName13 = Allow Pokemon Creation");
            sw.WriteLine("EvolutionName14 = Create Extra Pokemon");
            sw.WriteLine("EvolutionName15 = Max Beauty");
            sw.WriteLine("Evolution0Param = none");
            sw.WriteLine("Evolution1Param = evolvesbutnoparms");
            sw.WriteLine("Evolution2Param = evolvesbutnoparms");
            sw.WriteLine("Evolution3Param = evolvesbutnoparms");
            sw.WriteLine("Evolution4Param = level");
            sw.WriteLine("Evolution5Param = evolvesbutnoparms");
            sw.WriteLine("Evolution6Param = item");
            sw.WriteLine("Evolution7Param = item");
            sw.WriteLine("Evolution8Param = level");
            sw.WriteLine("Evolution9Param = level");
            sw.WriteLine("Evolution10Param = level");
            sw.WriteLine("Evolution11Param = level");
            sw.WriteLine("Evolution12Param = level");
            sw.WriteLine("Evolution13Param = evolvesbutnoparms");
            sw.WriteLine("Evolution14Param = level");
            sw.WriteLine("Evolution15Param = evolvesbasedonvalue");
            sw.WriteLine("EggMoveTable=" + DisplayOffset3(FREggM, ROM, result1));
            sw.WriteLine("EggMoveTableLimiter=45CC4");
            sw.WriteLine("HabitatTable=452C4C"); //
            sw.WriteLine("ItemAnimationTable=45FD54");
            sw.WriteLine("TrainerTable=23EAF0");
            sw.WriteLine("NumberOfTrainers=742");
            sw.WriteLine("TrainerClasses=23E558");
            sw.WriteLine("NumberOfTrainerClasses=6B");
            sw.WriteLine("TrainerImageTable=23957C");
            sw.WriteLine("NumberOfTrainerImages=147");
            sw.WriteLine("TrainerPaletteTable=239A1C");
            sw.WriteLine("TrainerClassMoney=24F220");
            sw.WriteLine("DexSizeTrainerSprite=135");
            sw.WriteLine("TradeData=26CF8C");
            sw.WriteLine("NumberOfTrades=9");
            sw.WriteLine("PokedexAlphabetTable=443FC0");
            sw.WriteLine("PokedexLightestTable=4442F6");
            sw.WriteLine("PokedexSmallestTable=4445FA");
            sw.WriteLine("PokedexTypeTable=4448FE");
            sw.Flush();
            sw.Close();
            MessageBox.Show("Document created.");
        }
    }
}
