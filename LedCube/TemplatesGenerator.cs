using LedCube.FileCode;
using LedCube.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LedCube
{
    public partial class TemplatesGenerator : Form
    {
        private LEDCubeTemplateActionsModel LEDCubeTemplateActionsModel = new LEDCubeTemplateActionsModel();
        private FileCoder FileCoder = new FileCoder();

        private string offFilePath = @"c:\users\artiom\source\repos\LedCube\LedCube\OffDiod.png";
        private string onFilePath = @"c:\users\artiom\source\repos\LedCube\LedCube\OnDiod.png";

        private Image onImage;
        private Image offImage;

        private int PreviousStep;

        private int CounterBefore;
        private int CounterAfter;

        private List<LEDCubeTemplateActionsModel> templates;

        public TemplatesGenerator()
        {
            InitializeComponent();
            InitializeLevelsAndDevices();
        }

        public void InitializeLevelsAndDevices()
        {
            onImage = Image.FromFile(onFilePath);
            offImage = Image.FromFile(offFilePath);
            comboBoxBeforeLevel.Items.AddRange(LEDCubeTemplateActionsModel.Levels);
            comboBoxAfterLevel.Items.AddRange(LEDCubeTemplateActionsModel.Levels);
            comboBoxSteps.Items.Add(1);
            comboBoxSteps.SelectedIndex = 0;
            comboBoxBeforeLevel.SelectedIndex = 0;
            comboBoxAfterLevel.SelectedIndex = 0;
            CounterBefore = 0;
            CounterAfter = 0;

            templates = FileCoder.EncodeFileToLEDCubeTemplateModel();
            if (templates != null)
            {
                comboBoxTemplates.Items.AddRange(templates.Select(x => x.Name).ToArray());
            }
        }

        private void comboBoxFirstLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFirstLevel();
        }

        private void UpdateFirstLevel()
        {
            LEDCubeTemplateActionsModel.FirstCurrentLevel = int.Parse(comboBoxBeforeLevel.SelectedItem.ToString()) - 1;
            int i = 7, j = 7;
            foreach (Button control in tableLayoutPanelFirstLedLevel.Controls)
            {
                control.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, j, i] ? onImage : offImage;
                control.Text = LEDCubeTemplateActionsModel.StepsStringsDiods[comboBoxSteps.SelectedIndex].First()[LEDCubeTemplateActionsModel.FirstCurrentLevel, j, i];
                i--;
                if (i < 0)
                {
                    i = 7;
                    j--;
                }
            }
        }

        private void comboBoxSecondLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSecondLevel();
        }

        private void UpdateSecondLevel()
        {
            LEDCubeTemplateActionsModel.SecondCurrentLevel = int.Parse(comboBoxAfterLevel.SelectedItem.ToString()) - 1;
            int i = 7, j = 7;
            foreach (Button control in tableLayoutPanelSecondLedLevel.Controls)
            {
                control.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, j, i] ? onImage : offImage;
                control.Text = LEDCubeTemplateActionsModel.StepsStringsDiods[comboBoxSteps.SelectedIndex].Last()[LEDCubeTemplateActionsModel.SecondCurrentLevel, j, i];
                i--;
                if (i < 0)
                {
                    i = 7;
                    j--;
                }
            }
        }

        private void UpdateBeforeCounter(Button button)
        {
            int i = 7, j = 7;
            var temp = LEDCubeTemplateActionsModel.StepsStringsDiods[comboBoxSteps.SelectedIndex].First();
            if (button.Text == "")
            {
                CounterBefore++;
                button.Text = CounterBefore.ToString();
                foreach (Button control in tableLayoutPanelFirstLedLevel.Controls)
                {
                    temp[comboBoxBeforeLevel.SelectedIndex, j, i] = control.Text;
                    i--;
                    if (i < 0)
                    {
                        i = 7;
                        j--;
                    }
                }
            }
            else
            {
                CounterBefore--;
                int thisCount = int.Parse(button.Text);
                foreach (Button control in tableLayoutPanelFirstLedLevel.Controls)
                {
                    temp[comboBoxBeforeLevel.SelectedIndex, j, i] = control.Text;
                    var currentNum = control.Text != "" ? int.Parse(control.Text) : 0;
                    if(currentNum> thisCount)
                    {
                        currentNum--;
                        control.Text = currentNum.ToString();
                    }
                    i--;
                    if (i < 0)
                    {
                        i = 7;
                        j--;
                    }
                }

                button.Text = "";
            }
            LEDCubeTemplateActionsModel.StepsStringsDiods[comboBoxSteps.SelectedIndex].RemoveAt(0);
            LEDCubeTemplateActionsModel.StepsStringsDiods[comboBoxSteps.SelectedIndex].Insert(0, temp);
        }

        private void UpdateAfterCounter(Button button)
        {
            int i = 7, j = 7;
            var temp = LEDCubeTemplateActionsModel.StepsStringsDiods[comboBoxSteps.SelectedIndex].Last();
            if (button.Text == "")
            {
                CounterAfter++;
                button.Text = CounterAfter.ToString();
                foreach (Button control in tableLayoutPanelSecondLedLevel.Controls)
                {
                    temp[comboBoxAfterLevel.SelectedIndex, j, i] = control.Text;
                    i--;
                    if (i < 0)
                    {
                        i = 7;
                        j--;
                    }
                }
            }
            else
            {
                CounterAfter--;
                int thisCount = int.Parse(button.Text);
                foreach (Button control in tableLayoutPanelSecondLedLevel.Controls)
                {
                    var currentNum = control.Text != "" ? int.Parse(control.Text) : 0;
                    temp[comboBoxAfterLevel.SelectedIndex, j, i] = control.Text;
                    if (currentNum > thisCount)
                    {
                        currentNum--;
                        control.Text = currentNum.ToString();
                    }
                    i--;
                    if (i < 0)
                    {
                        i = 7;
                        j--;
                    }
                }
                button.Text = "";
            }
            LEDCubeTemplateActionsModel.StepsStringsDiods[comboBoxSteps.SelectedIndex].RemoveAt(1);
            LEDCubeTemplateActionsModel.StepsStringsDiods[comboBoxSteps.SelectedIndex].Add(temp);
        }

        private void numericUpDownActions_ValueChanged(object sender, EventArgs e)
        {
            comboBoxSteps.SelectedIndex = 0;
            if (comboBoxSteps.Items.Count < numericUpDownActions.Value)
            {
                while (comboBoxSteps.Items.Count < numericUpDownActions.Value)
                {
                    int i = comboBoxSteps.Items.Count + 1;
                    comboBoxSteps.Items.Add(i);
                    var emptyCubes = new List<bool[,,]>();
                    emptyCubes.Add(new bool[8, 8, 8]);
                    emptyCubes.Add(new bool[8, 8, 8]);
                    LEDCubeTemplateActionsModel.StepsDiods.Add(i - 1, emptyCubes);
                    LEDCubeTemplateActionsModel.StepsTimingDiods.Add(i - 1, 20);
                    LEDCubeTemplateActionsModel.StepsTimingDiods.Add(i - 1 + 10, 20);
                    var emptyList = new List<string[,,]>();
                    emptyList.Add(new string[8, 8, 8]);
                    emptyList.Add(new string[8, 8, 8]);
                    LEDCubeTemplateActionsModel.StepsStringsDiods.Add(i - 1, emptyList);
                }
            }
            else if (comboBoxSteps.Items.Count > numericUpDownActions.Value)
            {
                while (comboBoxSteps.Items.Count > numericUpDownActions.Value)
                {
                    int i = comboBoxSteps.Items.Count;
                    comboBoxSteps.Items.Remove(i);
                    LEDCubeTemplateActionsModel.StepsDiods.Remove(i - 1);
                    LEDCubeTemplateActionsModel.StepsTimingDiods.Remove(i - 1);
                    LEDCubeTemplateActionsModel.StepsStringsDiods.Remove(i - 1);
                    LEDCubeTemplateActionsModel.StepsTimingDiods.Remove(i - 1 + 10);
                }
            }
        }

        private void comboBoxSteps_SelectedIndexChanged(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.CurrentActionStep = int.Parse(comboBoxSteps.SelectedItem.ToString()) - 1;

            var value = new List<bool[,,]>();
            value.Add(LEDCubeTemplateActionsModel.FirstLedCubeDiods);
            value.Add(LEDCubeTemplateActionsModel.SecondLedCubeDiods);
            LEDCubeTemplateActionsModel.StepsDiods.Remove(PreviousStep);
            LEDCubeTemplateActionsModel.StepsDiods.Add(PreviousStep, value);

            LEDCubeTemplateActionsModel.StepsTimingDiods[PreviousStep] = int.Parse(numericUpDownTiming.Value.ToString());
            LEDCubeTemplateActionsModel.StepsTimingDiods[PreviousStep] = int.Parse("1" + numericUpDownWait.Value.ToString());

            LEDCubeTemplateActionsModel.FirstLedCubeDiods = LEDCubeTemplateActionsModel.StepsDiods[LEDCubeTemplateActionsModel.CurrentActionStep].First();
            LEDCubeTemplateActionsModel.SecondLedCubeDiods = LEDCubeTemplateActionsModel.StepsDiods[LEDCubeTemplateActionsModel.CurrentActionStep].Last();

            comboBoxBeforeLevel.SelectedIndex = 0;
            comboBoxAfterLevel.SelectedIndex = 0;

            UpdateFirstLevel();
            UpdateSecondLevel();

            numericUpDownTiming.Value = LEDCubeTemplateActionsModel.StepsTimingDiods[PreviousStep];
            if(LEDCubeTemplateActionsModel.StepsTimingDiods.Count<2)
            {
                LEDCubeTemplateActionsModel.StepsTimingDiods.Add(10,20);
            }
            numericUpDownWait.Value = LEDCubeTemplateActionsModel.StepsTimingDiods[int.Parse("1" + PreviousStep.ToString())];

            PreviousStep = PreviousStep == LEDCubeTemplateActionsModel.CurrentActionStep ? PreviousStep : LEDCubeTemplateActionsModel.CurrentActionStep;
        }

        private void buttonReturn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.Name = textBoxName.Text;
            FileCoder.CodeCubeTemplateToFile(LEDCubeTemplateActionsModel);
        }

        private void buttonA1_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 0] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 0];
            buttonA1.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 0] ? onImage : offImage;
            UpdateBeforeCounter(buttonA1);
        }

        private void buttonA2_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 1] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 1];
            buttonA2.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 1] ? onImage : offImage;
            UpdateBeforeCounter(buttonA2);
        }

        private void buttonA3_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 2] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 2];
            buttonA3.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 2] ? onImage : offImage;
            UpdateBeforeCounter(buttonA3);
        }

        private void buttonA4_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 3] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 3];
            buttonA4.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 3] ? onImage : offImage;
            UpdateBeforeCounter(buttonA4);
        }

        private void buttonA5_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 4] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 4];
            buttonA5.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 4] ? onImage : offImage;
            UpdateBeforeCounter(buttonA5);
        }

        private void buttonA6_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 5] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 5];
            buttonA6.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 5] ? onImage : offImage;
            UpdateBeforeCounter(buttonA6);
        }

        private void buttonA7_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 6] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 6];
            buttonA7.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 6] ? onImage : offImage;
            UpdateBeforeCounter(buttonA7);
        }

        private void buttonA8_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 7] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 7];
            buttonA8.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 0, 7] ? onImage : offImage;
            UpdateBeforeCounter(buttonA8);
        }

        private void buttonB1_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 0] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 0];
            buttonB1.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 0] ? onImage : offImage;
            UpdateBeforeCounter(buttonB1);
        }

        private void buttonB2_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 1] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 1];
            buttonB2.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 1] ? onImage : offImage;
            UpdateBeforeCounter(buttonB2);
        }

        private void buttonB3_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 2] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 2];
            buttonB3.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 2] ? onImage : offImage;
            UpdateBeforeCounter(buttonB3);
        }

        private void buttonB4_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 3] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 3];
            buttonB4.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 3] ? onImage : offImage;
            UpdateBeforeCounter(buttonB4);
        }

        private void buttonB5_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 4] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 4];
            buttonB5.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 4] ? onImage : offImage;
            UpdateBeforeCounter(buttonB5);
        }

        private void buttonB6_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 5] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 5];
            buttonB6.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 5] ? onImage : offImage;
            UpdateBeforeCounter(buttonB6);
        }

        private void buttonB7_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 6] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 6];
            buttonB7.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 6] ? onImage : offImage;
            UpdateBeforeCounter(buttonB7);
        }

        private void buttonB8_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 7] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 7];
            buttonB8.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 1, 7] ? onImage : offImage;
            UpdateBeforeCounter(buttonB8);
        }

        private void buttonC1_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 0] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 0];
            buttonC1.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 0] ? onImage : offImage;
            UpdateBeforeCounter(buttonC1);
        }

        private void buttonC2_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 1] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 1];
            buttonC2.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 1] ? onImage : offImage;
            UpdateBeforeCounter(buttonC2);
        }

        private void buttonC3_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 2] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 2];
            buttonC3.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 2] ? onImage : offImage;
            UpdateBeforeCounter(buttonC3);
        }

        private void buttonC4_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 3] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 3];
            buttonC4.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 3] ? onImage : offImage;
            UpdateBeforeCounter(buttonC4);
        }

        private void buttonC5_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 4] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 4];
            buttonC5.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 4] ? onImage : offImage;
            UpdateBeforeCounter(buttonC5);
        }

        private void buttonC6_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 5] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 5];
            buttonC6.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 5] ? onImage : offImage;
            UpdateBeforeCounter(buttonC6);
        }

        private void buttonC7_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 6] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 6];
            buttonC7.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 6] ? onImage : offImage;
            UpdateBeforeCounter(buttonC7);
        }

        private void buttonC8_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 7] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 7];
            buttonC8.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 2, 7] ? onImage : offImage;
            UpdateBeforeCounter(buttonC8);
        }

        private void buttonD1_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 0] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 0];
            buttonD1.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 0] ? onImage : offImage;
            UpdateBeforeCounter(buttonD1);
        }

        private void buttonD2_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 1] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 1];
            buttonD2.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 1] ? onImage : offImage;
            UpdateBeforeCounter(buttonD2);
        }

        private void buttonD3_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 2] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 2];
            buttonD3.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 2] ? onImage : offImage;
            UpdateBeforeCounter(buttonD3);
        }

        private void buttonD4_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 3] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 3];
            buttonD4.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 3] ? onImage : offImage;
            UpdateBeforeCounter(buttonD4);
        }

        private void buttonD5_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 4] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 4];
            buttonD5.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 4] ? onImage : offImage;
            UpdateBeforeCounter(buttonD5);
        }

        private void buttonD6_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 5] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 5];
            buttonD6.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 5] ? onImage : offImage;
            UpdateBeforeCounter(buttonD6);
        }

        private void buttonD7_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 6] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 6];
            buttonD7.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 6] ? onImage : offImage;
            UpdateBeforeCounter(buttonD7);
        }

        private void buttonD8_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 7] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 7];
            buttonD8.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 3, 7] ? onImage : offImage;
            UpdateBeforeCounter(buttonD8);
        }

        private void buttonE1_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 0] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 0];
            buttonE1.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 0] ? onImage : offImage;
            UpdateBeforeCounter(buttonE1);
        }

        private void buttonE2_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 1] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 1];
            buttonE2.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 1] ? onImage : offImage;
            UpdateBeforeCounter(buttonE2);
        }

        private void buttonE3_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 2] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 2];
            buttonE3.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 2] ? onImage : offImage;
            UpdateBeforeCounter(buttonE3);
        }

        private void buttonE4_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 3] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 3];
            buttonE4.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 3] ? onImage : offImage;
            UpdateBeforeCounter(buttonE4);
        }

        private void buttonE5_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 4] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 4];
            buttonE5.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 4] ? onImage : offImage;
            UpdateBeforeCounter(buttonE5);
        }

        private void buttonE6_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 5] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 5];
            buttonE6.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 5] ? onImage : offImage;
            UpdateBeforeCounter(buttonE6);
        }

        private void buttonE7_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 6] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 6];
            buttonE7.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 6] ? onImage : offImage;
            UpdateBeforeCounter(buttonE7);
        }

        private void buttonE8_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 7] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 7];
            buttonE8.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 4, 7] ? onImage : offImage;
            UpdateBeforeCounter(buttonE8);
        }

        private void buttonF1_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 0] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 0];
            buttonF1.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 0] ? onImage : offImage;
            UpdateBeforeCounter(buttonF1);
        }

        private void buttonF2_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 1] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 1];
            buttonF2.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 1] ? onImage : offImage;
            UpdateBeforeCounter(buttonF2);
        }

        private void buttonF3_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 2] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 2];
            buttonF3.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 2] ? onImage : offImage;
            UpdateBeforeCounter(buttonF3);
        }

        private void buttonF4_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 3] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 3];
            buttonF4.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 3] ? onImage : offImage;
            UpdateBeforeCounter(buttonF4);
        }

        private void buttonF5_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 4] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 4];
            buttonF5.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 4] ? onImage : offImage;
            UpdateBeforeCounter(buttonF5);
        }

        private void buttonF6_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 5] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 5];
            buttonF6.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 5] ? onImage : offImage;
            UpdateBeforeCounter(buttonF6);
        }

        private void buttonF7_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 6] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 6];
            buttonF7.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 6] ? onImage : offImage;
            UpdateBeforeCounter(buttonF7);
        }

        private void buttonF8_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 7] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 7];
            buttonF8.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 5, 7] ? onImage : offImage;
            UpdateBeforeCounter(buttonF8);
        }

        private void buttonG1_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 0] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 0];
            buttonG1.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 0] ? onImage : offImage;
            UpdateBeforeCounter(buttonG1);
        }

        private void buttonG2_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 1] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 1];
            buttonG2.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 1] ? onImage : offImage;
            UpdateBeforeCounter(buttonG2);
        }

        private void buttonG3_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 2] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 2];
            buttonG3.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 2] ? onImage : offImage;
            UpdateBeforeCounter(buttonG3);
        }

        private void buttonG4_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 3] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 3];
            buttonG4.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 3] ? onImage : offImage;
            UpdateBeforeCounter(buttonG4);
        }

        private void buttonG5_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 4] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 4];
            buttonG5.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 4] ? onImage : offImage;
            UpdateBeforeCounter(buttonG5);
        }

        private void buttonG6_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 5] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 5];
            buttonG6.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 5] ? onImage : offImage;
            UpdateBeforeCounter(buttonG6);
        }

        private void buttonG7_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 6] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 6];
            buttonG7.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 6] ? onImage : offImage;
            UpdateBeforeCounter(buttonG7);
        }

        private void buttonG8_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 7] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 7];
            buttonG8.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 6, 7] ? onImage : offImage;
            UpdateBeforeCounter(buttonG8);
        }

        private void buttonH1_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 0] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 0];
            buttonH1.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 0] ? onImage : offImage;
            UpdateBeforeCounter(buttonH1);
        }

        private void buttonH2_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 1] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 1];
            buttonH2.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 1] ? onImage : offImage;
            UpdateBeforeCounter(buttonH2);
        }

        private void buttonH3_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 2] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 2];
            buttonH3.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 2] ? onImage : offImage;
            UpdateBeforeCounter(buttonH3);
        }

        private void buttonH4_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 3] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 3];
            buttonH4.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 3] ? onImage : offImage;
            UpdateBeforeCounter(buttonH4);
        }

        private void buttonH5_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 4] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 4];
            buttonH5.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 4] ? onImage : offImage;
            UpdateBeforeCounter(buttonH5);
        }

        private void buttonH6_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 5] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 5];
            buttonH6.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 5] ? onImage : offImage;
            UpdateBeforeCounter(buttonH6);
        }

        private void buttonH7_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 6] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 6];
            buttonH7.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 6] ? onImage : offImage;
            UpdateBeforeCounter(buttonH7);
        }

        private void buttonH8_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 7] = !LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 7];
            buttonH8.BackgroundImage = LEDCubeTemplateActionsModel.FirstLedCubeDiods[LEDCubeTemplateActionsModel.FirstCurrentLevel, 7, 7] ? onImage : offImage;
            UpdateBeforeCounter(buttonH8);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void buttonA11_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 0] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 0];
            buttonA11.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 0] ? onImage : offImage;
            UpdateAfterCounter(buttonA11);
        }

        private void buttonA21_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 1] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 1];
            buttonA21.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 1] ? onImage : offImage;
            UpdateAfterCounter(buttonA21);
        }

        private void buttonA31_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 2] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 2];
            buttonA31.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 2] ? onImage : offImage;
            UpdateAfterCounter(buttonA31);
        }

        private void buttonA41_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 3] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 3];
            buttonA41.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 3] ? onImage : offImage;
            UpdateAfterCounter(buttonA41);
        }

        private void buttonA51_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 4] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 4];
            buttonA51.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 4] ? onImage : offImage;
            UpdateAfterCounter(buttonA51);
        }

        private void buttonA61_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 5] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 5];
            buttonA61.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 5] ? onImage : offImage;
            UpdateAfterCounter(buttonA61);
        }

        private void buttonA71_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 6] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 6];
            buttonA71.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 6] ? onImage : offImage;
            UpdateAfterCounter(buttonA71);
        }

        private void buttonA81_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 7] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 7];
            buttonA81.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 0, 7] ? onImage : offImage;
            UpdateAfterCounter(buttonA81);
        }

        private void buttonB11_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 0] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 0];
            buttonB11.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 0] ? onImage : offImage;
            UpdateAfterCounter(buttonB11);
        }

        private void buttonB21_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 1] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 1];
            buttonB21.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 1] ? onImage : offImage;
            UpdateAfterCounter(buttonB21);
        }

        private void buttonB31_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 2] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 2];
            buttonB31.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 2] ? onImage : offImage;
            UpdateAfterCounter(buttonB31);
        }

        private void buttonB41_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 3] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 3];
            buttonB41.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 3] ? onImage : offImage;
            UpdateAfterCounter(buttonB41);
        }

        private void buttonB51_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 4] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 4];
            buttonB51.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 4] ? onImage : offImage;
            UpdateAfterCounter(buttonB51);
        }

        private void buttonB61_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 5] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 5];
            buttonB61.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 5] ? onImage : offImage;
            UpdateAfterCounter(buttonB61);
        }

        private void buttonB71_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 6] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 6];
            buttonB71.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 6] ? onImage : offImage;
            UpdateAfterCounter(buttonB71);
        }

        private void buttonB81_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 7] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 7];
            buttonB81.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 1, 7] ? onImage : offImage;
            UpdateAfterCounter(buttonB81);
        }

        private void buttonC11_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 0] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 0];
            buttonC11.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 0] ? onImage : offImage;
            UpdateAfterCounter(buttonC11);
        }

        private void buttonC21_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 1] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 1];
            buttonC21.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 1] ? onImage : offImage;
            UpdateAfterCounter(buttonC21);
        }

        private void buttonC31_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 2] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 2];
            buttonC31.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 2] ? onImage : offImage;
            UpdateAfterCounter(buttonC31);
        }

        private void buttonC41_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 3] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 3];
            buttonC41.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 3] ? onImage : offImage;
            UpdateAfterCounter(buttonC41);
        }

        private void buttonC51_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 4] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 4];
            buttonC51.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 4] ? onImage : offImage;
            UpdateAfterCounter(buttonC51);
        }

        private void buttonC61_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 5] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 5];
            buttonC61.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 5] ? onImage : offImage;
            UpdateAfterCounter(buttonC61);
        }

        private void buttonC71_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 6] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 6];
            buttonC71.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 6] ? onImage : offImage;
            UpdateAfterCounter(buttonC71);
        }

        private void buttonC81_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 7] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 7];
            buttonC81.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 2, 7] ? onImage : offImage;
            UpdateAfterCounter(buttonC81);
        }

        private void buttonD11_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 0] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 0];
            buttonD11.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 0] ? onImage : offImage;
            UpdateAfterCounter(buttonD11);
        }

        private void buttonD21_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 1] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 1];
            buttonD21.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 1] ? onImage : offImage;
            UpdateAfterCounter(buttonD21);
        }

        private void buttonD31_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 2] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 2];
            buttonD31.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 2] ? onImage : offImage;
            UpdateAfterCounter(buttonD31);
        }

        private void buttonD41_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 3] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 3];
            buttonD41.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 3] ? onImage : offImage;
            UpdateAfterCounter(buttonD41);
        }

        private void buttonD51_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 4] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 4];
            buttonD51.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 4] ? onImage : offImage;
            UpdateAfterCounter(buttonD51);
        }

        private void buttonD61_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 5] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 5];
            buttonD61.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 5] ? onImage : offImage;
            UpdateAfterCounter(buttonD61);
        }

        private void buttonD71_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 6] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 6];
            buttonD71.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 6] ? onImage : offImage;
            UpdateAfterCounter(buttonD71);
        }

        private void buttonD81_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 7] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 7];
            buttonD81.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 3, 7] ? onImage : offImage;
            UpdateAfterCounter(buttonD81);
        }

        private void buttonE11_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 0] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 0];
            buttonE11.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 0] ? onImage : offImage;
            UpdateAfterCounter(buttonE11);
        }

        private void buttonE21_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 1] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 1];
            buttonE21.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 1] ? onImage : offImage;
            UpdateAfterCounter(buttonE21);
        }

        private void buttonE31_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 2] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 2];
            buttonE31.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 2] ? onImage : offImage;
            UpdateAfterCounter(buttonE31);
        }

        private void buttonE41_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 3] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 3];
            buttonE41.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 3] ? onImage : offImage;
            UpdateAfterCounter(buttonE41);
        }

        private void buttonE51_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 4] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 4];
            buttonE51.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 4] ? onImage : offImage;
            UpdateAfterCounter(buttonE51);
        }

        private void buttonE61_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 5] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 5];
            buttonE61.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 5] ? onImage : offImage;
            UpdateAfterCounter(buttonE61);
        }

        private void buttonE71_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 6] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 6];
            buttonE71.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 6] ? onImage : offImage;
            UpdateAfterCounter(buttonE71);
        }

        private void buttonE81_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 7] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 7];
            buttonE81.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 4, 7] ? onImage : offImage;
            UpdateAfterCounter(buttonE81);
        }

        private void buttonF11_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 0] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 0];
            buttonF11.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 0] ? onImage : offImage;
            UpdateAfterCounter(buttonF11);
        }

        private void buttonF21_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 1] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 1];
            buttonF21.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 1] ? onImage : offImage;
            UpdateAfterCounter(buttonF21);
        }

        private void buttonF31_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 2] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 2];
            buttonF31.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 2] ? onImage : offImage;
            UpdateAfterCounter(buttonF31);
        }

        private void buttonF41_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 3] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 3];
            buttonF41.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 3] ? onImage : offImage;
            UpdateAfterCounter(buttonF41);
        }

        private void buttonF51_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 4] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 4];
            buttonF51.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 4] ? onImage : offImage;
            UpdateAfterCounter(buttonF51);
        }

        private void buttonF61_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 5] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 5];
            buttonF61.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 5] ? onImage : offImage;
            UpdateAfterCounter(buttonF61);
        }

        private void buttonF71_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 6] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 6];
            buttonF71.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 6] ? onImage : offImage;
            UpdateAfterCounter(buttonF71);
        }

        private void buttonF81_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 7] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 7];
            buttonF81.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 5, 7] ? onImage : offImage;
            UpdateAfterCounter(buttonF81);
        }

        private void buttonG11_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 0] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 0];
            buttonG11.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 0] ? onImage : offImage;
            UpdateAfterCounter(buttonG11);
        }

        private void buttonG21_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 1] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 1];
            buttonG21.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 1] ? onImage : offImage;
            UpdateAfterCounter(buttonG21);
        }

        private void buttonG31_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 2] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 2];
            buttonG31.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 2] ? onImage : offImage;
            UpdateAfterCounter(buttonG31);
        }

        private void buttonG41_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 3] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 3];
            buttonG41.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 3] ? onImage : offImage;
            UpdateAfterCounter(buttonG41);
        }

        private void buttonG51_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 4] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 4];
            buttonG51.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 4] ? onImage : offImage;
            UpdateAfterCounter(buttonG51);
        }

        private void buttonG61_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 5] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 5];
            buttonG61.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 5] ? onImage : offImage;
            UpdateAfterCounter(buttonG61);
        }

        private void buttonG71_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 6] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 6];
            buttonG71.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 6] ? onImage : offImage;
            UpdateAfterCounter(buttonG71);
        }

        private void buttonG81_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 7] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 7];
            buttonG81.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 6, 7] ? onImage : offImage;
            UpdateAfterCounter(buttonG81);
        }

        private void buttonH11_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 0] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 0];
            buttonH11.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 0] ? onImage : offImage;
            UpdateAfterCounter(buttonH11);
        }

        private void buttonH21_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 1] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 1];
            buttonH21.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 1] ? onImage : offImage;
            UpdateAfterCounter(buttonH21);
        }

        private void buttonH31_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 2] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 2];
            buttonH31.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 2] ? onImage : offImage;
            UpdateAfterCounter(buttonH31);
        }

        private void buttonH41_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 3] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 3];
            buttonH41.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 3] ? onImage : offImage;
            UpdateAfterCounter(buttonH41);
        }

        private void buttonH51_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 4] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 4];
            buttonH51.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 4] ? onImage : offImage;
            UpdateAfterCounter(buttonH51);
        }

        private void buttonH61_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 5] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 5];
            buttonH61.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 5] ? onImage : offImage;
            UpdateAfterCounter(buttonH61);
        }

        private void buttonH71_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 6] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 6];
            buttonH71.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 6] ? onImage : offImage;
            UpdateAfterCounter(buttonH71);
        }

        private void buttonH81_Click(object sender, EventArgs e)
        {
            LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 7] = !LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 7];
            buttonH81.BackgroundImage = LEDCubeTemplateActionsModel.SecondLedCubeDiods[LEDCubeTemplateActionsModel.SecondCurrentLevel, 7, 7] ? onImage : offImage;
            UpdateAfterCounter(buttonH81);
        }
    }
}
