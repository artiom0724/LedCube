using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedCube.Model
{
    public class LEDCubeTemplateActionsModel
    {
        public string Name { get; set; }

        public bool[,,] FirstLedCubeDiods { get; set; }

        public bool[,,] SecondLedCubeDiods { get; set; }

        public Dictionary<int, List<string[,,]>> StepsStringsDiods { get; set; }

        public object[] Levels { get; set; }

        public int FirstCurrentLevel { get; set; }

        public int SecondCurrentLevel { get; set; }

        public int CurrentActionStep { get; set; }

        public Dictionary<int,List<bool[,,]>> StepsDiods {get;set;}

        public Dictionary<int, int> StepsTimingDiods { get; set; }

        public LEDCubeTemplateActionsModel()
        {
            FirstLedCubeDiods = new bool[8, 8, 8];
            SecondLedCubeDiods = new bool[8, 8, 8];
            FirstCurrentLevel = 0;
            SecondCurrentLevel = 0;
            CurrentActionStep = 0;
            Levels = new object[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
            StepsDiods = new Dictionary<int, List<bool[,,]>>();
            StepsTimingDiods = new Dictionary<int, int>();
            StepsStringsDiods = new Dictionary<int, List<string[,,]>>();
            var emptyList = new List<string[,,]>();
            emptyList.Add(new string[8, 8, 8]);
            emptyList.Add(new string[8, 8, 8]);
            StepsStringsDiods.Add(0, emptyList);
        }
    }
}
