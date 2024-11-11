using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace Color_Clique
{
    public class CSVtoSO
    {
        //Check .csv path
        private static string CSVPath = "/Editor/CSVtoSO/Color_Clique/LevelCSV.csv";

        [MenuItem("Tools/CSV_to_SO/Color_Clique/Generate")]
        public static void GenerateSO()
        {
            int startingNamingIndex = 1;
            string[] allLines = File.ReadAllLines(Application.dataPath + CSVPath);

            for (int i = 1; i < allLines.Length; i++)
            {
                allLines[i] = RedefineString(allLines[i]);
            }

            for (int i = 1; i < allLines.Length; i++)
            {
                string[] splitData = allLines[i].Split(';');

                //Check data indexes
                LevelSO level = ScriptableObject.CreateInstance<LevelSO>();
                level.levelId = int.Parse(splitData[0]);
                level.numberOfColors = int.Parse(splitData[1]);
                level.shapeCount = int.Parse(splitData[2]);
                level.wheelSegments = int.Parse(splitData[3]);
                level.spinSpeedMultiplier = float.Parse(splitData[4]);
                level.answerTime = float.Parse(splitData[5]);
                level.isWheelBarReversalEnabled = Convert.ToBoolean(int.Parse(splitData[6]));
                level.minChangeFrequency = int.Parse(splitData[7]);
                level.maxChangeFrequency = int.Parse(splitData[8]);
                level.levelUpCriteria = int.Parse(splitData[9]);
                level.levelDownCriteria = int.Parse(splitData[10]);
                level.comboScore = int.Parse(splitData[11]);
                level.pointsPerQuestion = int.Parse(splitData[12]);
                level.penaltyPoints = int.Parse(splitData[13]);

                AssetDatabase.CreateAsset(level, $"Assets/Data/Color_Clique/Levels/{"ColorClique_Level " + startingNamingIndex}.asset");
                startingNamingIndex++;
            }

            AssetDatabase.SaveAssets();

            static string RedefineString(string val)
            {
                char[] charArr = val.ToCharArray();
                bool isSplittable = true;

                for (int i = 0; i < charArr.Length; i++)
                {
                    if (charArr[i] == '"')
                    {
                        charArr[i] = ' ';
                        isSplittable = !isSplittable;
                    }

                    if (isSplittable && charArr[i] == ',')
                        charArr[i] = ';';

                    if (isSplittable && charArr[i] == '.')
                        charArr[i] = ',';
                }

                return new string(charArr);
            }
        }
    }
}