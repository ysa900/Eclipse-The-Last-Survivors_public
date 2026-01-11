using System;
using UnityEngine;

namespace Eclipse.Game
{
    public static class SkillSelectManagerFactory
    {
        public static SkillSelectManager Create(PlayerData playerData, GameObject ssmObject)
        {
            int playerClass = PlayerPrefs.GetInt("PlayerClass");
            switch (playerClass)
            {
                case 0:
                    return ssmObject.AddComponent<Mage_SSM>();
                case 1:
                    return ssmObject.AddComponent<Warrior_SSM>();
                case 2:
                    return ssmObject.AddComponent<Assassin_SSM>();
                default:
                    throw new ArgumentException("Invalid player class");
            }
        }
    }
}