using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using MSharp.Framework.UI;
using Microsoft.VisualBasic;

namespace WSLManagerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DistroManager distroManager;


        public Label createLabel(string val, SolidColorBrush color)
        {
            Label l = new Label();
            l.Content = val;
            l.Foreground = color;
            l.FontSize = 20;
            return l;
        }


        public StackPanel generatePanel(Distro distro)
        {
            var distroName = distro.distroName;
            Label label = new Label();
            label.FontSize = 32;
            label.Content = distroName;

            Image img = new Image();

            img.Source = new BitmapImage(new Uri(@"/Images/ubuntu.png", UriKind.Relative));
            img.Height = 100;
            img.Width = 100;
            StackPanel panel = new StackPanel();
            panel.Children.Add(img);
            panel.Children.Add(label);

            return panel;
        }


        public DockPanel generateDockPanel(Distro distro)
        {

            DockPanel dockPanel = new DockPanel();
            dockPanel.Children.Add(generatePanel(distro));
            StackPanel panel = new StackPanel();
            
            dockPanel.Children.Add(panel);
            if(distro.succeed) panel.Children.Add(createLabel("VALID", new SolidColorBrush(Colors.Green)));
            else panel.Children.Add(createLabel("ERROR", new SolidColorBrush(Colors.Red)));
            panel.Children.Add(createLabel("WSL"+distro.wslVersion, new SolidColorBrush(Colors.Black)));

            panel.Children.Add(createLabel(distro.distroStatus, new SolidColorBrush(Colors.Black)));
            if (distro.isDefaultDistro) panel.Children.Add(createLabel("Default", new SolidColorBrush(Colors.Blue)));
            return dockPanel;

        }


        private void refresh()
        {
            distroManager = JsonConvert.DeserializeObject<DistroManager>(WslQuery.getInstalledDistrosJson());

            this.DistroElementHolder.Items.Clear();
            for (int i = 0; i < distroManager.distros.Count; ++i)
            {
                this.DistroElementHolder.Items.Add(generateDockPanel(distroManager.distros[i]));
            }
        }
        private void MainWindows_refresh(object sender, RoutedEventArgs e)
        {
            distroManager = JsonConvert.DeserializeObject<DistroManager>(WslQuery.getInstalledDistrosJson());

            this.DistroElementHolder.Items.Clear();

            for (int i = 0; i < distroManager.distros.Count; ++i)
            {
                this.DistroElementHolder.Items.Add(generateDockPanel(distroManager.distros[i]));
            }

        }


     
        public MainWindow()
        {
            distroManager = JsonConvert.DeserializeObject<DistroManager>(WslQuery.getInstalledDistrosJson());
            Console.WriteLine(distroManager.distros[0].distroName);
            InitializeComponent();
            this.DistroElementHolder.Items.Clear();
            for(int i = 0; i<distroManager.distros.Count;++i)
            {
                this.DistroElementHolder.Items.Add(generateDockPanel(distroManager.distros[i]));
            }

        }


        private Distro getSelectedDistro()
        {
            DockPanel dp = (DockPanel)this.DistroElementHolder.SelectedItem;

            if (dp == null) return null;
            StackPanel sp = (StackPanel)dp.Children[0];
            Label lb = (Label)sp.Children[1];
            string distroName = lb.Content.ToString();

            for (int i = 0; i < distroManager.distros.Count; ++i)
            {
                if (distroManager.distros[i].distroName == distroName)
                    return distroManager.distros[i];
            }
            return null;
        }

        private void MenuItem_Click_Set_Default(object sender, RoutedEventArgs e)
        {
            var distro = getSelectedDistro();
            if (distro == null) return;

            ProcessOps.setDefaultDistro(distro.distroName);
            refresh();

        }

        private void MenuItem_Click_Change_Wsl_Version(object sender, RoutedEventArgs e)
        {
            var distro = getSelectedDistro();
            if (distro == null) return;

            ProcessOps.changeDistroVersion(distro.distroName, ((distro.wslVersion) % 2) + 1);
            refresh();

        }
        private void MenuItem_Click_Terminate(object sender, RoutedEventArgs e)
        {
            var distro = getSelectedDistro();
            if (distro == null) return;

            ProcessOps.terminateDistro(distro.distroName);
            refresh();

        }
        private void MenuItem_Click_Remove(object sender, RoutedEventArgs e)
        {
            var distro = getSelectedDistro();
            if (distro == null) return;

            ProcessOps.unregisterDistro(distro.distroName);
            refresh();

        }

        private void MenuItem_Click_Export(object sender, RoutedEventArgs e)
        {
            var distro = getSelectedDistro();
            if (distro == null) return;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "rootfs|*.tar.gz";
            saveFileDialog1.Title = "Exporting distro: "+distro.distroName;
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
                ProcessOps.exportDistro(distro.distroName, saveFileDialog1.FileName);

            refresh();
        }

    
   
        private void MainWindows_import(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.OverwritePrompt = false;
            saveFileDialog1.Filter = "rootfs|*.tar.gz";
            saveFileDialog1.Title = "Importing distro";
            saveFileDialog1.ShowDialog();

            string tartoinstall = saveFileDialog1.FileName;
            if (tartoinstall == "") return;

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (dialog.FileName == "") return;
            string directorytoinstall = dialog.FileName;

            bool res  = MessageBox.Show("We encourage to install WSL2", "Click NO if you want WSL1", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question,
                MessageBoxResult.Yes) == MessageBoxResult.Yes;
            int version = 2;
            if(!res)
            {
                version = 1;
            }

            string input = Interaction.InputBox("Write the name you want to have for your distro", 
                "Distro Name", "Default", -1, -1);


            if (input == "") return;
            ProcessOps.installDistro(input, tartoinstall, directorytoinstall, version);
            refresh();

        }

        private void DistroElementHolder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

                var distro = getSelectedDistro();
                if (distro != null)
                    ProcessOps.runDistro(distro.distroName);
            refresh();

        }
    }
}
