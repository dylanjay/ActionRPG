using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Framework;
using Player;

namespace Managers
{
    public class GameManager : Singleton<MonobehaviourExtension>
    {
        public static PlayerController Player = FindObjectOfType<PlayerController>();

        private void Awake()
        {
            Instance = this;
        }
    }
}
