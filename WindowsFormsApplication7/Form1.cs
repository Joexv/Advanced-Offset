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

        
namespace WindowsFormsApplication7
{
    public partial class Form1 : Form
    {
        string fileLocation;
        string result1= "error";
        bool FireRed = false;
        bool MrDS = false;
        bool LeafGreen = false;
        bool Emerald = false;
        bool Sapphire = false;
        bool Ruby = false;
        bool other = false;
        ClassXV PokeHelper = new ClassXV();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
                OpenFileDialog ofd = new OpenFileDialog();
                BB.Enabled = false;
                ofd.Filter = "GBA File (*.gba)|*.gba";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    BinaryReader br = new BinaryReader(File.OpenRead(ofd.FileName));
                    br.BaseStream.Seek(0xAC, SeekOrigin.Begin);
                    switch (Encoding.UTF8.GetString(br.ReadBytes(4)))
                    {
                        case "MrDS":
                            FireRed = false;
                            MrDS = true;
                            LeafGreen = false;
                            other = false;
                            Emerald = false;
                            Ruby = false;
                            other = false;
                            break;
                        case "BPRE":
                            FireRed = true;
                            LeafGreen = false;
                            other = false;
                            Emerald = false;
                            Ruby = false;
                            other = false;
                            break;
                        case "BPGE":
                            FireRed = false;
                            LeafGreen = true;
                            other = false;
                            Emerald = false;
                            Ruby = false;
                            other = false;
                            break;
                        case "BPEE":
                            FireRed = false;
                            LeafGreen = false;
                            Emerald = true;
                            Ruby = false;
                            other = false;
                            break;
                        case "AXVE":
                            FireRed = false;
                            LeafGreen = false;
                            Emerald = false;
                            Ruby = true;
                            other = false;
                            break;
                        case "AXPE":
                            FireRed = false;
                            LeafGreen = false;
                            Ruby = true;
                            Sapphire = false;
                            Emerald = false;
                            other = false;
                            break;
                        default:
                            FireRed = false;
                            LeafGreen = false;
                            Ruby = false;
                            Sapphire = false;
                            Emerald = false;
                            other = true;
                            break;
                    }
                    if (other == true)
                    {
                        DialogResult result = MessageBox.Show("This game cannot be identified. If this is a ROM with a custom ID but with the normal english offsets then procede with caution. If it is not, it may cause irreversible damage. Do you wish to continue?", "Warning", MessageBoxButtons.YesNo);
                        if (result == DialogResult.No)
                        {
                            other = false;
                        }
                        DialogResult result2 = MessageBox.Show("If this ROM is Fire Red (BPRE) based then click Yes. If it's Emerald (BPEE) then click no. Else open a new supported ROM with the open button.", "Warning", MessageBoxButtons.YesNo);
                        if (result2 == DialogResult.No)
                        {
                            Emerald = true;
                            other = false;
                        }
                        if (result2 == DialogResult.Yes)
                        {
                            FireRed = true;
                            other = false;
                        }
                    }
                    if (MrDS == true)
                    {
                        button1.Enabled = false;
                        long FRTitleLogo = 0x78a98;
                        long FRTitleLogoRaw = 0x78a9c;
                        long FRTitleBack = 0x78ab0;
                        long FRTitleBackRaw = 0x78ab4;
                        long FRTitleSprite = 0x78aa4;
                        long FRTitleSpriteRaw = 0x78aa8;
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
                        AllOffsets.Enabled = true;
                        Rom.Text = "MrDollSteak Base";
                        fileLocation = ofd.FileName;
                        HeaderCode.Text = PokeHelper.DisplayHeader(fileLocation, result1);
                        //Logo
                        br.BaseStream.Seek(FRTitleLogoRaw, SeekOrigin.Begin);
                        PLogo.Text = PokeHelper.DisplayOffset2(FRTitleLogo, FRTitleLogoRaw, fileLocation, result1);
                        Version.Enabled = false;
                        TSprite.Enabled = true;
                        //Titlebackground
                        TBack.Text = PokeHelper.DisplayOffset2(FRTitleBack, FRTitleBackRaw, fileLocation, result1);
                        //TitleSprite
                        TSprite.Text = PokeHelper.DisplayOffset2(FRTitleSprite, FRTitleSpriteRaw, fileLocation, result1);
                        //Poke locations
                        Evo.Text = PokeHelper.DisplayOffset1(FREvo, fileLocation, result1);
                        TMCom.Text = PokeHelper.DisplayOffset1(FRTMCom, fileLocation, result1);
                        TMList.Text = PokeHelper.DisplayOffset1(FRTMList, fileLocation, result1);
                        Eggs.Text = PokeHelper.DisplayOffset1(FREggM, fileLocation, result1);
                        ANames.Text = PokeHelper.DisplayOffset1(FRMNames, fileLocation, result1);
                        Moves.Text = PokeHelper.DisplayOffset1(FRLMoves, fileLocation, result1);
                        Abillities.Text = PokeHelper.DisplayOffset1(FRAbill, fileLocation, result1);
                        TNames.Text = PokeHelper.DisplayOffset1(FRType, fileLocation, result1);
                        PNames.Text = PokeHelper.DisplayOffset1(FRPName, fileLocation, result1);
                        Stats.Text = PokeHelper.DisplayOffset1(FRStats, fileLocation, result1);
                        Front.Text = PokeHelper.DisplayOffset1(FRFront, fileLocation, result1);
                        Back.Text = PokeHelper.DisplayOffset1(FRBack, fileLocation, result1);
                        //Poke Locations 2
                        Normal.Text = PokeHelper.DisplayOffset1(FRPal, fileLocation, result1);
                        Shiny.Text = PokeHelper.DisplayOffset1(FRSPal, fileLocation, result1);
                        Dex.Text = PokeHelper.DisplayOffset1(FRDex, fileLocation, result1);
                        EnY.Text = PokeHelper.DisplayOffset1(FREnY, fileLocation, result1);
                        PlaY.Text = PokeHelper.DisplayOffset1(FRPlaY, fileLocation, result1);
                        EnAlt.Text = PokeHelper.DisplayOffset1(FREnAlt, fileLocation, result1);
                        Tutor.Text = PokeHelper.DisplayOffset1(FRTutCom, fileLocation, result1);
                        IconS.Text = PokeHelper.DisplayOffset1(FRIconS, fileLocation, result1);
                        IconP.Text = PokeHelper.DisplayOffset1(FRIconP, fileLocation, result1);
                        IP.Text = PokeHelper.DisplayOffset1(FRIP, fileLocation, result1);
                        NDex.Text = PokeHelper.DisplayOffset1(FRNDex, fileLocation, result1);
                        Feet.Text = PokeHelper.DisplayOffset1(FRFeet, fileLocation, result1);
                        WM.Text = PokeHelper.DisplayOffset1(FRWM, fileLocation, result1);
                    }
                    if (FireRed == true)
                    {
                        button1.Enabled = false;
                        long FRTitleLogo = 0x78a98;
                        long FRTitleLogoRaw = 0x78a9c;
                        long FRTitleBack = 0x78ab0;
                        long FRTitleBackRaw = 0x78ab4;
                        long FRTitleSprite = 0x78aa4;
                        long FRTitleSpriteRaw = 0x78aa8;
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
                        AllOffsets.Enabled = true;
                        Rom.Text = "Fire Red";
                        fileLocation = ofd.FileName;
                        HeaderCode.Text = PokeHelper.DisplayHeader(fileLocation, result1);
                        //Logo
                        br.BaseStream.Seek(FRTitleLogoRaw, SeekOrigin.Begin);
                        PLogo.Text = PokeHelper.DisplayOffset2(FRTitleLogo, FRTitleLogoRaw, fileLocation, result1);
                        Version.Enabled = false;
                        TSprite.Enabled = true;
                        //Titlebackground
                        TBack.Text = PokeHelper.DisplayOffset2(FRTitleBack, FRTitleBackRaw, fileLocation, result1);
                        //TitleSprite
                        TSprite.Text = PokeHelper.DisplayOffset2(FRTitleSprite, FRTitleSpriteRaw, fileLocation, result1);
                        //Poke locations
                        Evo.Text = PokeHelper.DisplayOffset1(FREvo, fileLocation, result1);
                        TMCom.Text = PokeHelper.DisplayOffset1(FRTMCom, fileLocation, result1);
                        TMList.Text = PokeHelper.DisplayOffset1(FRTMList, fileLocation, result1);
                        Eggs.Text = PokeHelper.DisplayOffset1(FREggM, fileLocation, result1);
                        ANames.Text = PokeHelper.DisplayOffset1(FRMNames, fileLocation, result1);
                        Moves.Text = PokeHelper.DisplayOffset1(FRLMoves, fileLocation, result1);
                        Abillities.Text = PokeHelper.DisplayOffset1(FRAbill, fileLocation, result1);
                        TNames.Text = PokeHelper.DisplayOffset1(FRType, fileLocation, result1);
                        PNames.Text = PokeHelper.DisplayOffset1(FRPName, fileLocation, result1);
                        Stats.Text = PokeHelper.DisplayOffset1(FRStats, fileLocation, result1);
                        Front.Text = PokeHelper.DisplayOffset1(FRFront, fileLocation, result1);
                        Back.Text = PokeHelper.DisplayOffset1(FRBack, fileLocation, result1);
                        //Poke Locations 2
                        Normal.Text = PokeHelper.DisplayOffset1(FRPal, fileLocation, result1);
                        Shiny.Text = PokeHelper.DisplayOffset1(FRSPal, fileLocation, result1);
                        Dex.Text = PokeHelper.DisplayOffset1(FRDex, fileLocation, result1);
                        EnY.Text = PokeHelper.DisplayOffset1(FREnY, fileLocation, result1);
                        PlaY.Text = PokeHelper.DisplayOffset1(FRPlaY, fileLocation, result1);
                        EnAlt.Text = PokeHelper.DisplayOffset1(FREnAlt, fileLocation, result1);
                        Tutor.Text = PokeHelper.DisplayOffset1(FRTutCom, fileLocation, result1);
                        IconS.Text = PokeHelper.DisplayOffset1(FRIconS, fileLocation, result1);
                        IconP.Text = PokeHelper.DisplayOffset1(FRIconP, fileLocation, result1);
                        IP.Text = PokeHelper.DisplayOffset1(FRIP, fileLocation, result1);
                        NDex.Text = PokeHelper.DisplayOffset1(FRNDex, fileLocation, result1);
                        Feet.Text = PokeHelper.DisplayOffset1(FRFeet, fileLocation, result1);
                        WM.Text = PokeHelper.DisplayOffset1(FRWM, fileLocation, result1);
                    }
                    if (LeafGreen == true)
                    {
                        Rom.Text = "Leaf Green";
                        fileLocation = ofd.FileName;
                        HeaderCode.Text = "Unsupported " + PokeHelper.DisplayHeader(fileLocation, result1);
                    }
                    if (other == true)
                    {
                        Rom.Text = "Unidentified game";
                        fileLocation = ofd.FileName;
                        HeaderCode.Text = "Unsupported " + PokeHelper.DisplayHeader(fileLocation, result1);
                    }
                    if (Emerald == true)
                    {
                        button1.Enabled = false;
                        long EMTitleLogo = 0xaa94c;
                        long EMTitleLogoRaw = 0xaa958;
                        long EMTitleBack = 0xaa95c;
                        long EMTitleBackRaw = 0xaa964;
                        long EMTitleSprite = 0x540048;
                        long EMTitleSpriteRaw = 0xaa98c;
                        long EMStats = 0x1bc;//
                        long EMPName = 0x144;//
                        long EMType = 0x166f4;//
                        long EMAbill = 0x1c0; //
                        long EMLMoves = 0x6930c;//
                        long EMMNames = 0x148; //
                        long EMEggM = 0x703F0; //
                        long EMTMCom = 0x6e048; //
                        long EMTMList = 0x1b6d10; //
                        long EMEvo = 0x6d140; //
                        long EMEMont = 0x128; //
                        long EMBack = 0x12c; //
                        long EMPal = 0x130; //
                        long EMSPal = 0x134; //
                        long EMDex = 0xbfa20; //
                        long EMTutCom = 0x1b2390; //
                        long EMEnY = 0x2dc78; //
                        long EMPlaY = 0xa5e8c; //
                        long EMEnAlt = 0x5ee44; //
                        long EMIconS = 0x138; //
                        long EMIconP = 0x13c; //
                        long EMIP = 0xc4208; //
                        long EMNDex = 0x6d448; //
                        long EMFeet = 0xc0dbc; //
                        long EMWM = 0x122e14;
                        long EMWM2 = 0x122e14;
                        AllOffsets.Enabled = true;
                        Rom.Text = "Emerald";
                        fileLocation = ofd.FileName;
                        HeaderCode.Text = PokeHelper.DisplayHeader(fileLocation, result1);
                        //Logo
                        br.BaseStream.Seek(EMTitleLogoRaw, SeekOrigin.Begin);
                        PLogo.Text = PokeHelper.DisplayOffset2(EMTitleLogo, EMTitleLogoRaw, fileLocation, result1);
                        Version.Text = PokeHelper.DisplayOffset2(EMTitleSprite, EMTitleSpriteRaw, fileLocation, result1);
                        Version.Enabled = false;
                        //Titlebackground
                        TBack.Text = PokeHelper.DisplayOffset2(EMTitleBack, EMTitleBackRaw, fileLocation, result1);
                        //TitleSprite
                        TSprite.Enabled = false;
                        //Poke locations
                        Evo.Text = PokeHelper.DisplayOffset1(EMEvo, fileLocation, result1);
                        TMCom.Text = PokeHelper.DisplayOffset1(EMTMCom, fileLocation, result1);
                        TMList.Text = PokeHelper.DisplayOffset1(EMTMList, fileLocation, result1);
                        Eggs.Text = PokeHelper.DisplayOffset1(EMEggM, fileLocation, result1);
                        ANames.Text = PokeHelper.DisplayOffset1(EMMNames, fileLocation, result1);
                        Moves.Text = PokeHelper.DisplayOffset1(EMLMoves, fileLocation, result1);
                        Abillities.Text = PokeHelper.DisplayOffset1(EMAbill, fileLocation, result1);
                        TNames.Text = PokeHelper.DisplayOffset1(EMType, fileLocation, result1);
                        PNames.Text = PokeHelper.DisplayOffset1(EMPName, fileLocation, result1);
                        Stats.Text = PokeHelper.DisplayOffset1(EMStats, fileLocation, result1);
                        Front.Text = PokeHelper.DisplayOffset1(EMEMont, fileLocation, result1);
                        Back.Text = PokeHelper.DisplayOffset1(EMBack, fileLocation, result1);
                        //Poke Locations 2
                        Normal.Text = PokeHelper.DisplayOffset1(EMPal, fileLocation, result1);
                        Shiny.Text = PokeHelper.DisplayOffset1(EMSPal, fileLocation, result1);
                        Dex.Text = PokeHelper.DisplayOffset1(EMDex, fileLocation, result1);
                        EnY.Text = PokeHelper.DisplayOffset1(EMEnY, fileLocation, result1);
                        PlaY.Text = PokeHelper.DisplayOffset1(EMPlaY, fileLocation, result1);
                        EnAlt.Text = PokeHelper.DisplayOffset1(EMEnAlt, fileLocation, result1);
                        Tutor.Text = PokeHelper.DisplayOffset1(EMTutCom, fileLocation, result1);
                        IconS.Text = PokeHelper.DisplayOffset1(EMIconS, fileLocation, result1);
                        IconP.Text = PokeHelper.DisplayOffset1(EMIconP, fileLocation, result1);
                        IP.Text = PokeHelper.DisplayOffset1(EMIP, fileLocation, result1);
                        NDex.Text = PokeHelper.DisplayOffset1(EMNDex, fileLocation, result1);
                        Feet.Text = PokeHelper.DisplayOffset1(EMFeet, fileLocation, result1);
                        //worldmap
                        WM.Text = PokeHelper.DisplayOffset1(EMWM, fileLocation, result1);
                    }
                    if (Ruby == true)
                    {
                        Rom.Text = "Ruby";
                        fileLocation = ofd.FileName;
                        HeaderCode.Text = "Unsupported " + PokeHelper.DisplayHeader(fileLocation, result1);
                    }
                    if (Sapphire == true)
                    {
                        Rom.Text = "sapphire";
                        fileLocation = ofd.FileName;
                        HeaderCode.Text = "Unsupported " + PokeHelper.DisplayHeader(fileLocation, result1);
                    }
                    br.Close();
                }
            
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void BB_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

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
    }
}
