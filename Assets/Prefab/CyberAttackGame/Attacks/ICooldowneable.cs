using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Prefab.CyberAttackGame.Attacks
{
    public interface ICooldowneable
    {
        public float RandomCD { get; }
    }
}
