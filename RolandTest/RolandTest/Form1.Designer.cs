namespace RolandTest
{
    partial class Form1
    {
        /// <summary>
        /// Gerekli designer değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // TabControl ve TabPage nesneleri
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageRobot;
        private System.Windows.Forms.TabPage tabPageHarmony;
        private System.Windows.Forms.TabPage tabPageMegaphone;
        private System.Windows.Forms.TabPage tabPageReverb;
        private System.Windows.Forms.TabPage tabPageVocoder;
        private System.Windows.Forms.TabPage tabPageEqualizer;

        // Robot sekmesi nesneleri
        private System.Windows.Forms.ComboBox comboBoxRobotOctave;
        private System.Windows.Forms.CheckBox checkBoxRobotFeedbackSwitch;
        private System.Windows.Forms.TrackBar trackBarRobotFeedbackResonance;
        private System.Windows.Forms.TrackBar trackBarRobotFeedbackLevel;

        // Harmony sekmesi nesneleri
        private System.Windows.Forms.TrackBar trackBarHarmony1Level;
        private System.Windows.Forms.TrackBar trackBarHarmony2Level;
        private System.Windows.Forms.TrackBar trackBarHarmony3Level;
        private System.Windows.Forms.ComboBox comboBoxHarmony1Key;
        private System.Windows.Forms.ComboBox comboBoxHarmony2Key;
        private System.Windows.Forms.ComboBox comboBoxHarmony3Key;
        private System.Windows.Forms.TrackBar trackBarHarmony1Gender;
        private System.Windows.Forms.TrackBar trackBarHarmony2Gender;
        private System.Windows.Forms.TrackBar trackBarHarmony3Gender;
        private System.Windows.Forms.Button btnApplyHarmony;

        // Megaphone sekmesi nesneleri
        private System.Windows.Forms.ComboBox comboBoxMegaphoneType;
        private System.Windows.Forms.TrackBar trackBarMegaphoneParam1;
        private System.Windows.Forms.TrackBar trackBarMegaphoneParam2;
        private System.Windows.Forms.TrackBar trackBarMegaphoneParam3;
        private System.Windows.Forms.TrackBar trackBarMegaphoneParam4;

        // Reverb sekmesi nesneleri
        private System.Windows.Forms.ComboBox comboBoxReverbType;
        private System.Windows.Forms.TrackBar trackBarReverbParam1;
        private System.Windows.Forms.TrackBar trackBarReverbParam2;
        private System.Windows.Forms.TrackBar trackBarReverbParam3;
        private System.Windows.Forms.TrackBar trackBarReverbParam4;

        // Vocoder sekmesi nesneleri
        private System.Windows.Forms.ComboBox comboBoxVocoderType;
        private System.Windows.Forms.TrackBar trackBarVocoderParam1;
        private System.Windows.Forms.TrackBar trackBarVocoderParam2;
        private System.Windows.Forms.TrackBar trackBarVocoderParam3;
        private System.Windows.Forms.TrackBar trackBarVocoderParam4;

        // Equalizer sekmesi nesneleri
        private System.Windows.Forms.CheckBox checkBoxEqualizer;
        private System.Windows.Forms.TrackBar trackBarLowShelfFreq;
        private System.Windows.Forms.TrackBar trackBarLowShelfGain;
        private System.Windows.Forms.TrackBar trackBarLowMidFreq;
        private System.Windows.Forms.TrackBar trackBarLowMidQ;
        private System.Windows.Forms.TrackBar trackBarLowMidGain;
        private System.Windows.Forms.TrackBar trackBarHighMidFreq;
        private System.Windows.Forms.TrackBar trackBarHighMidQ;
        private System.Windows.Forms.TrackBar trackBarHighMidGain;
        private System.Windows.Forms.TrackBar trackBarHighShelfFreq;
        private System.Windows.Forms.TrackBar trackBarHighShelfGain;
        private System.Windows.Forms.Button btnApplyEqualizer;

        /// <summary>
        /// Temizleme işlemlerini gerçekleştirir.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Tasarımcı desteği için gerekli metottur.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl = new TabControl();
            tabPageRobot = new TabPage();
            comboBoxRobotOctave = new ComboBox();
            checkBoxRobotFeedbackSwitch = new CheckBox();
            trackBarRobotFeedbackResonance = new TrackBar();
            trackBarRobotFeedbackLevel = new TrackBar();
            tabPageHarmony = new TabPage();
            trackBarHarmony1Level = new TrackBar();
            trackBarHarmony2Level = new TrackBar();
            trackBarHarmony3Level = new TrackBar();
            comboBoxHarmony1Key = new ComboBox();
            comboBoxHarmony2Key = new ComboBox();
            comboBoxHarmony3Key = new ComboBox();
            trackBarHarmony1Gender = new TrackBar();
            trackBarHarmony2Gender = new TrackBar();
            trackBarHarmony3Gender = new TrackBar();
            btnApplyHarmony = new Button();
            tabPageMegaphone = new TabPage();
            comboBoxMegaphoneType = new ComboBox();
            trackBarMegaphoneParam1 = new TrackBar();
            trackBarMegaphoneParam2 = new TrackBar();
            trackBarMegaphoneParam3 = new TrackBar();
            trackBarMegaphoneParam4 = new TrackBar();
            tabPageReverb = new TabPage();
            comboBoxReverbType = new ComboBox();
            trackBarReverbParam1 = new TrackBar();
            trackBarReverbParam2 = new TrackBar();
            trackBarReverbParam3 = new TrackBar();
            trackBarReverbParam4 = new TrackBar();
            tabPageVocoder = new TabPage();
            comboBoxVocoderType = new ComboBox();
            trackBarVocoderParam1 = new TrackBar();
            trackBarVocoderParam2 = new TrackBar();
            trackBarVocoderParam3 = new TrackBar();
            trackBarVocoderParam4 = new TrackBar();
            tabPageEqualizer = new TabPage();
            checkBoxEqualizer = new CheckBox();
            trackBarLowShelfFreq = new TrackBar();
            trackBarLowShelfGain = new TrackBar();
            trackBarLowMidFreq = new TrackBar();
            trackBarLowMidQ = new TrackBar();
            trackBarLowMidGain = new TrackBar();
            trackBarHighMidFreq = new TrackBar();
            trackBarHighMidQ = new TrackBar();
            trackBarHighMidGain = new TrackBar();
            trackBarHighShelfFreq = new TrackBar();
            trackBarHighShelfGain = new TrackBar();
            btnApplyEqualizer = new Button();
            tabControl.SuspendLayout();
            tabPageRobot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarRobotFeedbackResonance).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarRobotFeedbackLevel).BeginInit();
            tabPageHarmony.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarHarmony1Level).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHarmony2Level).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHarmony3Level).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHarmony1Gender).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHarmony2Gender).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHarmony3Gender).BeginInit();
            tabPageMegaphone.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarMegaphoneParam1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMegaphoneParam2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMegaphoneParam3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMegaphoneParam4).BeginInit();
            tabPageReverb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarReverbParam1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarReverbParam2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarReverbParam3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarReverbParam4).BeginInit();
            tabPageVocoder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarVocoderParam1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVocoderParam2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVocoderParam3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVocoderParam4).BeginInit();
            tabPageEqualizer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarLowShelfFreq).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLowShelfGain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLowMidFreq).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLowMidQ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLowMidGain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHighMidFreq).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHighMidQ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHighMidGain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHighShelfFreq).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHighShelfGain).BeginInit();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabPageRobot);
            tabControl.Controls.Add(tabPageHarmony);
            tabControl.Controls.Add(tabPageMegaphone);
            tabControl.Controls.Add(tabPageReverb);
            tabControl.Controls.Add(tabPageVocoder);
            tabControl.Controls.Add(tabPageEqualizer);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(800, 600);
            tabControl.TabIndex = 0;
            // 
            // tabPageRobot
            // 
            tabPageRobot.Controls.Add(comboBoxRobotOctave);
            tabPageRobot.Controls.Add(checkBoxRobotFeedbackSwitch);
            tabPageRobot.Controls.Add(trackBarRobotFeedbackResonance);
            tabPageRobot.Controls.Add(trackBarRobotFeedbackLevel);
            tabPageRobot.Location = new Point(4, 24);
            tabPageRobot.Name = "tabPageRobot";
            tabPageRobot.Size = new Size(792, 572);
            tabPageRobot.TabIndex = 0;
            tabPageRobot.Text = "Robot";
            // 
            // comboBoxRobotOctave
            // 
            comboBoxRobotOctave.Items.AddRange(new object[] { "2DOWN", "DOWN", "ZERO", "UP" });
            comboBoxRobotOctave.Location = new Point(20, 20);
            comboBoxRobotOctave.Name = "comboBoxRobotOctave";
            comboBoxRobotOctave.Size = new Size(120, 23);
            comboBoxRobotOctave.TabIndex = 0;
            comboBoxRobotOctave.SelectedIndexChanged += RobotParametersChanged;
            // 
            // checkBoxRobotFeedbackSwitch
            // 
            checkBoxRobotFeedbackSwitch.AutoSize = true;
            checkBoxRobotFeedbackSwitch.Location = new Point(20, 60);
            checkBoxRobotFeedbackSwitch.Name = "checkBoxRobotFeedbackSwitch";
            checkBoxRobotFeedbackSwitch.Size = new Size(76, 19);
            checkBoxRobotFeedbackSwitch.TabIndex = 1;
            checkBoxRobotFeedbackSwitch.Text = "Feedback";
            checkBoxRobotFeedbackSwitch.CheckedChanged += RobotParametersChanged;
            // 
            // trackBarRobotFeedbackResonance
            // 
            trackBarRobotFeedbackResonance.Location = new Point(20, 100);
            trackBarRobotFeedbackResonance.Maximum = 255;
            trackBarRobotFeedbackResonance.Name = "trackBarRobotFeedbackResonance";
            trackBarRobotFeedbackResonance.Size = new Size(200, 45);
            trackBarRobotFeedbackResonance.TabIndex = 2;
            trackBarRobotFeedbackResonance.ValueChanged += RobotParametersChanged;
            // 
            // trackBarRobotFeedbackLevel
            // 
            trackBarRobotFeedbackLevel.Location = new Point(20, 150);
            trackBarRobotFeedbackLevel.Maximum = 255;
            trackBarRobotFeedbackLevel.Name = "trackBarRobotFeedbackLevel";
            trackBarRobotFeedbackLevel.Size = new Size(200, 45);
            trackBarRobotFeedbackLevel.TabIndex = 3;
            trackBarRobotFeedbackLevel.ValueChanged += RobotParametersChanged;
            // 
            // tabPageHarmony
            // 
            tabPageHarmony.Controls.Add(trackBarHarmony1Level);
            tabPageHarmony.Controls.Add(trackBarHarmony2Level);
            tabPageHarmony.Controls.Add(trackBarHarmony3Level);
            tabPageHarmony.Controls.Add(comboBoxHarmony1Key);
            tabPageHarmony.Controls.Add(comboBoxHarmony2Key);
            tabPageHarmony.Controls.Add(comboBoxHarmony3Key);
            tabPageHarmony.Controls.Add(trackBarHarmony1Gender);
            tabPageHarmony.Controls.Add(trackBarHarmony2Gender);
            tabPageHarmony.Controls.Add(trackBarHarmony3Gender);
            tabPageHarmony.Controls.Add(btnApplyHarmony);
            tabPageHarmony.Location = new Point(4, 24);
            tabPageHarmony.Name = "tabPageHarmony";
            tabPageHarmony.Size = new Size(192, 72);
            tabPageHarmony.TabIndex = 1;
            tabPageHarmony.Text = "Harmony";
            // 
            // trackBarHarmony1Level
            // 
            trackBarHarmony1Level.Location = new Point(20, 20);
            trackBarHarmony1Level.Maximum = 255;
            trackBarHarmony1Level.Name = "trackBarHarmony1Level";
            trackBarHarmony1Level.Size = new Size(200, 45);
            trackBarHarmony1Level.TabIndex = 0;
            // 
            // trackBarHarmony2Level
            // 
            trackBarHarmony2Level.Location = new Point(20, 70);
            trackBarHarmony2Level.Maximum = 255;
            trackBarHarmony2Level.Name = "trackBarHarmony2Level";
            trackBarHarmony2Level.Size = new Size(200, 45);
            trackBarHarmony2Level.TabIndex = 1;
            // 
            // trackBarHarmony3Level
            // 
            trackBarHarmony3Level.Location = new Point(20, 120);
            trackBarHarmony3Level.Maximum = 255;
            trackBarHarmony3Level.Name = "trackBarHarmony3Level";
            trackBarHarmony3Level.Size = new Size(200, 45);
            trackBarHarmony3Level.TabIndex = 2;
            // 
            // comboBoxHarmony1Key
            // 
            comboBoxHarmony1Key.Items.AddRange(new object[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" });
            comboBoxHarmony1Key.Location = new Point(240, 20);
            comboBoxHarmony1Key.Name = "comboBoxHarmony1Key";
            comboBoxHarmony1Key.Size = new Size(100, 23);
            comboBoxHarmony1Key.TabIndex = 3;
            comboBoxHarmony1Key.SelectedIndexChanged += btnApplyHarmony_Click;
            // 
            // comboBoxHarmony2Key
            // 
            comboBoxHarmony2Key.Items.AddRange(new object[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" });
            comboBoxHarmony2Key.Location = new Point(240, 70);
            comboBoxHarmony2Key.Name = "comboBoxHarmony2Key";
            comboBoxHarmony2Key.Size = new Size(100, 23);
            comboBoxHarmony2Key.TabIndex = 4;
            comboBoxHarmony2Key.SelectedIndexChanged += btnApplyHarmony_Click;
            // 
            // comboBoxHarmony3Key
            // 
            comboBoxHarmony3Key.Items.AddRange(new object[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" });
            comboBoxHarmony3Key.Location = new Point(240, 120);
            comboBoxHarmony3Key.Name = "comboBoxHarmony3Key";
            comboBoxHarmony3Key.Size = new Size(100, 23);
            comboBoxHarmony3Key.TabIndex = 5;
            comboBoxHarmony3Key.SelectedIndexChanged += btnApplyHarmony_Click;
            // 
            // trackBarHarmony1Gender
            // 
            trackBarHarmony1Gender.Location = new Point(360, 20);
            trackBarHarmony1Gender.Maximum = 255;
            trackBarHarmony1Gender.Name = "trackBarHarmony1Gender";
            trackBarHarmony1Gender.Size = new Size(150, 45);
            trackBarHarmony1Gender.TabIndex = 6;
            trackBarHarmony1Gender.ValueChanged += btnApplyHarmony_Click;
            // 
            // trackBarHarmony2Gender
            // 
            trackBarHarmony2Gender.Location = new Point(360, 70);
            trackBarHarmony2Gender.Maximum = 255;
            trackBarHarmony2Gender.Name = "trackBarHarmony2Gender";
            trackBarHarmony2Gender.Size = new Size(150, 45);
            trackBarHarmony2Gender.TabIndex = 7;
            trackBarHarmony2Gender.ValueChanged += btnApplyHarmony_Click;
            // 
            // trackBarHarmony3Gender
            // 
            trackBarHarmony3Gender.Location = new Point(360, 120);
            trackBarHarmony3Gender.Maximum = 255;
            trackBarHarmony3Gender.Name = "trackBarHarmony3Gender";
            trackBarHarmony3Gender.Size = new Size(150, 45);
            trackBarHarmony3Gender.TabIndex = 8;
            trackBarHarmony3Gender.ValueChanged += btnApplyHarmony_Click;
            // 
            // btnApplyHarmony
            // 
            btnApplyHarmony.Location = new Point(20, 180);
            btnApplyHarmony.Name = "btnApplyHarmony";
            btnApplyHarmony.Size = new Size(120, 30);
            btnApplyHarmony.TabIndex = 9;
            btnApplyHarmony.Text = "Apply Harmony";
            btnApplyHarmony.Click += btnApplyHarmony_Click;
            // 
            // tabPageMegaphone
            // 
            tabPageMegaphone.Controls.Add(comboBoxMegaphoneType);
            tabPageMegaphone.Controls.Add(trackBarMegaphoneParam1);
            tabPageMegaphone.Controls.Add(trackBarMegaphoneParam2);
            tabPageMegaphone.Controls.Add(trackBarMegaphoneParam3);
            tabPageMegaphone.Controls.Add(trackBarMegaphoneParam4);
            tabPageMegaphone.Location = new Point(4, 24);
            tabPageMegaphone.Name = "tabPageMegaphone";
            tabPageMegaphone.Size = new Size(192, 72);
            tabPageMegaphone.TabIndex = 2;
            tabPageMegaphone.Text = "Megaphone";
            // 
            // comboBoxMegaphoneType
            // 
            comboBoxMegaphoneType.Items.AddRange(new object[] { "Megaphone", "Radio", "BBD Chorus", "Strobo" });
            comboBoxMegaphoneType.Location = new Point(20, 20);
            comboBoxMegaphoneType.Name = "comboBoxMegaphoneType";
            comboBoxMegaphoneType.Size = new Size(120, 23);
            comboBoxMegaphoneType.TabIndex = 0;
            comboBoxMegaphoneType.SelectedIndexChanged += MegaphoneParametersChanged;
            // 
            // trackBarMegaphoneParam1
            // 
            trackBarMegaphoneParam1.Location = new Point(20, 60);
            trackBarMegaphoneParam1.Maximum = 255;
            trackBarMegaphoneParam1.Name = "trackBarMegaphoneParam1";
            trackBarMegaphoneParam1.Size = new Size(200, 45);
            trackBarMegaphoneParam1.TabIndex = 1;
            trackBarMegaphoneParam1.ValueChanged += MegaphoneParametersChanged;
            // 
            // trackBarMegaphoneParam2
            // 
            trackBarMegaphoneParam2.Location = new Point(20, 110);
            trackBarMegaphoneParam2.Maximum = 255;
            trackBarMegaphoneParam2.Name = "trackBarMegaphoneParam2";
            trackBarMegaphoneParam2.Size = new Size(200, 45);
            trackBarMegaphoneParam2.TabIndex = 2;
            trackBarMegaphoneParam2.ValueChanged += MegaphoneParametersChanged;
            // 
            // trackBarMegaphoneParam3
            // 
            trackBarMegaphoneParam3.Location = new Point(20, 160);
            trackBarMegaphoneParam3.Maximum = 255;
            trackBarMegaphoneParam3.Name = "trackBarMegaphoneParam3";
            trackBarMegaphoneParam3.Size = new Size(200, 45);
            trackBarMegaphoneParam3.TabIndex = 3;
            trackBarMegaphoneParam3.ValueChanged += MegaphoneParametersChanged;
            // 
            // trackBarMegaphoneParam4
            // 
            trackBarMegaphoneParam4.Location = new Point(20, 210);
            trackBarMegaphoneParam4.Maximum = 255;
            trackBarMegaphoneParam4.Name = "trackBarMegaphoneParam4";
            trackBarMegaphoneParam4.Size = new Size(200, 45);
            trackBarMegaphoneParam4.TabIndex = 4;
            trackBarMegaphoneParam4.ValueChanged += MegaphoneParametersChanged;
            // 
            // tabPageReverb
            // 
            tabPageReverb.Controls.Add(comboBoxReverbType);
            tabPageReverb.Controls.Add(trackBarReverbParam1);
            tabPageReverb.Controls.Add(trackBarReverbParam2);
            tabPageReverb.Controls.Add(trackBarReverbParam3);
            tabPageReverb.Controls.Add(trackBarReverbParam4);
            tabPageReverb.Location = new Point(4, 24);
            tabPageReverb.Name = "tabPageReverb";
            tabPageReverb.Size = new Size(192, 72);
            tabPageReverb.TabIndex = 3;
            tabPageReverb.Text = "Reverb";
            // 
            // comboBoxReverbType
            // 
            comboBoxReverbType.Items.AddRange(new object[] { "Reverb", "Echo", "Delay", "Dub Echo", "Deep Reverb", "VT Reverb" });
            comboBoxReverbType.Location = new Point(20, 20);
            comboBoxReverbType.Name = "comboBoxReverbType";
            comboBoxReverbType.Size = new Size(120, 23);
            comboBoxReverbType.TabIndex = 0;
            comboBoxReverbType.SelectedIndexChanged += ReverbParametersChanged;
            // 
            // trackBarReverbParam1
            // 
            trackBarReverbParam1.Location = new Point(20, 60);
            trackBarReverbParam1.Maximum = 255;
            trackBarReverbParam1.Name = "trackBarReverbParam1";
            trackBarReverbParam1.Size = new Size(200, 45);
            trackBarReverbParam1.TabIndex = 1;
            trackBarReverbParam1.ValueChanged += ReverbParametersChanged;
            // 
            // trackBarReverbParam2
            // 
            trackBarReverbParam2.Location = new Point(20, 110);
            trackBarReverbParam2.Maximum = 255;
            trackBarReverbParam2.Name = "trackBarReverbParam2";
            trackBarReverbParam2.Size = new Size(200, 45);
            trackBarReverbParam2.TabIndex = 2;
            trackBarReverbParam2.ValueChanged += ReverbParametersChanged;
            // 
            // trackBarReverbParam3
            // 
            trackBarReverbParam3.Location = new Point(20, 160);
            trackBarReverbParam3.Maximum = 255;
            trackBarReverbParam3.Name = "trackBarReverbParam3";
            trackBarReverbParam3.Size = new Size(200, 45);
            trackBarReverbParam3.TabIndex = 3;
            trackBarReverbParam3.ValueChanged += ReverbParametersChanged;
            // 
            // trackBarReverbParam4
            // 
            trackBarReverbParam4.Location = new Point(20, 210);
            trackBarReverbParam4.Maximum = 255;
            trackBarReverbParam4.Name = "trackBarReverbParam4";
            trackBarReverbParam4.Size = new Size(200, 45);
            trackBarReverbParam4.TabIndex = 4;
            trackBarReverbParam4.ValueChanged += ReverbParametersChanged;
            // 
            // tabPageVocoder
            // 
            tabPageVocoder.Controls.Add(comboBoxVocoderType);
            tabPageVocoder.Controls.Add(trackBarVocoderParam1);
            tabPageVocoder.Controls.Add(trackBarVocoderParam2);
            tabPageVocoder.Controls.Add(trackBarVocoderParam3);
            tabPageVocoder.Controls.Add(trackBarVocoderParam4);
            tabPageVocoder.Location = new Point(4, 24);
            tabPageVocoder.Name = "tabPageVocoder";
            tabPageVocoder.Size = new Size(192, 72);
            tabPageVocoder.TabIndex = 4;
            tabPageVocoder.Text = "Vocoder";
            // 
            // comboBoxVocoderType
            // 
            comboBoxVocoderType.Items.AddRange(new object[] { "Vintage", "Advanced", "Talk Box", "Spell Toy" });
            comboBoxVocoderType.Location = new Point(20, 20);
            comboBoxVocoderType.Name = "comboBoxVocoderType";
            comboBoxVocoderType.Size = new Size(120, 23);
            comboBoxVocoderType.TabIndex = 0;
            comboBoxVocoderType.SelectedIndexChanged += VocoderParametersChanged;
            // 
            // trackBarVocoderParam1
            // 
            trackBarVocoderParam1.Location = new Point(20, 60);
            trackBarVocoderParam1.Maximum = 255;
            trackBarVocoderParam1.Name = "trackBarVocoderParam1";
            trackBarVocoderParam1.Size = new Size(200, 45);
            trackBarVocoderParam1.TabIndex = 1;
            trackBarVocoderParam1.ValueChanged += VocoderParametersChanged;
            // 
            // trackBarVocoderParam2
            // 
            trackBarVocoderParam2.Location = new Point(20, 110);
            trackBarVocoderParam2.Maximum = 255;
            trackBarVocoderParam2.Name = "trackBarVocoderParam2";
            trackBarVocoderParam2.Size = new Size(200, 45);
            trackBarVocoderParam2.TabIndex = 2;
            trackBarVocoderParam2.ValueChanged += VocoderParametersChanged;
            // 
            // trackBarVocoderParam3
            // 
            trackBarVocoderParam3.Location = new Point(20, 160);
            trackBarVocoderParam3.Maximum = 255;
            trackBarVocoderParam3.Name = "trackBarVocoderParam3";
            trackBarVocoderParam3.Size = new Size(200, 45);
            trackBarVocoderParam3.TabIndex = 3;
            trackBarVocoderParam3.ValueChanged += VocoderParametersChanged;
            // 
            // trackBarVocoderParam4
            // 
            trackBarVocoderParam4.Location = new Point(20, 210);
            trackBarVocoderParam4.Maximum = 255;
            trackBarVocoderParam4.Name = "trackBarVocoderParam4";
            trackBarVocoderParam4.Size = new Size(200, 45);
            trackBarVocoderParam4.TabIndex = 4;
            trackBarVocoderParam4.ValueChanged += VocoderParametersChanged;
            // 
            // tabPageEqualizer
            // 
            tabPageEqualizer.Controls.Add(checkBoxEqualizer);
            tabPageEqualizer.Controls.Add(trackBarLowShelfFreq);
            tabPageEqualizer.Controls.Add(trackBarLowShelfGain);
            tabPageEqualizer.Controls.Add(trackBarLowMidFreq);
            tabPageEqualizer.Controls.Add(trackBarLowMidQ);
            tabPageEqualizer.Controls.Add(trackBarLowMidGain);
            tabPageEqualizer.Controls.Add(trackBarHighMidFreq);
            tabPageEqualizer.Controls.Add(trackBarHighMidQ);
            tabPageEqualizer.Controls.Add(trackBarHighMidGain);
            tabPageEqualizer.Controls.Add(trackBarHighShelfFreq);
            tabPageEqualizer.Controls.Add(trackBarHighShelfGain);
            tabPageEqualizer.Controls.Add(btnApplyEqualizer);
            tabPageEqualizer.Location = new Point(4, 24);
            tabPageEqualizer.Name = "tabPageEqualizer";
            tabPageEqualizer.Size = new Size(192, 72);
            tabPageEqualizer.TabIndex = 5;
            tabPageEqualizer.Text = "Equalizer";
            // 
            // checkBoxEqualizer
            // 
            checkBoxEqualizer.AutoSize = true;
            checkBoxEqualizer.Location = new Point(20, 20);
            checkBoxEqualizer.Name = "checkBoxEqualizer";
            checkBoxEqualizer.Size = new Size(111, 19);
            checkBoxEqualizer.TabIndex = 0;
            checkBoxEqualizer.Text = "Enable Equalizer";
            // 
            // trackBarLowShelfFreq
            // 
            trackBarLowShelfFreq.Location = new Point(20, 50);
            trackBarLowShelfFreq.Maximum = 127;
            trackBarLowShelfFreq.Name = "trackBarLowShelfFreq";
            trackBarLowShelfFreq.Size = new Size(150, 45);
            trackBarLowShelfFreq.TabIndex = 1;
            // 
            // trackBarLowShelfGain
            // 
            trackBarLowShelfGain.Location = new Point(20, 100);
            trackBarLowShelfGain.Maximum = 40;
            trackBarLowShelfGain.Name = "trackBarLowShelfGain";
            trackBarLowShelfGain.Size = new Size(150, 45);
            trackBarLowShelfGain.TabIndex = 2;
            // 
            // trackBarLowMidFreq
            // 
            trackBarLowMidFreq.Location = new Point(20, 150);
            trackBarLowMidFreq.Maximum = 127;
            trackBarLowMidFreq.Name = "trackBarLowMidFreq";
            trackBarLowMidFreq.Size = new Size(150, 45);
            trackBarLowMidFreq.TabIndex = 3;
            // 
            // trackBarLowMidQ
            // 
            trackBarLowMidQ.Location = new Point(20, 200);
            trackBarLowMidQ.Maximum = 127;
            trackBarLowMidQ.Name = "trackBarLowMidQ";
            trackBarLowMidQ.Size = new Size(150, 45);
            trackBarLowMidQ.TabIndex = 4;
            // 
            // trackBarLowMidGain
            // 
            trackBarLowMidGain.Location = new Point(20, 250);
            trackBarLowMidGain.Maximum = 40;
            trackBarLowMidGain.Name = "trackBarLowMidGain";
            trackBarLowMidGain.Size = new Size(150, 45);
            trackBarLowMidGain.TabIndex = 5;
            // 
            // trackBarHighMidFreq
            // 
            trackBarHighMidFreq.Location = new Point(200, 50);
            trackBarHighMidFreq.Maximum = 127;
            trackBarHighMidFreq.Name = "trackBarHighMidFreq";
            trackBarHighMidFreq.Size = new Size(150, 45);
            trackBarHighMidFreq.TabIndex = 6;
            // 
            // trackBarHighMidQ
            // 
            trackBarHighMidQ.Location = new Point(200, 100);
            trackBarHighMidQ.Maximum = 127;
            trackBarHighMidQ.Name = "trackBarHighMidQ";
            trackBarHighMidQ.Size = new Size(150, 45);
            trackBarHighMidQ.TabIndex = 7;
            // 
            // trackBarHighMidGain
            // 
            trackBarHighMidGain.Location = new Point(200, 150);
            trackBarHighMidGain.Maximum = 40;
            trackBarHighMidGain.Name = "trackBarHighMidGain";
            trackBarHighMidGain.Size = new Size(150, 45);
            trackBarHighMidGain.TabIndex = 8;
            // 
            // trackBarHighShelfFreq
            // 
            trackBarHighShelfFreq.Location = new Point(200, 200);
            trackBarHighShelfFreq.Maximum = 127;
            trackBarHighShelfFreq.Name = "trackBarHighShelfFreq";
            trackBarHighShelfFreq.Size = new Size(150, 45);
            trackBarHighShelfFreq.TabIndex = 9;
            // 
            // trackBarHighShelfGain
            // 
            trackBarHighShelfGain.Location = new Point(200, 250);
            trackBarHighShelfGain.Maximum = 40;
            trackBarHighShelfGain.Name = "trackBarHighShelfGain";
            trackBarHighShelfGain.Size = new Size(150, 45);
            trackBarHighShelfGain.TabIndex = 10;
            // 
            // btnApplyEqualizer
            // 
            btnApplyEqualizer.Location = new Point(20, 310);
            btnApplyEqualizer.Name = "btnApplyEqualizer";
            btnApplyEqualizer.Size = new Size(120, 30);
            btnApplyEqualizer.TabIndex = 11;
            btnApplyEqualizer.Text = "Apply Equalizer";
            btnApplyEqualizer.Click += btnApplyEqualizer_Click;
            // 
            // Form1
            // 
            ClientSize = new Size(800, 600);
            Controls.Add(tabControl);
            Name = "Form1";
            Text = "VT-4 MIDI WinForms Controller";
            Load += Form1_Load;
            tabControl.ResumeLayout(false);
            tabPageRobot.ResumeLayout(false);
            tabPageRobot.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarRobotFeedbackResonance).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarRobotFeedbackLevel).EndInit();
            tabPageHarmony.ResumeLayout(false);
            tabPageHarmony.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarHarmony1Level).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHarmony2Level).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHarmony3Level).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHarmony1Gender).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHarmony2Gender).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHarmony3Gender).EndInit();
            tabPageMegaphone.ResumeLayout(false);
            tabPageMegaphone.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarMegaphoneParam1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMegaphoneParam2).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMegaphoneParam3).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMegaphoneParam4).EndInit();
            tabPageReverb.ResumeLayout(false);
            tabPageReverb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarReverbParam1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarReverbParam2).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarReverbParam3).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarReverbParam4).EndInit();
            tabPageVocoder.ResumeLayout(false);
            tabPageVocoder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarVocoderParam1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVocoderParam2).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVocoderParam3).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarVocoderParam4).EndInit();
            tabPageEqualizer.ResumeLayout(false);
            tabPageEqualizer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarLowShelfFreq).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLowShelfGain).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLowMidFreq).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLowMidQ).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarLowMidGain).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHighMidFreq).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHighMidQ).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHighMidGain).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHighShelfFreq).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarHighShelfGain).EndInit();
            ResumeLayout(false);
        }
    }
}
