using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedCube.Model
{
    public class LEDCubeModel
    {
        public string Name { get; set; }

        public bool[,,] LedCubeDiods { get; set; }

        public object[] Levels { get; set; }

        public int CurrentLevel { get; set; }

        public bool IsTemplate { get; set; }

        public Dictionary<int, int> StepsTiming { get; set; }

        public Dictionary<int, bool[,,]> StepsDiods { get; set; }

        public LEDCubeTemplateActionsModel CurrentLEDCubeTemplateActionsModel { get; set; }

        public LEDCubeModel()
        {
            LedCubeDiods = new bool[8, 8, 8];          
            CurrentLevel = 0;
            Levels = new object[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
            StepsDiods = new Dictionary<int, bool[,,]>();
            StepsTiming = new Dictionary<int, int>();
        }

        internal void Update(LEDCubeModel model)
        {
            if (model != null)
            {
                StepsDiods = model.StepsDiods;
                Name = model.Name;
                LedCubeDiods = model.StepsDiods[0];
                CurrentLevel = 0;
                Levels = new object[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
                StepsTiming = model.StepsTiming;
            }
        }
    }
}
