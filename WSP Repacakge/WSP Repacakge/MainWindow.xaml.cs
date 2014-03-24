using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.Xml.Linq;

namespace WSP_Repacakge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Title += " v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void Log(string message)
        {
            txtProgressLog.Text += message + "\n";
            txtProgressLog.ScrollToEnd();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtProgressLog.Text = "starting...\n";
                CabLib.Extract extractor = new CabLib.Extract();
                CabLib.Compress compressor = new CabLib.Compress();

                CabLib.Extract.kHeaderInfo cInfo = null;
                if (extractor.IsFileCabinet(txtFilePath.Text, out cInfo))
                {
                    Log("file is a valid cabinet");
                    //extract WSP to temp folder
                    Guid jobGuid = Guid.NewGuid();
                    string jobid = jobGuid.ToString("N");

                    Log("current job id is " + jobid);

                    var fi = new FileInfo(txtFilePath.Text);

                    var jobFolder = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\" + jobid);
                    Log("job temp directory created");
                    extractor.ExtractFile(txtFilePath.Text, jobFolder.FullName);
                    Log("cabinet exatracted to job temp directory");

                    //Load manifest, change solution ID, remove assemblies, etc
                    XDocument xDoc = XDocument.Load(jobFolder.FullName + "\\manifest.xml");
                    Log("manifest loaded");
                    if (chkSetNewSolutionId.IsChecked.Value)
                    {
                        xDoc.Root.Attribute("SolutionId").Value = jobGuid.ToString("D");
                        Log("solution id set to " + jobGuid.ToString("D"));
                    }
                    if (chkRemoveAllDlls.IsChecked.Value)
                    {
                        var xAssemblies = xDoc.Root.Descendants(xDoc.Root.GetDefaultNamespace() + "Assemblies");
                        xAssemblies.Remove();
                        Log("assemblies element removed from manifest");
                        jobFolder.EnumerateFiles("*.dll").ToList().ForEach(dll => { File.SetAttributes(dll.FullName, FileAttributes.Normal); File.Delete(dll.FullName); });
                        Log("all dll files deleted");
                    }
                    xDoc.Save(jobFolder.FullName + "\\manifest.xml");
                    Log("updated manifest file saved");

                    //repackage as a new WSP or overwrite source file
                    string wspFileName = txtNewFileName.Text.Replace("{filename}", fi.Name.Remove(fi.Name.Length - 4)) + ".wsp";
                    Log("new wsp file name is " + wspFileName);
                    //Build Dictionary<string,string> for all cab files
                    DDFBuilder.Build(jobFolder.FullName, wspFileName);
                    Log("ddf file built");

                    compressor.SwitchCompression(CabLib.Compress.eCompress.MSZIP);
                    compressor.CompressFolder(jobFolder.FullName, fi.Directory.FullName + "\\" + wspFileName, null, true, true, int.MaxValue);
                    Log("cabinet file craeted");


                    jobFolder.EnumerateFiles("*.*", SearchOption.AllDirectories).ToList().ForEach(dll => { File.SetAttributes(dll.FullName, FileAttributes.Normal); });
                    System.IO.Directory.Delete(jobFolder.FullName, true);
                    Log("temp job folder deleted");
                    Log("i'm done. what's next?");
                }
            }
            catch (Exception ex)
            {
                Log("oops! something went wrong, " + ex.ToString());
            }
        }

        private void chkOverrideSourceFile_Checked(object sender, RoutedEventArgs e)
        {
            if (txtNewFileName == null || chkOverrideSourceFile == null) return;
            if (chkOverrideSourceFile.IsChecked.HasValue && chkOverrideSourceFile.IsChecked.Value)
                txtNewFileName.Text = "{filename}";
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            btnGo.IsEnabled = false;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var firstFile = files[0];
                if (GotAFile(firstFile))
                {
                    return;
                }
            }

            Log("sorry, i don't know what to do with that. please drop a wsp file to begin");
        }

        private void btnFilePicker_Click(object sender, RoutedEventArgs e)
        {
            btnGo.IsEnabled = false;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Filter = "Solution packages (*.wsp)|*.wsp";// "Solution packages (*.wsp)|*.wsp|All files (*.*)|*.*";
            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                GotAFile(dialog.FileName);
            }
        }

        private bool GotAFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                var fi = new FileInfo(fileName);
                if (fi.Extension == ".wsp")
                {
                    txtFilePath.Text = fileName;
                    btnGo.IsEnabled = true;
                    Log("got wsp file, ready to go");
                    return true;
                }
            }
            return false;
        }
    }
}
