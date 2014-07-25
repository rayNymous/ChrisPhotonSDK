using System.Collections.Generic;
using ExitGames.Logging;
using GameServer.Model;
using GameServer.Model.Interfaces;

namespace GameServer.Effects
{
    public class AreaOfEffectManager
    {
        private readonly List<AreaOfEffect> _effects;

        protected ILogger Log = LogManager.GetCurrentClassLogger();
        private GZone _zone;

        public AreaOfEffectManager(GZone zone)
        {
            _effects = new List<AreaOfEffect>();
            _zone = zone;
        }

        public AreaOfEffectManager()
        {
            _effects = new List<AreaOfEffect>();
        }

        public void AddEffect(AreaOfEffect effect)
        {
            _effects.Add(effect);
        }

        public void Start()
        {
            foreach (AreaOfEffect effect in _effects)
            {
                effect.Start();
            }
        }

        public void Stop()
        {
            foreach (AreaOfEffect effect in _effects)
            {
                effect.Stop();
            }
        }

        public void NotifyPositionChange(ICharacter character)
        {
            foreach (AreaOfEffect effect in _effects)
            {
                if (effect.Contains(character))
                {
                    effect.OnCharacterPositionChanged(character);
                }
                else if (effect.IsInRegion(character))
                {
                    effect.AddCharacter(character);
                }
            }
        }

        public void RemoveCharacter(GCharacter character)
        {
            foreach (AreaOfEffect effect in _effects)
            {
                effect.RemoveCharacter(character);
            }
        }
    }
}