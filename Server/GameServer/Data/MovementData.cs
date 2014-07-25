using System.Collections.Generic;
using GameServer.Model;
using System;

namespace GameServer.Data
{
    public class MovementData
    {
        public bool IsMoving { get; set; }
        public List<Position> Path { get; set; }
        public float Speed { get; set; }
        public Action OnArriveAction { get; set; }
    }
}