using UnityEngine;
using System.Collections.Generic;

public class LevelDataGenerator : MonoBehaviour
{
    [ContextMenu("Generate All 50 Levels")]
    public void GenerateAllLevels()
    {
        for (int i = 1; i <= 50; i++)
        {
            GenerateLevel(i);
        }
        
        Debug.Log("All 50 levels generated!");
    }
    
    void GenerateLevel(int levelNumber)
    {
        LevelData level = ScriptableObject.CreateInstance<LevelData>();
        level.levelNumber = levelNumber;
        level.levelName = $"Level {levelNumber}";
        
        // تحديد نوع المرحلة
        if (levelNumber <= 10)
        {
            SetupScoreGoalLevel(level, levelNumber);
        }
        else if (levelNumber <= 20)
        {
            SetupIceBlockLevel(level, levelNumber);
        }
        else if (levelNumber <= 30)
        {
            SetupJewelLevel(level, levelNumber);
        }
        else if (levelNumber <= 40)
        {
            SetupBombLevel(level, levelNumber);
        }
        else
        {
            SetupBossLevel(level, levelNumber);
        }
        
        // حفظ الـ Asset
        #if UNITY_EDITOR
        string path = $"Assets/Resources/LevelData/Level_{levelNumber:D2}.asset";
        UnityEditor.AssetDatabase.CreateAsset(level, path);
        UnityEditor.AssetDatabase.SaveAssets();
        #endif
    }
    
    void SetupScoreGoalLevel(LevelData level, int levelNum)
    {
        level.levelType = LevelType.ScoreGoal;
        level.goals.targetScore = 500 + (levelNum * 100);
        
        // جميع الأشكال متاحة
        level.allowSquare = true;
        level.allowLine = true;
        level.allowL = true;
        level.allowT = true;
        level.allowSingle = true;
        
        level.iceBlockPositions = new Vector2Int[0];
        level.jewelPositions = new Vector2Int[0];
        level.bombPositions = new Vector2Int[0];
    }
    
    void SetupIceBlockLevel(LevelData level, int levelNum)
    {
        level.levelType = LevelType.IceBlocks;
        level.goals.targetScore = 1000 + ((levelNum - 10) * 150);
        
        int iceCount = 3 + (levelNum - 10);
        level.iceBlockPositions = GetRandomPositions(iceCount);
        
        level.jewelPositions = new Vector2Int[0];
        level.bombPositions = new Vector2Int[0];
    }
    
    void SetupJewelLevel(LevelData level, int levelNum)
    {
        level.levelType = LevelType.Jewels;
        level.goals.targetJewels = 2 + (levelNum - 20);
        
        int jewelCount = level.goals.targetJewels + 2;
        level.jewelPositions = GetRandomPositions(jewelCount);
        
        level.iceBlockPositions = new Vector2Int[0];
        level.bombPositions = new Vector2Int[0];
    }
    
    void SetupBombLevel(LevelData level, int levelNum)
    {
        level.levelType = LevelType.TimeBomb;
        level.goals.bombMoves = 15 - ((levelNum - 30) / 2);
        level.goals.targetScore = 2000 + ((levelNum - 30) * 100);
        
        int bombCount = 2 + ((levelNum - 30) / 3);
        level.bombPositions = GetRandomPositions(bombCount);
        
        level.iceBlockPositions = new Vector2Int[0];
        level.jewelPositions = new Vector2Int[0];
    }
    
    void SetupBossLevel(LevelData level, int levelNum)
    {
        level.levelType = LevelType.BossLevel;
        level.goals.targetScore = 3000 + ((levelNum - 40) * 200);
        level.goals.preFilledPercentage = 40;
        
        // مزيج من كل شيء
        int iceCount = 3;
        int jewelCount = 3;
        int bombCount = 2;
        
        level.iceBlockPositions = GetRandomPositions(iceCount);
        level.jewelPositions = GetRandomPositions(jewelCount);
        level.bombPositions = GetRandomPositions(bombCount);
    }
    
    Vector2Int[] GetRandomPositions(int count)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        
        for (int i = 0; i < count; i++)
        {
            Vector2Int pos;
            do
            {
                pos = new Vector2Int(Random.Range(0, 8), Random.Range(0, 8));
            } while (positions.Contains(pos));
            
            positions.Add(pos);
        }
        
        return positions.ToArray();
    }
}