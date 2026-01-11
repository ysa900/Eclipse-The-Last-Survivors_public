using Eclipse.Game.Panels;
using System;
using UnityEngine;

namespace Eclipse.Game
{
    public static class ActiveSkillPanelFactory
    {
        public static ActiveSkillPanel Create(PlayerData playerData, GameObject panelObject)
        {
            int playerClass = PlayerPrefs.GetInt("PlayerClass");
            switch (playerClass)
            {
                case 0:
                    return panelObject.AddComponent<Mage_ASP>();
                case 1:
                    return panelObject.AddComponent<Warrior_ASP>();
                case 2:
                    return panelObject.AddComponent<Assassin_ASP>();
                default:
                    throw new ArgumentException("Invalid player class");
            }
        }
    }
}