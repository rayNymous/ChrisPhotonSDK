using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ExitGames.Logging;
using GameServer.Calculators;
using GameServer.Model.Interfaces;

namespace GameServer.Model.Stats
{
    public class StatHolder : IStatHolder
    {
        private readonly Dictionary<Type, IStat> _stats;

        protected Dictionary<IStat, Calculator> Calculators = new Dictionary<IStat, Calculator>();

        protected ILogger Log = LogManager.GetCurrentClassLogger();

        public StatHolder(IEnumerable<IStat> stats)
        {
            _stats = new Dictionary<Type, IStat>();
            foreach (IStat stat in stats)
            {
                Calculators.Add(stat, new Calculator());
                _stats.Add(stat.GetType(), stat);
                var derived = stat as IDerivedStat;
                if (derived != null)
                {
                    Calculators[stat].AddFunction(derived.Functions);
                }
            }
        }

        public ICharacter Character { get; set; }

        public Dictionary<Type, IStat> Stats
        {
            get { return _stats; }
        }

        public void AddModifier(IFunction function)
        {
            IStat stat;
            _stats.TryGetValue(function.Stat.GetType(), out stat);
            if (stat != null)
            {
                Calculators[stat].AddFunction(function);
            }
        }

        public void RemoveModifier(IFunction function)
        {
            IStat stat;
            _stats.TryGetValue(function.Stat.GetType(), out stat);
            if (stat != null)
            {
                Calculators[stat].RemoveFunction(function);
            }
        }

        public float GetStat<T>() where T : class, IStat
        {
            IStat result;
            _stats.TryGetValue(typeof (T), out result);
            if (result != null)
            {
                return CalcStat(result);
            }
            return 0;
        }

        public float GetStat<T>(T stat) where T : class, IStat
        {
            IStat result;
            _stats.TryGetValue(typeof (T), out result);
            if (result == null)
            {
                _stats.TryGetValue(((dynamic) stat).GetType(), out result);
            }
            if (result != null)
            {
                return CalcStat(result);
            }
            return 0;
        }

        public void SetStat<T>(float value) where T : class, IStat
        {
            IStat result;
            _stats.TryGetValue(typeof (T), out result);
            if (result != null)
            {
                result.BaseValue = value;
                Log.DebugFormat("Stat '{0}' has been set to {1}", result.GetType(), value);
            }
        }

        public void SetStat(Type type, float value)
        {
            IStat result;
            _stats.TryGetValue(type, out result);
            if (result != null)
            {
                result.BaseValue = value;
                Log.DebugFormat("Stat '{0}' has been set to {1}", result.GetType(), value);
            }
        }

        public string SerializeStats()
        {
            var serializer = new XmlSerializer(typeof (List<IStat>));
            var writer = new StringWriter();
            serializer.Serialize(writer, Stats.Values.ToList());
            return writer.ToString();
        }

        public void DeserializeStats(string stats)
        {
            var serializer = new XmlSerializer(typeof (List<IStat>));
            var reader = new StringReader(stats);
            IStat result;
            foreach (IStat stat in (List<IStat>) serializer.Deserialize(reader))
            {
                _stats.TryGetValue(stat.GetType(), out result);
                if (result != null)
                {
                    result.BaseValue = stat.BaseValue;
                }
            }
        }

        public float CalcStat(IStat stat)
        {
            return CalcStat(stat, null);
        }

        public float CalcStat(IStat stat, ICharacter target)
        {
            float returnValue = stat.BaseValue;

            Calculator calculator = Calculators[stat];
            var env = new GEnvironment {Character = Character, Target = target, Value = returnValue};

            calculator.Calculate(env);

            if (env.Value <= 0 && stat.IsNonZero)
            {
                return 1;
            }
            return env.Value;
        }

        public static IEnumerable<IStat> CreateStatsList()
        {
            var stats = new List<IStat>();
            stats.Add(new Armor());
            stats.Add(new AttackRange());
            stats.Add(new AttackSpeed());
            stats.Add(new Constitution());
            stats.Add(new Dexterity());
            stats.Add(new Fame());
            stats.Add(new Intelligence());
            stats.Add(new Level());
            stats.Add(new MaxHealth());
            stats.Add(new MovementSpeed());
            stats.Add(new Strength());
            stats.Add(new FrostResistance());
            return stats;
        }
    }
}