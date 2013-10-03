using AutoProxySwitcherLib.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoProxySwitcherLib
{
    public enum Operator
    {
        And,
        Or,
        Not
    }

    /// <summary>
    /// Set of rules
    /// </summary>
    public class NetworkRulesSet : NetworkRule
    {
        private Operator op = Operator.Or;
        List<NetworkRule> subRules = new List<NetworkRule>();

        public NetworkRulesSet()
        {
        }

        public NetworkRulesSet(Operator op)
        {
            this.op = op;
        }

        public List<NetworkRule> Rules
        {
            get { return subRules; }
            set { subRules = value; }
        }

        public Operator Op
        {
            get { return op; }
            set { op = value; }
        }

        public override string ToString()
        {
            if (subRules.Count > 0)
            {
                if (op == Operator.Not)
                {
                    return "(Not " + string.Join<NetworkRule>(", ", subRules) + ")";
                }
                else
                {
                    return "(" + string.Join<NetworkRule>(" " + op.ToString() + " ", subRules) + ")";
                }
            }
            else
            {
                return "none";
            }
        }
    }

}
