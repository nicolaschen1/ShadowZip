/*
SHADOWZIP
VERSION: 2.0

Inputs: Files

Outputs: ZIP file

Description: This software tool allows to make a ZIP file (archiving and data compression).

Developer: Nicolas CHEN
*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Ionic.Zip;
using System.Text;
using System.IO;
using System.Threading;

namespace ShadowZip
{
    public partial class Form1 : Form
    {
        List<string> listSelectedFile = new List<string>();
        string zipPassword;
        string zipName;
        string zipDestinationPath;
        bool zipStatus;

        public Form1()
        {
            Thread t = new Thread(new ThreadStart(Splash));
            t.Start();
            Thread.Sleep(3000);
            InitializeComponent();
            t.Abort();
        }

        public void Splash()
        {
            SplashScreen.SplashForm frm = new SplashScreen.SplashForm();
            frm.AppName = "ShadowZip";
            frm.Icon = Properties.Resources.zip;
            frm.ShowIcon = true;
            frm.ShowInTaskbar = true;
            Application.Run(frm);
        }

        /*** BUTTONS ***/
        //File Dialog Button
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                BrowserTextBox.Enabled = true;
                BrowserTextBox.Text = openFileDialog1.FileName.ToString();

                SelectedFileList.Items.Add(openFileDialog1.FileName);
            }
        }

        //Button to delete an item
        private void button3_Click(object sender, EventArgs e)
        {
            SelectedFileList.Items.RemoveAt(SelectedFileList.SelectedIndex);
        }

        //Folder Browser Dialog Button for the destination location
        private void button4_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                DestinationTextBox.Enabled = true;
                DestinationTextBox.Text = folderBrowserDialog1.SelectedPath.ToString();
            }
        }

        //Button to make a ZIP file
        private void ShadowZipButton_Click(object sender, EventArgs e)
        {
            foreach (object item in SelectedFileList.Items)
                listSelectedFile.Add(item.ToString());

            zipDestinationPath = DestinationTextBox.Text;
            zipName = ZipTextBox.Text;

            if (VerifyTextBox(BrowserTextBox.Text, PasswordTextBox.Text, ZipTextBox.Text, DestinationTextBox.Text))
            {
                zipStatus = RunShadowZip(listSelectedFile, zipPassword, zipDestinationPath, zipName);

                if (zipStatus == true)
                {
                    MessageBox.Show("ZIP COMPLETED.", "Success");
                    Reset();
                }
                else
                {
                    MessageBox.Show("ZIP FAILED.", "Fail", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                MessageBox.Show("Please fill all the blanks.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                listSelectedFile.Clear();
            }
        }

        /*** MENU BAR ***/
        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] about = new string[]
            {
                "ShadowZip"
                , ""
                , "VERSION: 2.0"
                , ""
                , "Developed by Nicolas Chen"
            };

            MessageBoxMultiLines(about);
        }

        private void instructionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] instructions = new string[]
            {
                "SHADOWZIP is a software tool allows to make a ZIP file (archiving and data compression). "
                , ""
                , "ZIP MODE:"
                , "1) Select your file. If there are several files, add these files one by one."
                , ""
                , "2) Select your destination location."
                , ""
                , "3) Enter a name for the Zip file."
                , ""
                , "4) Click on the ZIP button."
                , ""
                , "Password Mode: If a password must be added in the ZIP file follow the steps below:"
                , ""
                , "a) Check 'Add a Password' to activete Password Mode."
                , ""
                , "b) Type your password (password is hidden by '*' and limited to 14 characters)."
                , ""
                , "c) Click on the padlock to assign your password."
                , ""
                , "UNZIP MODE"
                , "1) Select your Zip file. If there are several files, add these files one by one."
                , ""
                , "2) Select your destination location."
                , ""
                , "3) Click on the UNZIP button."
            };

            MessageBoxMultiLines(instructions);
        }


        /*** METHODS ***/
        //Verify if all textboxes are filled
        public bool VerifyTextBox(String BrowserTextBox, String PasswordTextBox, String ZipTextBox, String DestinationTextBox)
        {
            if (radioButtonZip.Enabled && !radioButtonUnZip.Enabled)
            {
                return !(String.IsNullOrEmpty(BrowserTextBox) || (String.IsNullOrEmpty(PasswordTextBox) && IsPassword.Checked) || String.IsNullOrEmpty(ZipTextBox) || String.IsNullOrEmpty(DestinationTextBox));
            }
            else
            {
                return !(String.IsNullOrEmpty(BrowserTextBox) || (String.IsNullOrEmpty(PasswordTextBox) && IsPassword.Checked) || String.IsNullOrEmpty(DestinationTextBox));
            }            
        }

        //Reset all
        public void Reset()
        {
            SelectedFileList.Items.Clear();
            listSelectedFile.Clear();
            ZipTextBox.Text = String.Empty;
            DestinationTextBox.Text = String.Empty;
            PasswordTextBox.Text = String.Empty;
            BrowserTextBox.Text = String.Empty;
            IsPassword.Checked = false;
        }

        //Make the ZIP file
        public static bool RunShadowZip(List<string> listSelectedFile, string zipPassword, string DestinationPath, string ZipName)
        {
            if (listSelectedFile != null)
            {

                using (ZipFile shadowZip = new ZipFile())
                {
                    shadowZip.Password = zipPassword;
                    foreach (var sFile in listSelectedFile)
                    {
                        shadowZip.AddFile(sFile, @"\"); //permet d'avoir le fichier sans les dossiers                        
                    }
                    shadowZip.Save(DestinationPath + "\\" + ZipName + ".zip");

                    return true;
                }
            }
            else
                return false;
        }

        //Password Button
        private void AddPasswordButton_Click(object sender, EventArgs e)
        {
            zipPassword = PasswordTextBox.Text;

            if (String.IsNullOrEmpty(zipPassword))
            {
                PasswordTextBox.Enabled = true;
                MessageBox.Show("Please enter a password.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            else
            {
                PasswordTextBox.Enabled = false;
                MessageBox.Show("Password is assigned.", "Password Mode Enabled");
            }
        }

        //Password Mode
        private void IsPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (IsPassword.Checked)
            {
                PasswordTextBox.Enabled = true;
                AddPasswordButton.Enabled = true;

                MessageBox.Show("Enter a password and please activate it by clicking on the padlock.", "Password Mode");
            }
            else
            {
                PasswordTextBox.Enabled = false;
                AddPasswordButton.Enabled = false;
            }
        }

        public void MessageBoxMultiLines(IEnumerable<string> lines)
        {
            var instructionLine = new StringBuilder();
            bool firstLine = false;
            foreach (string line in lines)
            {
                if (firstLine)
                    instructionLine.Append(Environment.NewLine);

                instructionLine.Append(line);
                firstLine = true;
            }
            MessageBox.Show(instructionLine.ToString(), "Information");
        }

        private void radioButtonZip_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonZip.Checked)
            {
                ShadowZipButton.Enabled = true;
                ShadowUnZipButton.Enabled = false;
            }
            else
            {
                ShadowZipButton.Enabled = false;
                ShadowUnZipButton.Enabled = true;
            }
        }

        private void radioButtonUnZip_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonUnZip.Checked)
            {
                ShadowZipButton.Enabled = false;
                ShadowUnZipButton.Enabled = true;
                ZipTextBox.Enabled = false;
                PasswordTextBox.Enabled = false;
                IsPassword.Enabled = false;
            }
            else
            {
                ShadowZipButton.Enabled = true;
                ShadowUnZipButton.Enabled = false;
                ZipTextBox.Enabled = true;
                PasswordTextBox.Enabled = true;
                IsPassword.Enabled = true;
            }
        }

        private void ShadowUnZipButton_Click(object sender, EventArgs e)
        {
            foreach (object item in SelectedFileList.Items)
                listSelectedFile.Add(item.ToString());

            zipDestinationPath = DestinationTextBox.Text;
            zipName = ZipTextBox.Text;
            string pathZipName = BrowserTextBox.Text;

            if (VerifyTextBox(BrowserTextBox.Text, PasswordTextBox.Text, ZipTextBox.Text, DestinationTextBox.Text))
            {
                zipStatus = RunShadowUnZip(listSelectedFile, zipPassword, zipDestinationPath, zipName);

                if (zipStatus == true)
                {
                    MessageBox.Show("ZIP COMPLETED.", "Success");
                    Reset();
                }
                else
                {
                    MessageBox.Show("ZIP FAILED.", "Fail", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                MessageBox.Show("Please fill all the blanks.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                listSelectedFile.Clear();
            }
        }

        //Make the UNZIP file
        public static bool RunShadowUnZip(List<string> listSelectedFile, string zipPassword, string DestinationPath, string ZipName)
        {
            if (listSelectedFile != null)
            {
                    foreach (var sFile in listSelectedFile)
                    {
                        using (ZipFile shadowZip = new ZipFile(sFile))
                        {
                            shadowZip.Password = zipPassword;
                            shadowZip.ExtractAll(DestinationPath);
                        }                       
                    }  
                    return true;                
            }
            else
                return false;
        }
    }
}
