using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using LedCube.FileCode;
using LedCube.Model;
using LedCube.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LedCube
{
    public partial class LEDForm : Form
    {
        private LEDCubeModel _LEDCubeModel = new LEDCubeModel();
        private FileCoder FileCoder = new FileCoder();
        private BluetoothClient client = new BluetoothClient();
        private BluetoothDeviceInfo[] devices;
        private Guid service = BluetoothService.SerialPort;
        SerialPort serial = new SerialPort();


        private string offFilePath = @"c:\users\artiom\source\repos\LedCube\LedCube\OffDiod.png";
        private string onFilePath = @"c:\users\artiom\source\repos\LedCube\LedCube\OnDiod.png";
        private string content = string.Empty;

        private Image onImage;
        private Image offImage;

        private int PreviousStep;

        private List<LEDCubeTemplateActionsModel> templates;
        public LEDForm()
        {
            InitializeComponent();
            InitializeLevelsAndDevices();
            //serial.PortName = "COM6";
            //string[] ports = SerialPort.GetPortNames();
            //var incomingbox = new List<string>();
            //for (int i = 0; i < ports.Length; i++)
            //    incomingbox.Add(ports[i]);
            //serial.BaudRate = 9600;
        }

        public void InitializeLevelsAndDevices()
        {
            onImage = Image.FromFile(onFilePath);
            offImage = Image.FromFile(offFilePath);
            comboBoxLevel.Items.AddRange(_LEDCubeModel.Levels);
            comboBoxSteps.Items.Add(1);
            comboBoxLevel.SelectedIndex = 0;
            comboBoxSteps.SelectedIndex = 0;
            PreviousStep = 0;
           
            devices = client.DiscoverDevicesInRange();
            if (devices.Count() > 0)
            {
                comboBoxSelectDevice.DataSource = devices;
                comboBoxSelectDevice.DisplayMember = "DeviceName";
                comboBoxSelectDevice.ValueMember = "DeviceAddress";
                comboBoxSelectDevice.Focus();
                comboBoxSelectDevice.SelectedIndex = 0;
            }

            templates = FileCoder.EncodeFileToLEDCubeTemplateModel();
            if (templates != null)
            {
                comboBoxTemplatesSelect.Items.AddRange(templates.Select(x => x.Name).ToArray());
            }
        }

        private void LEDForm_Load(object sender, EventArgs e)
        {
            
        }

        private void comboBoxTemplatesSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            _LEDCubeModel.CurrentLEDCubeTemplateActionsModel = templates[comboBoxTemplatesSelect.SelectedIndex];
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            var content = "-";

            if (!checkBoxIsTemplates.Checked)
            {
                for (int step = 0; step < numericUpDownSteps.Value; step++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            for (int k = 0; k < 8; k++)
                            {
                                content += _LEDCubeModel.StepsDiods[step][i, j, k] ? "1" : "0";
                            }
                        }
                    }
                    content += $"-{_LEDCubeModel.StepsTiming[step]}-";
                }
            }
            else
            {
                for (int step = 0; step < _LEDCubeModel.CurrentLEDCubeTemplateActionsModel.StepsDiods.Count; step++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            for (int k = 0; k < 8; k++)
                            {
                                content += _LEDCubeModel.StepsDiods[step][i, j, k] ? "1" : "0";
                            }
                        }
                    }
                    content += $"-{_LEDCubeModel.CurrentLEDCubeTemplateActionsModel.StepsTimingDiods[step]}-";
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            for (int k = 0; k < 8; k++)
                            {
                                if (_LEDCubeModel.CurrentLEDCubeTemplateActionsModel.StepsStringsDiods[step].First()[i, j, k]!=null)
                                {
                                    var firstNum = int.Parse(_LEDCubeModel.CurrentLEDCubeTemplateActionsModel.StepsStringsDiods[step].First()[i, j, k]);
                                    int secondNum = -1;
                                    for (int i2 = 0; i2 < 8; i2++)
                                    {
                                        for (int j2 = 0; j2 < 8; j2++)
                                        {
                                            for (int k2 = 0; k2 < 8; k2++)
                                            {
                                                var secondTarget = _LEDCubeModel.CurrentLEDCubeTemplateActionsModel.StepsStringsDiods[step].Last()[i2, j2, k2];
                                                if(int.Parse(secondTarget!=null?secondTarget:"-1")==firstNum)
                                                {
                                                    secondNum = i2 * 100 + j2 * 10 + k2;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (secondNum != -1)
                                    {
                                        _LEDCubeModel.StepsDiods[step][i, j, k] = _LEDCubeModel.StepsDiods[step][secondNum / 100, (secondNum / 10) % 10, secondNum % 10];
                                    }
                                    content += _LEDCubeModel.StepsDiods[step][i, j, k] ? "1" : "0";
                                }
                                else
                                {
                                    content += _LEDCubeModel.StepsDiods[step][i, j, k] ? "1" : "0";
                                }
                            }
                        }
                    }
                    content += $"-{_LEDCubeModel.CurrentLEDCubeTemplateActionsModel.StepsTimingDiods[step+10]}-";
                }
            }
            try
            {
                //if (!serial.IsOpen)
                //{
                //    serial.Open();
                //}
                //serial.Write(content);
                if (!client.Connected)
                    client.Connect(new BluetoothEndPoint((BluetoothAddress)comboBoxSelectDevice.SelectedValue, service));
                var bluetoothStream = client.GetStream();
                if (client.Connected && bluetoothStream != null)
                {
                    var buffer = Encoding.UTF8.GetBytes(content+"#");
                    var bufferParts = buffer.Length / 64 + (buffer.Length % 64 > 0 ? 1 : 0);
                    for (int i = 0; i < bufferParts; i++)
                    {
                        if(i != bufferParts-1)
                            bluetoothStream.BeginWrite(buffer, i*64, 64, null, null);
                        else
                            bluetoothStream.BeginWrite(buffer, i * 64, buffer.Length % 64, null, null);
                        System.Threading.Thread.Sleep(500);
                    }
                }
                content = string.Empty;
            }
            catch(Exception exc)
            {
                MessageBox.Show("Failed Connect");
            }
            _LEDCubeModel.IsTemplate = checkBoxIsTemplates.Checked;
            _LEDCubeModel.Name = textBoxName.Text;
        }

        private void comboBoxLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLevels();
        }

        private void UpdateLevels()
        {
            _LEDCubeModel.CurrentLevel = int.Parse(comboBoxLevel.SelectedItem.ToString()) - 1;
            int i = 7, j = 7;
            foreach (Button control in tableLayoutPanelLedLevel.Controls)
            {
                control.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, j, i] ? onImage : offImage;
                i--;
                if (i < 0)
                {
                    i = 7;
                    j--;
                }
            }
        }

        private void numericUpDownSteps_ValueChanged(object sender, EventArgs e)
        {
            comboBoxSteps.SelectedIndex = 0;
            if (comboBoxSteps.Items.Count < numericUpDownSteps.Value)
            {
                while (comboBoxSteps.Items.Count < numericUpDownSteps.Value)
                {
                    int i = comboBoxSteps.Items.Count + 1;
                    comboBoxSteps.Items.Add(i);
                    _LEDCubeModel.StepsDiods.Add(i - 1, new bool[8,8,8]);
                    _LEDCubeModel.StepsTiming.Add(i - 1, 20);
                }
            }
            else if(comboBoxSteps.Items.Count > numericUpDownSteps.Value)
            {
                while (comboBoxSteps.Items.Count > numericUpDownSteps.Value)
                {
                    int i = comboBoxSteps.Items.Count;
                    comboBoxSteps.Items.Remove(i);
                    _LEDCubeModel.StepsDiods.Remove(i - 1);
                    _LEDCubeModel.StepsTiming.Remove(i - 1);
                }
            }
        }

        private void checkBoxIsTemplates_CheckStateChanged(object sender, EventArgs e)
        {
            comboBoxTemplatesSelect.Enabled = checkBoxIsTemplates.Checked;
            buttonGenerateTemplates.Enabled = checkBoxIsTemplates.Checked;
            numericUpDownSteps.Enabled = !checkBoxIsTemplates.Checked;
            comboBoxSteps.Enabled = !checkBoxIsTemplates.Checked;
            numericUpDownTiming.Enabled = !checkBoxIsTemplates.Checked;
        }

        private void numericUpDownTiming_ValueChanged(object sender, EventArgs e)
        {
            _LEDCubeModel.StepsTiming[comboBoxSteps.SelectedIndex] = int.Parse(numericUpDownTiming.Value.ToString());
        }

        private void comboBoxSteps_SelectedIndexChanged(object sender, EventArgs e)
        {
            _LEDCubeModel.StepsTiming[PreviousStep] = int.Parse(numericUpDownTiming.Value.ToString());
            numericUpDownTiming.Value = _LEDCubeModel.StepsTiming[comboBoxSteps.SelectedIndex];

            _LEDCubeModel.StepsDiods.Remove(PreviousStep);
            _LEDCubeModel.StepsDiods.Add(PreviousStep, _LEDCubeModel.LedCubeDiods);

            _LEDCubeModel.LedCubeDiods = _LEDCubeModel.StepsDiods[comboBoxSteps.SelectedIndex];

            UpdateLevels();

            PreviousStep = PreviousStep == comboBoxSteps.SelectedIndex ? PreviousStep : comboBoxSteps.SelectedIndex;
        }

        private void buttonGenerateTemplates_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            var templateForm = new TemplatesGenerator();
            templateForm.ShowDialog();
            this.Visible = true;
            devices = client.DiscoverDevicesInRange();
            if (devices.Count() > 0)
            {
                comboBoxSelectDevice.DataSource = devices;
                comboBoxSelectDevice.DisplayMember = "DeviceName";
                comboBoxSelectDevice.ValueMember = "DeviceAddress";
                comboBoxSelectDevice.Focus();
                comboBoxSelectDevice.SelectedIndex = 0;
            }

            templates = FileCoder.EncodeFileToLEDCubeTemplateModel();
            if (templates != null)
            {
                comboBoxTemplatesSelect.Items.AddRange(templates.Select(x => x.Name).ToArray());
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.Update(FileCoder.EncodeFileToLEDCubeModel(textBoxName.Text));
            comboBoxLevel.SelectedIndex = 1;
            comboBoxLevel.SelectedIndex = 0;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.Name = textBoxName.Text;
            FileCoder.CodeCubeToFile(_LEDCubeModel);
        }

        private void buttonA1_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 0] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 0];
            buttonA1.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 0] ? onImage : offImage;
        }

        private void buttonA2_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 1] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 1];
            buttonA2.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 1] ? onImage : offImage;
        }

        private void buttonA3_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 2] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 2];
            buttonA3.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 2] ? onImage : offImage;
        }

        private void buttonA4_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 3] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 3];
            buttonA4.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 3] ? onImage : offImage;
        }

        private void buttonA5_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 4] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 4];
            buttonA5.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 4] ? onImage : offImage;
        }

        private void buttonA6_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 5] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 5];
            buttonA6.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 5] ? onImage : offImage;
        }

        private void buttonA7_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 6] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 6];
            buttonA7.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 6] ? onImage : offImage;
        }

        private void buttonA8_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 7] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 7];
            buttonA8.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 0, 7] ? onImage : offImage;
        }

        private void buttonB1_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 0] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 0];
            buttonB1.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 0] ? onImage : offImage;
        }

        private void buttonB2_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 1] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 1];
            buttonB2.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 1] ? onImage : offImage;
        }

        private void buttonB3_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 2] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 2];
            buttonB3.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 2] ? onImage : offImage;
        }

        private void buttonB4_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 3] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 3];
            buttonB4.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 3] ? onImage : offImage;
        }

        private void buttonB5_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 4] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 4];
            buttonB5.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 4] ? onImage : offImage;
        }

        private void buttonB6_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 5] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 5];
            buttonB6.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 5] ? onImage : offImage;
        }

        private void buttonB7_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 6] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 6];
            buttonB7.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 6] ? onImage : offImage;
        }

        private void buttonB8_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 7] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 7];
            buttonB8.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 1, 7] ? onImage : offImage;
        }

        private void buttonC1_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 0] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 0];
            buttonC1.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 0] ? onImage : offImage;
        }

        private void buttonC2_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 1] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 1];
            buttonC2.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 1] ? onImage : offImage;
        }

        private void buttonC3_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 2] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 2];
            buttonC3.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 2] ? onImage : offImage;
        }

        private void buttonC4_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 3] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 3];
            buttonC4.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 3] ? onImage : offImage;
        }

        private void buttonC5_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 4] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 4];
            buttonC5.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 4] ? onImage : offImage;
        }

        private void buttonC6_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 5] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 5];
            buttonC6.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 5] ? onImage : offImage;
        }

        private void buttonC7_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 6] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 6];
            buttonC7.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 6] ? onImage : offImage;
        }

        private void buttonC8_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 7] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 7];
            buttonC8.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 2, 7] ? onImage : offImage;
        }

        private void buttonD1_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 0] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 0];
            buttonD1.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 0] ? onImage : offImage;
        }

        private void buttonD2_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 1] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 1];
            buttonD2.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 1] ? onImage : offImage;
        }

        private void buttonD3_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 2] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 2];
            buttonD3.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 2] ? onImage : offImage;
        }

        private void buttonD4_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 3] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 3];
            buttonD4.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 3] ? onImage : offImage;
        }

        private void buttonD5_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 4] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 4];
            buttonD5.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 4] ? onImage : offImage;
        }

        private void buttonD6_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 5] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 5];
            buttonD6.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 5] ? onImage : offImage;
        }

        private void buttonD7_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 6] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 6];
            buttonD7.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 6] ? onImage : offImage;
        }

        private void buttonD8_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 7] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 7];
            buttonD8.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 3, 7] ? onImage : offImage;
        }

        private void buttonE1_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 0] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 0];
            buttonE1.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 0] ? onImage : offImage;
        }

        private void buttonE2_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 1] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 1];
            buttonE2.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 1] ? onImage : offImage;
        }

        private void buttonE3_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 2] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 2];
            buttonE3.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 2] ? onImage : offImage;
        }

        private void buttonE4_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 3] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 3];
            buttonE4.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 3] ? onImage : offImage;
        }

        private void buttonE5_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 4] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 4];
            buttonE5.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 4] ? onImage : offImage;
        }

        private void buttonE6_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 5] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 5];
            buttonE6.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 5] ? onImage : offImage;
        }

        private void buttonE7_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 6] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 6];
            buttonE7.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 6] ? onImage : offImage;
        }

        private void buttonE8_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 7] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 7];
            buttonE8.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 4, 7] ? onImage : offImage;
        }

        private void buttonF1_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 0] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 0];
            buttonF1.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 0] ? onImage : offImage;
        }

        private void buttonF2_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 1] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 1];
            buttonF2.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 1] ? onImage : offImage;
        }

        private void buttonF3_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 2] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 2];
            buttonF3.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 2] ? onImage : offImage;
        }

        private void buttonF4_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 3] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 3];
            buttonF4.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 3] ? onImage : offImage;
        }

        private void buttonF5_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 4] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 4];
            buttonF5.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 4] ? onImage : offImage;
        }

        private void buttonF6_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 5] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 5];
            buttonF6.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 5] ? onImage : offImage;
        }

        private void buttonF7_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 6] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 6];
            buttonF7.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 6] ? onImage : offImage;
        }

        private void buttonF8_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 7] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 7];
            buttonF8.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 5, 7] ? onImage : offImage;
        }

        private void buttonG1_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 0] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 0];
            buttonG1.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 0] ? onImage : offImage;
        }

        private void buttonG2_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 1] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 1];
            buttonG2.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 1] ? onImage : offImage;
        }

        private void buttonG3_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 2] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 2];
            buttonG3.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 2] ? onImage : offImage;
        }

        private void buttonG4_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 3] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 3];
            buttonG4.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 3] ? onImage : offImage;
        }

        private void buttonG5_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 4] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 4];
            buttonG5.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 4] ? onImage : offImage;
        }

        private void buttonG6_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 5] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 5];
            buttonG6.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 5] ? onImage : offImage;
        }

        private void buttonG7_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 6] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 6];
            buttonG7.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 6] ? onImage : offImage;
        }

        private void buttonG8_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 7] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 7];
            buttonG8.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 6, 7] ? onImage : offImage;
        }

        private void buttonH1_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 0] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 0];
            buttonH1.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 0] ? onImage : offImage;
        }

        private void buttonH2_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 1] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 1];
            buttonH2.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 1] ? onImage : offImage;
        }

        private void buttonH3_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 2] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 2];
            buttonH3.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 2] ? onImage : offImage;
        }

        private void buttonH4_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 3] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 3];
            buttonH4.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 3] ? onImage : offImage;
        }

        private void buttonH5_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 4] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 4];
            buttonH5.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 4] ? onImage : offImage;
        }

        private void buttonH6_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 5] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 5];
            buttonH6.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 5] ? onImage : offImage;
        }

        private void buttonH7_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 6] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 6];
            buttonH7.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 6] ? onImage : offImage;
        }

        private void buttonH8_Click(object sender, EventArgs e)
        {
            _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 7] = !_LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 7];
            buttonH8.BackgroundImage = _LEDCubeModel.LedCubeDiods[_LEDCubeModel.CurrentLevel, 7, 7] ? onImage : offImage;
        }
    }
}
