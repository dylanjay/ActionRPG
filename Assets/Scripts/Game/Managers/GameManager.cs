using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BenCo.Extensions;
using BenCo.Framework;
using BenCo.Player;

namespace BenCo.Managers
{
    public class GameManager : Singleton<MonoBehaviourWrapper>
    {
        public static PlayerController Player = FindObjectOfType<PlayerController>();

        private void Awake()
        {
            Instance = this;
        }
    }
}
