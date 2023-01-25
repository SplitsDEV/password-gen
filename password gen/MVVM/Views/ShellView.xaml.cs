using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using password_gen.MVVM.ViewModels;

namespace password_gen.MVVM.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        // making a dictionary for language strings
        ResourceDictionary dictionary = new ResourceDictionary();
        string? generatedPass = null;

        public ShellView()
        {
            Language_Change("fr");
            InitializeComponent();
            randChars.IsChecked = true;
        }

        private void cmbLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /// getting the langCode   
            string clgC1 = e.AddedItems[0].ToString().ToLower();
            string currentLangCode = clgC1.Substring(clgC1.Length - 2);
            Trace.WriteLine(currentLangCode);
            /// passing it into Language Change function
            Language_Change(currentLangCode);
        }

        public void Language_Change(string langCode)
        {
            switch (langCode)
            {
                case "en":
                    dictionary.Source = new Uri("..\\StringResources.en.xaml", UriKind.Relative);
                    Trace.WriteLine(dictionary.Source);
                    break;
                case "fr":
                    dictionary.Source = new Uri("..\\StringResources.fr.xaml", UriKind.Relative);
                    Trace.WriteLine(dictionary.Source);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dictionary);
            // update generated pass ofc
            if (generatedPass != null)
            {
                errorTextUpdate();
            }
        }

        private void includeWords_Checked(object sender, RoutedEventArgs e)
        {
            txtWdIn.Visibility = Visibility.Visible;
            txtInfoWdIn.Visibility = Visibility.Visible;
            boxWdIn.Visibility = Visibility.Visible;
            checkScram.IsEnabled = true;
        }

        private void randChars_Checked(object sender, RoutedEventArgs e)
        {
            if (txtWdIn != null)
            {
                txtWdIn.Visibility = Visibility.Collapsed;
            }
            if (txtInfoWdIn != null)
            {
                txtInfoWdIn.Visibility = Visibility.Collapsed;
            }
            if (boxWdIn != null)
            {
                boxWdIn.Visibility = Visibility.Collapsed;
            }
            checkScram.IsEnabled = false;
            checkScram.IsChecked = false;
        }

        public void generate_Pressed(object sender, RoutedEventArgs e)
        {
            // make pVars from the options to pass as parameters in generate function
            bool pInclude = false;
            bool pCaps = false;
            bool pNums = false;
            bool pSpecials = false;
            bool pScramble = false;
            int pMinLen = 7;
            int pMaxLen = 12;
            string[] pWordsIn = { };

            if (includeWords.IsChecked != null)
            {
                pInclude = (bool)includeWords.IsChecked;
            }
            if (checkCaps.IsChecked != null)
            {
                pCaps = (bool)checkCaps.IsChecked;
            }
            if (checkNums.IsChecked != null)
            {
                pNums = (bool)checkNums.IsChecked;
            }
            if (checkSpecials.IsChecked != null)
            {
                pSpecials = (bool)checkSpecials.IsChecked;
            }
            if (checkScram.IsChecked != null)
            {
                pScramble = (bool)checkScram.IsChecked;
            }
            if (updownMinLength.Value != null)
            {
                pMinLen = (int)updownMinLength.Value;
            }
            if (updownMaxLength.Value != null)
            {
                pMaxLen = (int)updownMaxLength.Value;
            }
            if (boxWdIn.Text != null)
            {
                string strWdIn = boxWdIn.Text;
                pWordsIn = strWdIn.Split(",", StringSplitOptions.RemoveEmptyEntries);
            }
            // debugging to check if it works right (it does)
            Trace.WriteLine(pInclude + " " + pCaps + " " + pNums + " " + pSpecials + " " + pScramble + " " + pMinLen + " " + pMaxLen + " " + String.Join(" ", pWordsIn));

            // generating the password then putting it in txt in the app
            try
            {
                // run the function
                generatedPass = passGenScript.GeneratePassword(pInclude, pCaps, pNums, pSpecials, pScramble, pMinLen, pMaxLen, pWordsIn);
                Trace.WriteLine(generatedPass);
                // possible errors
                errorTextUpdate();
                textCopied.Visibility = Visibility.Hidden;
            }
            catch
            {
            }
            
        }

        private void copyBtn_Pressed(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(boxPass.Text);
            textCopied.Visibility = Visibility.Visible;
        }

        private void errorTextUpdate()
        {
            string errorText = "";
            if (generatedPass == "MinLenMaxLen")
            {
                textError.Visibility = Visibility.Visible;
                errorText = (string)dictionary["StrTextErrorMinMax"];
                textError.Text = errorText;
            }
            else if (generatedPass == "MinLenLowOptions")
            {
                textError.Visibility = Visibility.Visible;
                errorText = (string)dictionary["StrTextErrorMinLow"];
                textError.Text = errorText;
            }
            else if (generatedPass == "NoWords")
            {
                textError.Visibility = Visibility.Visible;
                errorText = (string)dictionary["StrTextErrorNoWd"];
                textError.Text = errorText;
            }
            else if (generatedPass == "Words5")
            {
                textError.Visibility = Visibility.Visible;
                errorText = (string)dictionary["StrTextErrorTooManyWd"];
                textError.Text = errorText;
            }
            else if (generatedPass == "Len32")
            {
                textError.Visibility = Visibility.Visible;
                errorText = (string)dictionary["StrTextErrorLenTooHigh"];
                textError.Text = errorText;
            }
            else if (generatedPass == "Len7")
            {
                textError.Visibility = Visibility.Visible;
                errorText = (string)dictionary["StrTextErrorLenTooLow"];
                textError.Text = errorText;
            }
            else if (generatedPass == "SpaceWord")
            {
                textError.Visibility = Visibility.Visible;
                errorText = (string)dictionary["StrTextErrorSpace"];
                textError.Text = errorText;
            }
            else
            {
                textError.Visibility = Visibility.Hidden;
                boxPass.Text = generatedPass;
            }
        }
    }
}
