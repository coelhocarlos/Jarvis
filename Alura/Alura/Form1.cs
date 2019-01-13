using System;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Text;


namespace Alura
{
    public partial class Form1 : Form
    {
        private SelectVoice selectVoice = null;
        private SpeechRecognitionEngine engine; //engine de Reconhecimento
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer(); // Sisntetisador
        private Browser browser;
        private MediaPlayer mediaplayer;
       
        
        public bool is_Alura_Listening = true;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSpeech();
            Speak("Arquivos Carregados e Funcionais");
           
        }
        private void LoadSpeech()
        {
            try
            {
                engine = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("es-ES"));//instancia
                //engine = new SpeechRecognitionEngine();
                engine.SetInputToDefaultAudioDevice();// microfone

                //Choices Numbers 
                Choices cNumbers = new Choices();
                for (int i = 0; i <= 100; i++)
                    cNumbers.Add(i.ToString());

                //string[] words = { "ola", "hi", "hello",  "bom dia" };
                Choices comandos = new Choices();
                comandos.Add(GrammarRules.Alura_Horas.ToArray());// Pedir horas
                comandos.Add(GrammarRules.Alura_Datas.ToArray());// Pedir Data
                comandos.Add(GrammarRules.Alura_Start_Listening.ToArray());// Alura Start Listening
                comandos.Add(GrammarRules.Alura_Stop_Listening.ToArray());// Alura Stop Listening
                comandos.Add(GrammarRules.Alura_Minimize.ToArray());// Alura Minimize Window
                comandos.Add(GrammarRules.Alura_Normal_window.ToArray());// Alura Normal Window
                comandos.Add(GrammarRules.Alura_Change_voice.ToArray());// Alura Change Voice
                comandos.Add(GrammarRules.Alura_Open_Program.ToArray());// Alura Change Voice
                comandos.Add(GrammarRules.Alura_MediaPlayer_Commands .ToArray());// Alura Change Voice



                //Comando pare de Ouvir Alura
                GrammarBuilder gb_comandos = new GrammarBuilder();
                gb_comandos.Append(comandos);
                Grammar g_comandos = new Grammar(gb_comandos);
                g_comandos.Name = "sys";
                engine.LoadGrammar(g_comandos);

                //Comando para somar valores
                GrammarBuilder gbNumber = new GrammarBuilder();
                gbNumber.Append(cNumbers);// 5 vezes
                gbNumber.Append(new Choices("vezes", "mais", "menos", "por"));
                gbNumber.Append(cNumbers);//5 vezes
                Grammar gNumbers = new Grammar(gbNumber);
                gNumbers.Name = "calc";
                engine.LoadGrammar(gNumbers);

                //Carregar Gramatica
                //engine.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(words))));
                engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(rec);
                engine.AudioLevelUpdated += new EventHandler<AudioLevelUpdatedEventArgs>(audioLevel);
                engine.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(rej);
                engine.RecognizeAsync(RecognizeMode.Multiple);// inicia o reconhecimento



                synthesizer.SpeakStarted += new EventHandler<SpeakStartedEventArgs>(Speak_Started);
                synthesizer.SpeakProgress += new EventHandler<SpeakProgressEventArgs>(Speak_Progress);
            
               
                Speak("Arquivos Carregados");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro no LoadSpeech()" + ex.Message);
            }
        }

        #region Speak Methods
        private void Speak(string text)
        {
            synthesizer.SpeakAsync(text);
        }
        private void Speak(params string[] texts)
        {
            Random r = new Random();
            Speak(texts[r.Next(0, texts.Length)]);
        }
        #endregion
        private void Speak_Started(object sender, SpeakStartedEventArgs e)
        {
            label2.Text = "Alura: ";
        }
        private void Speak_Progress(object sender, SpeakProgressEventArgs e)
        {
            label2.Text += e.Text + " ";
        }

        private void rej(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            label1.BackColor = Color.Red;
        }
        private void MinimizeWindow()
        {
            if (WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Minimized;
                Speak("Minimizando a janela", "Tudo bem", "Como quiser", "Vou fazer isso");
            }
            else
            {
                Speak("já esta minimizada", "a janela já esta minimizada", "jã fiz isso");
            }
        }
        private void NormalWindow()
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                Speak("Deixando a janela em tamahno normal", "Como quiser", "Vou fazer isso", "Tudo bem", "Irei fazer isso");
            }
            else
            {
               Speak("A janela já está tamahno normal", "já fiz isso", "Já foi Feito", "Desculpe isso já foi feito");
            }
        }
        private void audioLevel(object sender, AudioLevelUpdatedEventArgs e)
        {
            progressBar1.Maximum = 100;
            progressBar1.Value = e.AudioLevel;
          
        }


        //metodo que e chamado quando a algo e reconhecido
        private void rec(object sender, SpeechRecognizedEventArgs e)
        {
            //MessageBox.Show(e.Result.Text);
            richTextBox1.Text = e.Result.Text;
            string speech = e.Result.Text; // string reconhecida
            float conf = e.Result.Confidence;

            //Log Alura Comands
            string date = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();
            string log_Alura = "log\\" + date + ".txt";
            StreamWriter sw = File.AppendText(log_Alura);
            if (File.Exists(log_Alura))
                sw.WriteLine(speech);
            else
            {
                sw.WriteLine(speech);
            }
            sw.Close();

            if (conf > 0.35f)
            {
                label1.BackColor = Color.Olive;
                if (GrammarRules.Alura_Stop_Listening.Any(x => x == speech))
                {
                    is_Alura_Listening = false;
                    label1.BackColor = Color.Black;
                    label1.Text = "Reconhecido" + speech;
                }
                else if (GrammarRules.Alura_Start_Listening.Any(x => x == speech))
                {
                    is_Alura_Listening = true; 
                    label1.BackColor = Color.Olive;
                    Speak("Olá, tudo bem","Oi como vai ?", "Estou aqui","Sim Mestre!","Aguardando Ordens","o Que deseja?", "Em que posso ajudar?");
                }
               
                    if (is_Alura_Listening == true)
                {
                    switch (e.Result.Grammar.Name)
                    {
                        case "sys":
                            if (GrammarRules.Alura_Horas.Any(x => x == speech))
                            {
                                Runner.Horas();
                            }
                            else if (GrammarRules.Alura_Datas.Any(x => x == speech))
                            {
                                Runner.Datas();
                            }
                            else if (GrammarRules.Alura_Minimize.Any(x => x == speech))
                            {
                                MinimizeWindow();
                            }
                            else if (GrammarRules.Alura_Normal_window.Any(x => x == speech))
                            {
                                NormalWindow();
                            }
                            else if (GrammarRules.Alura_Change_voice.Any(x => x == speech))
                            {
                                if (selectVoice == null || selectVoice.IsDisposed == true)
                                    selectVoice = new SelectVoice();
                                selectVoice.Show();
                            }
                            else if (GrammarRules.Alura_Open_Program.Any(x => x == speech))
                            {
                                switch (speech)
                                {
                                    case "Navegador":
                                        browser = new Browser();
                                        browser.Show();
                                        break;
                                    case "Media Player":
                                        mediaplayer = new MediaPlayer();
                                        mediaplayer.Show();
                                        break;
                                }
                            }
                            else if (GrammarRules.Alura_MediaPlayer_Commands.Any(x => x == speech))
                                switch (speech)
                                {
                                    case "Abrir arquivo":
                                        if(mediaplayer != null)
                                        {
                                            mediaplayer.OpenFile();
                                            Speak("Selecione um arquivo");
                                        }
                                        else
                                        {
                                            Speak("Media player nõo está aberto");
                                        }
                                    break;
                                }
                                break;
                           

                        case "calc":
                            Speak(CalcSolver.Solve(speech));
                            break;
                    }
                }
                  
            }
        }

    }
}