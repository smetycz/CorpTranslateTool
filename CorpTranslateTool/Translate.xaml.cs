using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace CorpTranslateTool
{
    /// <summary>
    /// Interaction logic for Translate.xaml
    /// </summary>
    public partial class Translate : Page
    {
        public Translate()
        {
            InitializeComponent();

            LoadLanguages();

            txt_SrcText.Focus();

            lbl_Console.Content = "";
        }

        // this method populates combobox with some languages

        public void LoadLanguages()
        {
            string[] langugages = { "en", "de", "cs", "pl", "ro", "it", "fr", "es" };

            foreach (string language in langugages)
            {
                cb_langauges.Items.Add(language);

            }

            if (String.IsNullOrEmpty(Properties.Settings.Default.DefLanguage) != true)
            {
                cb_langauges.SelectedValue = Properties.Settings.Default.DefLanguage;

            }

        }

        //method if a language is chosen and txtbox with source text is not empty

        public bool AllInputs()
        {
            bool SelectedLanguage = cb_langauges.SelectedIndex < 0;
            bool InputText = txt_SrcText.Text.Length < 1;

            if (SelectedLanguage is true | InputText is true)
            {
                return false;
            }

            else
                return true;

        }

        // this tasks detects source language
        public async Task DetectLanguage()
        {

            try
            {
                if (AllInputs() == false)
                {
                    return;
                }

                var DetectApiUrl = "SECRETURL";
                using (var client = new HttpClient())
                {
                    var requestBody = new
                    {
                        contentOrigin = "SECRET",
                        srcLanguage = "xx",
                        tgtLanguage = cb_langauges.SelectedValue.ToString(),
                        sourcetext = txt_SrcText.Text.ToString(),
                        textType = "plain"

                    };

                    var request = new HttpRequestMessage(HttpMethod.Post, DetectApiUrl)
                    {
                        Content = new JsonContent(requestBody),

                    };

                    var responseDetectedLanguage = await client.SendAsync(request);
                    var responseContent = await responseDetectedLanguage.Content.ReadAsStringAsync();
                    var responseString = responseContent.ToString().Replace("\"", "");
                    lbl_detect.Content = responseString;
                }
            }
            catch (Exception ex)
            {
                txt_SrcText.Text = ex.Message;
                throw;
            }

        }

        public async Task TranslateTask()
        {


            try
            {
                if (AllInputs() == false)
                {
                    return;
                }

                var apiURL = "SECRETURL";
                using (var client = new HttpClient())
                {

                    var requestBody = new
                    {
                        contentOrigin = "SECRET",
                        srcLanguage = lbl_detect.Content,
                        tgtLanguage = cb_langauges.SelectedValue.ToString(),
                        sourcetext = txt_SrcText.Text.ToString(),
                        textType = "plain"

                    };

                    var request = new HttpRequestMessage(HttpMethod.Post, apiURL)
                    {
                        Content = new JsonContent(requestBody),


                    };

                    var response = await client.SendAsync(request);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseString = responseContent.ToString().Replace("[", "").Replace("]", "");




                    Translation translation = JsonConvert.DeserializeObject<Translation>(responseString);
                    txt_TransText.Text = translation.text;


                }
            }
            catch (Exception ex)
            {
                txt_TransText.Text = ex.Message;

            }
        }


        // if you select language, it will be saved into settings
        private void cb_langauges_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Properties.Settings.Default.DefLanguage = cb_langauges.SelectedItem.ToString();
                Properties.Settings.Default.Save();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);

            }

        }

        // copy translated text
        private void btn_copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(txt_TransText.Text);
                ShowMessage("The translated text has been copied to the clipboard");

            }
            catch (Exception ex)
            {
                txt_TransText.Text = ex.Message;

            }
        }


        // this method shows some information to the user for 4 seconds
        public void ShowMessage(string input)
        {
            try
            {

                int seconds = 4;

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(seconds);
                lbl_Console.Visibility = Visibility.Visible;
                lbl_Console.Content = input;
                timer.Tick += (s, en) => {
                    lbl_Console.Visibility = Visibility.Collapsed;
                    timer.Stop(); 
                };
                timer.Start(); 


            }
            catch (Exception ex)
            {
                txt_TransText.Text = ex.Message;

            }
        }

        // CTRL + T and CTRL + R shortcuts
        private async void Grid_KeyDown(object sender, KeyEventArgs e)
        {

            if (Keyboard.IsKeyDown(Key.T) && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (AllInputs() == false)
                {
                    ShowMessage("No input");
                    return;
                }

                await DetectLanguage();
                txt_TransText.Text = "Translating...";


                await TranslateTask();
                ShowMessage("Translated");

            }

            if (Keyboard.IsKeyDown(Key.R) && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (AllInputs() == false)
                {
                    ShowMessage("No input");
                    return;
                }
                await DetectLanguage();
                txt_TransText.Text = "Translating...";
                await TranslateTask();

                Clipboard.SetText(txt_TransText.Text);
                ShowMessage("The translated text has been copied to the clipboard");

            }

        }


        // double click - copy translated text into clipboard
        private void transText_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (txt_TransText.Text.Length < 1)
                {
                    ShowMessage("Nothing to copy");
                    return;
                }


                Clipboard.SetText(txt_TransText.Text);
                ShowMessage("The translated text has been copied to the clipboard");
            }
            catch (Exception ex)
            {
                txt_TransText.Text = ex.Message;

            }
        }

        // click on img -> translating
        private async void img_translate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AllInputs() == false)
            {
                ShowMessage("No input");
                return;
            }

            await DetectLanguage();
            txt_TransText.Text = "Translating...";

            await TranslateTask();
        }


        //click on img --> copy text into clipboard
        private void img_copy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (txt_TransText.Text.Length < 1)
            {
                ShowMessage("Nothing to copy");
            }
            else
            {
                Clipboard.SetText(txt_TransText.Text);
                ShowMessage("The translated text has been copied to the clipboard");
            }
        }
    }





  

    public class Translation
    {
        public string DetectedLanguage { get; set; }
        public string TgtLanguage { get; set; }

        public string Text { get; set; }

    }

   
    public class JsonContent : StringContent
    {
        public JsonContent(object obj) :
            base(Newtonsoft.Json.JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8, "application/json")
        { }
    }

}
