using System;
using UnityEngine;

namespace Eclipse.Game
{
    public static class SkillManagerFactory
    {
        public static SkillManager Create(PlayerData playerData, GameObject skillManagerObject)
        {
            int playerClass = PlayerPrefs.GetInt("PlayerClass");

            switch (playerClass)
            {
                case 0:
                    return skillManagerObject.AddComponent<Mage_SkillManager>();
                case 1:
                    return skillManagerObject.AddComponent<Warrior_SkillManager>();
                case 2:
                    return skillManagerObject.AddComponent<Assassin_SkillManager>();
                default:
                    throw new ArgumentException("Invalid player class");
            }
        }
    }
}

