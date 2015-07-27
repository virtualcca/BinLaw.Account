using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinLaw.Account.VoiceCommand
{
    public class VoiceCommandcs
    {
        private async void InstallVoiceCommands()
        {
            //const string wp80vcdPath = "ms-appx:///VoiceCommandDefinition_8.0.xml";
            //const string wp81vcdPath = "ms-appx:///VoiceCommandDefinition_8.1.xml";
            //const string chineseWp80vcdPath = "ms-appx:///ChineseVoiceCommandDefinition_8.0.xml";
            //const string chineseWp81vcdPath = "ms-appx:///ChineseVoiceCommandDefinition_8.1.xml";

            //try
            //{
            //    bool using81orAbove = ((Environment.OSVersion.Version.Major >= 8)
            //        && (Environment.OSVersion.Version.Minor >= 10));

            //    string vcdPath = using81orAbove ? wp81vcdPath : wp80vcdPath;
            //    if (InstalledSpeechRecognizers.Default.Language.Equals("zh-CN", StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        vcdPath = using81orAbove ? chineseWp81vcdPath : chineseWp80vcdPath;
            //    }

            //    Uri vcdUri = new Uri(vcdPath);
            //    await VoiceCommandService.InstallCommandSetsFromFileAsync(vcdUri);
            //}
            //catch (Exception vcdEx)
            //{
            //    Dispatcher.BeginInvoke(() =>
            //    {
            //        MessageBox.Show(String.Format(
            //            AppResources.VoiceCommandInstallErrorTemplate, vcdEx.HResult, vcdEx.Message));
            //    });
            //}
        }
    }
}
