using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FileSync
{
    public partial class frmFileSync : Form
    {
        string sourcePath, destinationPath;

        public frmFileSync()
        {
            InitializeComponent();
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            // call CopyDirectory() when a change in the specified directory happens
            try
            {
                CopyDirectory(sourcePath, destinationPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStartSync_Click(object sender, EventArgs e)
        {
            try
            {
                if(sourceLabel.Text == "" || destinationLabel.Text == "")
                {
                    MessageBox.Show("Please enter a source path and destination path.", "Choose Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    syncStartedLabel.Text = "Sync Started";

                    // Set the source path for the file system watcher control
                    fileSystemWatcher1.Path = sourcePath;
                    fileSystemWatcher1.EnableRaisingEvents = true;
                    timer1.Start();

                    // call CopyDirectory to automatically copy the directory at the given point
                    CopyDirectory(sourcePath, destinationPath);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnStopSync_Click(object sender, EventArgs e)
        {
            fileSystemWatcher1.EnableRaisingEvents = false; // don't allow the directory to be watched

            // stop the stopwatch
            timer1.Stop();
            elapsedTimeLabel.Text = "Sync last refreshed at: " + DateTime.Now.ToLongTimeString();
            syncStartedLabel.Text = "Sync Stopped";
        }

        private void btnMoveDirectory_Click(object sender, EventArgs e)
        {
            try
            {
                // Move contents from source folder to dest folder
                MoveDirectory(sourcePath, destinationPath);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDeleteDirectory_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteDirectory();
            }
            catch
            {
                MessageBox.Show("Path must not be empty");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            elapsedTimeLabel.Text = "Sync refreshed at: " + DateTime.Now.ToLongTimeString();
        }

        private void sourceFolderPictureBox_Click(object sender, EventArgs e)
        {
            ChooseSourceFolder();
        }

        private void destinationFolderPictureBox_Click(object sender, EventArgs e)
        {
            ChooseDestinationFolder();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            syncStartedLabel.Text = "Sync Stopped";
        }

        private void CopyDirectory(string source, string target)
        {
            // variables
            string sourcePath = source.TrimEnd('\\', ' ');  // trims the source path
            string destinationPath = target.TrimEnd('\\', ' ');  // trims the destination path
            var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories).GroupBy(s => Path.GetDirectoryName(s));
            destinationListBox.Items.Clear();
            sourceListBox.Items.Clear();

            // iterate through each folder
            foreach (var folder in files)
            {
                string targetFolder = folder.Key.Replace(sourcePath, destinationPath);
                Directory.CreateDirectory(targetFolder);

                // iterate through each file 
                foreach (var file in folder)
                {
                    string targetFile = Path.Combine(targetFolder, Path.GetFileName(file));

                    // check if the target file exists
                    if (File.Exists(targetFile))
                    {
                        // delete the target file 
                        File.Delete(targetFile);
                    }
                    // copy the file
                    File.Copy(file, targetFile);

                    destinationListBox.Items.Add(targetFile);
                    sourceListBox.Items.Add(file);    
                }
            }
        }

        public void DeleteDirectory()
        {   
            try
            {
                // use the DirectoryInfo instance method.
                System.IO.DirectoryInfo sourceDi = new System.IO.DirectoryInfo(sourcePath);

                // delete the directory and all subdirs
                sourceDi.Delete(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void MoveDirectory(string source, string target)
        {
            string sourcePath = source.TrimEnd('\\', ' ');  // trims the source path
            string destinationPath = target.TrimEnd('\\', ' ');  // trims the destination path
            var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories).GroupBy(s => Path.GetDirectoryName(s));

            // iterate through each folder
            foreach (var folder in files)
            {
                string targetFolder = folder.Key.Replace(sourcePath, destinationPath);
                Directory.CreateDirectory(targetFolder);

                // iterate through each file 
                foreach (var file in folder)
                {
                    string targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                    
                    // check if the target file exists
                    if (File.Exists(targetFile))
                    {
                        // delete the target file 
                        File.Delete(targetFile);
                    }
                    // move the file
                    File.Move(file, targetFile);
                    destinationListBox.Items.Add(targetFile);
                    sourceListBox.Items.Add(file);
                }
            }
            // Delete the initial source directory
            Directory.Delete(source, true);
        }

        public void ChooseSourceFolder()
        {
            // if the user made a selection
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                // set the selected path to the source path
                sourcePath = folderBrowserDialog1.SelectedPath;
                sourceLabel.Text = sourcePath;
            }
        }
        public void ChooseDestinationFolder()
        {
            // if the user made a selection
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                // set the selected path to the destination paths
                destinationPath = folderBrowserDialog2.SelectedPath;
                destinationLabel.Text = destinationPath;
            }
        }     
    }
}
