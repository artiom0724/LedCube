using LedCube.Model;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace LedCube.FileCode
{
    public class FileCoder
    {
        private string pathToCubeFile = @"c:\users\artiom\source\repos\LedCube\LedCube\cube.txt";
        private string pathToCubeTemplateFile = @"c:\users\artiom\source\repos\LedCube\LedCube\cubetemplate.txt";
        private int numLine;
        
        public FileCoder()
        {
            numLine = 0;
        }

        public void CodeCubeToFile(LEDCubeModel cube)
        {
            FileStream fs;
            if (!File.Exists(pathToCubeFile))
            {
                fs = File.Create(pathToCubeFile);
                fs.Close();
            }
            int numLine = 0;
            using (StreamReader sr = new StreamReader(pathToCubeFile))
            {
                while (sr.Peek() >= 0)
                {
                    sr.ReadLine();
                    numLine++;
                }
            }
            var fileString = GenerateFileString(cube, numLine);
            using (StreamWriter sw = new StreamWriter(pathToCubeFile))
            {
                sw.WriteLine(fileString);
            }
        }

        private string GenerateFileString(LEDCubeModel cube, int number)
        {
            var tempCount = cube.IsTemplate ? cube.CurrentLEDCubeTemplateActionsModel.StepsStringsDiods.Count.ToString() : cube.StepsDiods.Count.ToString();
            string result = $"*{number}*+{cube.Name}+t{(cube.IsTemplate ? cube.CurrentLEDCubeTemplateActionsModel.Name : "0")}tm{tempCount}m-";
            if (!cube.IsTemplate)
            {
                for (int i = 0; i < cube.StepsDiods.Count; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        for (int k = 0; k < 8; k++)
                        {
                            for (int l = 0; l < 8; l++)
                            {
                                result += cube.StepsDiods[i][j, k, l] ? "1" : "0";
                            }
                        }
                    }
                    result += "-" + cube.StepsTiming[i].ToString() + "-";
                }
            }
            else
            {
                for (int step = 0; step < cube.CurrentLEDCubeTemplateActionsModel.StepsStringsDiods.Count; step++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            for (int k = 0; k < 8; k++)
                            {
                                result += cube.StepsDiods[step][i, j, k] ? "1" : "0";
                            }
                        }
                    }
                    result += $"T{cube.CurrentLEDCubeTemplateActionsModel.StepsTimingDiods[step]}T";
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            for (int k = 0; k < 8; k++)
                            {
                                if (cube.CurrentLEDCubeTemplateActionsModel.StepsDiods[step].First()[i, j, k])
                                {
                                    var firstNum = int.Parse(cube.CurrentLEDCubeTemplateActionsModel.StepsStringsDiods[step].First()[i, j, k]);
                                    int secondNum = -1;
                                    for (int i2 = 0; i2 < 8; i2++)
                                    {
                                        for (int j2 = 0; j2 < 8; j2++)
                                        {
                                            for (int k2 = 0; k2 < 8; k2++)
                                            {
                                                var secondTarget = cube.CurrentLEDCubeTemplateActionsModel.StepsStringsDiods[step].Last()[i2, j2, k2];
                                                if (int.Parse(secondTarget) == firstNum)
                                                {
                                                    secondNum = i2 * 100 + j2 * 10 + k2;
                                                }
                                            }
                                        }
                                    }
                                    if (secondNum != -1)
                                    {
                                        cube.StepsDiods[step][i, j, k] = cube.StepsDiods[step][secondNum / 100, (secondNum / 10) % 10, secondNum % 10];
                                    }
                                    result += cube.StepsDiods[step][i, j, k] ? "1" : "0";
                                }
                                else
                                {
                                    result += cube.StepsDiods[step][i, j, k] ? "1" : "0";
                                }
                            }
                        }
                    }
                    result += $"T{cube.CurrentLEDCubeTemplateActionsModel.StepsTimingDiods[step + 10]}T";
                }
            }
            return result + "#\n";
        }

        public void CodeCubeTemplateToFile(LEDCubeTemplateActionsModel cube)
        {
            FileStream fs;
            if (!File.Exists(pathToCubeTemplateFile))
            {
                fs = File.Create(pathToCubeTemplateFile);
                fs.Close();
            }
            using (StreamReader sr = new StreamReader(pathToCubeTemplateFile))
            {
                while (sr.Peek() >= 0)
                {
                    sr.ReadLine();
                    numLine++;
                }
            }
            var fileString = GenerateFileStringTemplate(cube, numLine);
            using (StreamWriter sw = new StreamWriter(pathToCubeTemplateFile))
            {
                sw.WriteLine(fileString);
            }
        }

        private string GenerateFileStringTemplate(LEDCubeTemplateActionsModel cube, int numLine)
        {
            string result = $"*{numLine}*+{cube.Name}+m{cube.StepsDiods.Count}m-";
            string before = string.Empty, after = string.Empty;
            for (int i = 0; i < cube.StepsDiods.Count; i++)
            {
                result += cube.StepsTimingDiods[i].ToString() + "-";
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        for (int l = 0; l < 8; l++)
                        {
                            var test = cube.StepsStringsDiods[i].First()[j, k, l];
                            if (test == "" || test == null)
                            {
                                before += "0,";
                            }
                            else
                            {
                                string next = string.Empty;
                                for (int j1 = 0; j1 < 8; j1++)
                                {
                                    for (int k1 = 0; k1 < 8; k1++)
                                    {
                                        for (int l1 = 0; l1 < 8; l1++)
                                        {
                                            next = cube.StepsStringsDiods[i].Last()[j1, k1, l1];
                                            if(next == test)
                                            {
                                                next = j1.ToString() + k1.ToString() + l1.ToString();
                                                before += next + ",";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                result += before + "-" + cube.StepsTimingDiods[i+10].ToString() + "-";
                before = string.Empty;
            }
            return result;
        }

        public LEDCubeModel EncodeFileToLEDCubeModel(string name)
        {
            if (!File.Exists(pathToCubeFile))
            {
                return null;
            }
            using (StreamReader sr = new StreamReader(pathToCubeFile))
            {
                string resLine = string.Empty;
                numLine = 0;
                while (sr.Peek() >= 0)
                {
                    numLine++;
                    var line = sr.ReadLine();
                    if (line.Contains(name))
                    {
                        resLine = line;
                    }
                }
                if(resLine == string.Empty)
                {
                    return null;
                }
                return GetLEDCubeModelFromString(resLine);
            }
        }

        private LEDCubeModel GetLEDCubeModelFromString(string line)
        {
            var resultModel = new LEDCubeModel();
            resultModel.StepsTiming = new Dictionary<int, int>();
            resultModel.StepsDiods = new Dictionary<int, bool[,,]>();
            for (int str = 0; str < line.Count(); str++)
            {
                if (line[str] == '+')
                {
                    string name = string.Empty;
                    str++;
                    while (line[str] != '+')
                    {
                        name += line[str];
                        str++;
                    }
                    str++;
                    resultModel.Name = name;
                }
                if (line[str] == 't')
                {
                    str++;
                    resultModel.IsTemplate = line[str] == '1';
                    str++;
                }
                if (line[str] == 'm')
                {
                    str++;
                    string numStepsStr = string.Empty;
                    while (line[str] != 'm')
                    {
                        numStepsStr += line[str];
                        str++;
                    }
                    int numSteps = int.Parse(numStepsStr);
                    for (int k = 0; k < numSteps; k++)
                    {
                        resultModel.StepsDiods.Add(k, new bool[8, 8, 8]);
                    }
                    str++;
                }
                if (line[str] == '-')
                {
                    str++;
                    for (int step = 0; step < resultModel.StepsDiods.Count; step++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                for (int k = 0; k < 8; k++)
                                {
                                    resultModel.StepsDiods[step][i, j, k] = line[str] == '1';
                                    str++;
                                }
                            }
                        }
                        str++;
                        string timing = string.Empty;
                        while (line[str] != '-')
                        {
                            timing += line[str];
                            str++;
                        }
                        resultModel.StepsTiming.Add(step, int.Parse(timing));
                        str++;
                    }
                }
            }
            return resultModel;
        }

        public List<LEDCubeTemplateActionsModel> EncodeFileToLEDCubeTemplateModel()
        {
            if (!File.Exists(pathToCubeTemplateFile))
            {
                return null;
            }
            var resultList = new List<LEDCubeTemplateActionsModel>();
            using (StreamReader sr = new StreamReader(pathToCubeTemplateFile))
            {
                numLine = 0;
                while (sr.Peek() >= 0)
                {
                    numLine++;
                    var line = sr.ReadLine();
                    var model = GetLEDCubeTemplateActionsModel(line);
                    if(model!=null)
                    {
                        resultList.Add(model);
                    }
                }
            }
            return resultList.Count != 0 ? resultList : null;
        }

        private LEDCubeTemplateActionsModel GetLEDCubeTemplateActionsModel(string line)
        {
            if (line.Count() > 10)
            {
                var model = new LEDCubeTemplateActionsModel();
                model.StepsTimingDiods = new Dictionary<int, int>();
                model.FirstLedCubeDiods = new bool[8,8,8];
                for (int str = 0; str < line.Count(); str++)
                {
                    if (line[str] == '+')
                    {
                        string name = string.Empty;
                        str++;
                        while (line[str] != '+')
                        {
                            name += line[str];
                            str++;
                        }
                        str++;
                        model.Name = name;
                    }
                    if (line[str] == 'm')
                    {
                        str++;
                        string numStepsStr = string.Empty;
                        while (line[str] != 'm')
                        {
                            numStepsStr += line[str];
                            str++;
                        }
                        int numSteps = int.Parse(numStepsStr);
                        for (int k = 0; k < numSteps; k++)
                        {
                            var list = new List<bool[,,]>();
                            model.StepsDiods.Add(k, list);
                        }
                        str++;
                    }
                    if (line[str] == '-')
                    {
                        str++;
                        int counter = 1;
                        for (int step = 0; step < model.StepsDiods.Count; step++)
                        {
                            string timing = string.Empty;
                            while (line[str] != '-')
                            {
                                timing += line[str];
                                str++;
                            }
                            str++;
                            model.StepsTimingDiods[step] = int.Parse(timing);
                            var readingFirst = new string[8, 8, 8];
                            var readingSecond = new string[8, 8, 8];
                            var boolReadingFirst = new bool[8, 8, 8];
                            var boolReadingSecond = new bool[8, 8, 8];
                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    for (int k = 0; k < 8; k++)
                                    {
                                        if(line[str+1]==',')
                                        {
                                            readingFirst[i, j, k] = string.Empty;
                                            str += 2;
                                        }
                                        else{
                                            string position = string.Empty;
                                            while (line[str] != ',')
                                            {
                                                position += line[str];
                                                str++;
                                            }
                                            str++;
                                            var intPosition = int.Parse(position);
                                            readingFirst[i, j, k] = counter.ToString();
                                            boolReadingFirst[i, j, k] = true;
                                            readingSecond[intPosition / 100, (intPosition / 10) % 10, intPosition % 10] = counter.ToString();
                                            boolReadingSecond[intPosition / 100, (intPosition / 10) % 10, intPosition % 10] = true;
                                            counter++;
                                        }
                                    }
                                }
                            }
                            model.StepsStringsDiods[step].Add(readingFirst);
                            model.StepsStringsDiods[step].Add(readingSecond);
                            model.StepsDiods[step].Add(boolReadingFirst);
                            model.StepsDiods[step].Add(boolReadingSecond);

                            str++;
                            timing = string.Empty;
                            while (line[str] != '-')
                            {
                                timing += line[str];
                                str++;
                            }
                            model.StepsTimingDiods[step+10] = int.Parse(timing);
                            str++;
                        }
                    }
                }
                return model;
            }
            return null;
        }


    }
}
