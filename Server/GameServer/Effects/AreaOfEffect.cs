using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ExitGames.Logging;
using GameServer.Model;
using GameServer.Model.Interfaces;

namespace GameServer.Effects
{
    public class AreaOfEffect
    {
        private readonly ConcurrentDictionary<int, ICharacter> _characters;

        private readonly AreaOfEffectData _data;
        protected ILogger Log = LogManager.GetCurrentClassLogger();

        private TimeSpan _delay;
        private bool _stop;

        public AreaOfEffect(AreaOfEffectData data)
        {
            _characters = new ConcurrentDictionary<int, ICharacter>();
            _data = data;
        }

        public static int Priority { get; set; }

        public void Start()
        {
            _stop = false;

            if (_data.Options.ContainsKey("repeat"))
            {
                _delay = TimeSpan.FromMilliseconds(Convert.ToInt32(_data.Options["repeat"]));
                Schedule();
            }
        }

        public void Stop()
        {
            _stop = true;
        }

        public bool IsInRegion(ICharacter character)
        {
            Position pos = character.Position;

            if (_data.StartX <= pos.X && _data.EndX >= pos.X)
            {
                if (_data.StartY >= pos.Y && _data.EndY <= pos.Y)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(ICharacter character)
        {
            return _characters.ContainsKey(character.InstanceId);
        }

        public void AddCharacter(ICharacter character)
        {
            if (!_data.PlayersOnly || _data.PlayersOnly && character as GPlayerInstance != null)
            {
                if (character != null && !_characters.ContainsKey(character.InstanceId))
                {
                    _characters.TryAdd(character.InstanceId, character);
                    OnCharacterEntered(character);
                }
            }
        }

        public void RemoveCharacter(ICharacter character)
        {
            if (character != null)
            {
                _characters.TryRemove(character.InstanceId, out character);
                OnCharacterLeft(character);
            }
        }

        public virtual void OnCharacterPositionChanged(ICharacter character)
        {
            if (!IsInRegion(character))
            {
                RemoveCharacter(character);
            }
        }

        public virtual void OnCharacterEntered(ICharacter character)
        {
            if (_data.OnEnter != null)
            {
                _data.OnEnter(character);
            }
        }

        public virtual void OnCharacterLeft(ICharacter character)
        {
            if (_data.OnLeave != null)
            {
                _data.OnLeave(character);
            }
        }

        private async void Schedule()
        {
            await Task.Delay(_delay);
            if (!_stop)
            {
                foreach (var character in _characters)
                {
                    _data.Effect(character.Value);
                }
                Schedule();
            }
        }
    }
}