using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
namespace Alura
{
    public class Speaker
    {
        private static SpeechSynthesizer sp = new SpeechSynthesizer();
        public static void Speak(string text)
        {
            //caso esteja falando 
            if (sp.State == SynthesizerState.Speaking)
                sp.SpeakAsyncCancelAll();
            sp.SpeakAsync(text);
        }

        public static void Speak(params string[] texts)
        {
            Random rnd = new Random();
            Speak(texts[rnd.Next(0, texts.Length)]);
        }
        //Alterar voz
        public static void SetVoice(string voice)
        {
            try
            {
                sp.SelectVoice(voice);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Erro em Speaker:" + ex.Message);
            }
        }
      
    }
}
