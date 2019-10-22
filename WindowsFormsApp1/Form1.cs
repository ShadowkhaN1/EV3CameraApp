using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using Lego.Ev3.Desktop;
using Lego.Ev3.Core;
using System.Threading;
using AForge.Video;

namespace WindowsFormsApp1
{
    public partial class VoiceControllerEV3 : Form
    {

        SpeechRecognitionEngine recEngine = new SpeechRecognitionEngine();
        SpeechSynthesizer sarah = new SpeechSynthesizer();
        Brick brick;
        int forward = 40;
        int backward = 30;
        uint time = 300;
        int speed = 50;
        MJPEGStream stream;

        string speek = "";

        public VoiceControllerEV3()
        {
            InitializeComponent();
            stream = new MJPEGStream("http://192.168.1.156:4747/video");

            stream.NewFrame += Stream_NewFrame;

        }

        private void Stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bmp = (Bitmap)eventArgs.Frame.Clone();
            pictureBox2.Image = bmp;

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            recEngine.RecognizeAsync(RecognizeMode.Multiple);
        }



        private void Brick_BrickChanged(object sender, BrickChangedEventArgs e)
        {
            //MessageBox.Show(Convert.ToString(e.Ports[InputPort.A].SIValue));
        }

        private async void RecEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {

            sarah.SpeakCompleted += Sarah_SpeakCompleted;


            switch (e.Result.Text)
            {           
                
                case "go john":
                   // richTextBox1.Text += " " + "GO";
                    speek = "go";
                    //MessageBox.Show("Hello Lukas");
                    await brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
                    sarah.SpeakAsync("Ok, I'm moving forward");
                    break;
                case "back john":
                    speek = "back";
                    //richTextBox1.Text += " " + "BACK";
                    //MessageBox.Show("Hello Lukas");
                    await brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
                    sarah.SpeakAsync("Ok, i'm goin back");
                    break;
                case "stop john":
                    speek = "stop";
                    await brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
                    sarah.SpeakAsync("Ok, I'm not going further.");
                  //  richTextBox1.Text += " " + "STOP";
                    break;
                case "left john":
                    speek = "left";
                    await brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
                    sarah.SpeakAsync("I'm turning left");
                   // richTextBox1.Text += " " + "left";
                    break;
                case "white john":
                    speek = "white";
                    await brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
                    sarah.SpeakAsync("I'm turning right");
                   // richTextBox1.Text += " " + "right";
                    break;
                case "attack john":
                    speek = "attack";
                    await brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
                    sarah.SpeakAsync("I'm attacking you!");
                   // richTextBox1.Text += " " + "attack";
                    break;
                case "faster":
                   // richTextBox1.Text += " " + "faster";
                    speed = speed +10;
                    break;
                case "what time is it":
                   // richTextBox1.Text += " " + "time";
                    sarah.SpeakAsync(DateTime.Now.ToString("h mm tt"));
                    break;
                case "say hello john":
                   // richTextBox1.Text += " " + "hello";
                    sarah.SpeakAsync("Hello everyone!!");
                    break;

            }

    
    }

        private async void Sarah_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            switch (speek)
            {

                case "go":
                    await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B | OutputPort.C, speed);
                    //MessageBox.Show("Hello Lukas");
                    break;

                case "back":
                    //MessageBox.Show("Hello Lukas");
                    await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B | OutputPort.C, -speed);
                    break;
                case "stop":
                    await brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
                    break;
                case "left":
                    await brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
                    await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B, speed);
                    break;
                case "white":
                    await brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
                    await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.C, speed);
                    break;
                case "attack":
                    await brick.DirectCommand.StepMotorAtPowerAsync(OutputPort.A, 50, 360, true);
                    break;
                case "faster":
                    speed = speed + 10;
                    await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B | OutputPort.C, speed);
                    break;

            }
        }

        

     

        private void Button2_Click(object sender, EventArgs e)
        {
            recEngine.RecognizeAsyncStop();
        }

        private async void Button3_Click(object sender, EventArgs e)
        {
            await brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.A, forward, time, false);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {


            //brick = new Brick(new BluetoothCommunication("COM4"));
            brick = new Brick(new NetworkCommunication("192.168.1.219"));
            brick.BrickChanged += Brick_BrickChanged;
            await brick.ConnectAsync();
            await brick.DirectCommand.PlayToneAsync(10, 1000, 300);

            recEngine = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));


            Choices commands = new Choices();
            commands.Add(new string[] { "go john", "stop john", "left john", "white john", "attack john", "back john", "faster john", "what time is it", "say hello john" });
            GrammarBuilder gBuilder = new GrammarBuilder();
            gBuilder.Append(commands);
            Grammar grammar = new Grammar(gBuilder);


            recEngine.LoadGrammarAsync(grammar);
            recEngine.SetInputToDefaultAudioDevice();


            recEngine.SpeechRecognized += RecEngine_SpeechRecognized;

        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            speed = Convert.ToInt32(trackBar1.Value);
            label2.Text = "Speed " + trackBar1.Value.ToString();
        }

        private async void Button8_Click(object sender, EventArgs e)
        {
            //await brick.SystemCommand.CopyFileAsync("EV3.rsf", "/myapp/myapp/EV3.rsf");
            await brick.DirectCommand.PlaySoundAsync(1000, "../prjs/myapp/EV3.rsf");
        }

        private async void Button3_Click_1(object sender, EventArgs e)
        {
            await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B | OutputPort.C, speed);
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B | OutputPort.C, -speed);
        }

        private async void Button7_Click(object sender, EventArgs e)
        {
           await brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
        }

        private async void Button4_Click(object sender, EventArgs e)
        {
            await brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
            await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B, -speed);
            await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.C, speed);
        }

        private async void Button9_Click(object sender, EventArgs e)
        {
            await brick.DirectCommand.StepMotorAtPowerAsync(OutputPort.A, -50, 360 * 4, true);

           
        }

        public async void WaitSomeTime(int time)
        {
            await Task.Delay(time);

        }

        private async void Button5_Click(object sender, EventArgs e)
        {
            await brick.DirectCommand.StopMotorAsync(OutputPort.B | OutputPort.C, true);
            await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.B, speed);
            await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.C, -speed);
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            stream.Start();
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {

        }

        private async void Button13_Click(object sender, EventArgs e)
        {
            await brick.DirectCommand.StepMotorAtPowerAsync(OutputPort.A, 50, 360 * 4, true);
        }

        private async void Button12_Click(object sender, EventArgs e)
        {
           
            await brick.DirectCommand.TurnMotorAtPowerForTimeAsync(OutputPort.D, -40, 3000, true);
        }

        private async void Button11_Click(object sender, EventArgs e)
        {
            await brick.DirectCommand.StepMotorAtPowerAsync(OutputPort.D, 20, 120, true);
        }
    }
    
}
