using System.Collections.Generic;
using GameServer.Model;
using GameServer.Model.Interfaces;

namespace GameServer.Calculators
{
    public class Calculator
    {
        protected List<IFunction> Functions = new List<IFunction>();

        public void AddFunction(IFunction function)
        {
            Functions.Add(function);
            Functions.Sort((x, y) => x.Order.CompareTo(y.Order));
        }

        public void AddFunction(IEnumerable<IFunction> functions)
        {
            foreach (IFunction function in functions)
            {
                Functions.Add(function);
            }
            Functions.Sort((x, y) => x.Order.CompareTo(y.Order));
        }

        public void RemoveFunction(IFunction function)
        {
            Functions.Remove(function);
        }

        public void RemoveFunction(IEnumerable<IFunction> functions)
        {
            foreach (IFunction function in functions)
            {
                Functions.Remove(function);
            }
        }

        public List<IStat> RemoveOwner(GObject owner)
        {
            var modifiedStats = new List<IStat>();

            foreach (IFunction function in Functions)
            {
                if (function.Owner == owner)
                {
                    modifiedStats.Add(function.Stat);
                    RemoveFunction(function);
                }
            }
            return modifiedStats;
        }

        public void Calculate(GEnvironment env)
        {
            foreach (IFunction function in Functions)
            {
                function.Calc(env);
            }
        }
    }
}