using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace CorpTranslateTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CheckTranslateService();

            fr_content.Source = new Uri("Translate.xaml", UriKind.Relative);
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }


        }

        private void lbl_close_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void lbl_minimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        public void CheckTranslateService()
        {

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("SecretURL");
                webRequest.AllowAutoRedirect = false;
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    MessageBox.Show("You are not connected to Corporate network\n" +
                  "or Translation service is out of order.\n" +
                  "Please connect your computer to VPN\n" +
                  "or try again later.", "Corporate Translate Tool", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }

            }
            catch (System.Net.WebException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
