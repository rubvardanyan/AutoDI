using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AutoDI.Fody
{
    internal class Settings
    {
        internal const Lifetime DefaultLifetime = Lifetime.LazySingleton;

        public Behaviors Behavior { get; set; } = Behaviors.Default;

        /// <summary>
        /// Automatically initialize AutoDI in assembly entry point (if avialible)
        /// </summary>
        public bool AutoInit { get; set; } = true;

        /// <summary>
        /// Generate registration calls no the container. Setting to false will negate AutoInit.
        /// </summary>
        public bool GenerateRegistrations { get; set; } = true;

        public DebugLogLevel DebugLogLevel { get; set; } = DebugLogLevel.Default;

        public IList<MatchType> Types { get; } = new List<MatchType>();

        public IList<Map> Maps { get; } = new List<Map>();

        public IList<MatchAssembly> Assemblies { get; } = new List<MatchAssembly>();

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("AutoDI Settings:");
            sb.AppendLine($"  Behavior(s): {Behavior}");
            sb.AppendLine($"  AutoInit: {AutoInit}");
            sb.AppendLine($"  GenerateRegistrations: {GenerateRegistrations}");
            sb.AppendLine($"  DebugLogLevel: {DebugLogLevel}");
            sb.Append("  Included Assemblies: ");
            if (Assemblies.Any())
            {
                sb.AppendLine();
                 foreach (MatchAssembly assembly in Assemblies)
               {
                    sb.AppendLine($"    {assembly}");
                }
            }
            else
            {
                sb.AppendLine("<none>");
            }

            sb.Append("  Maps: ");
            if (Maps.Any())
            {
                sb.AppendLine();
                foreach (Map map in Maps)
                {
                    sb.AppendLine($"    {map}");
                }
            }
            else
            {
                sb.AppendLine("<none>");
            }

            sb.Append("  Type Lifetimes: ");
            if (Types.Any())
            {
                sb.AppendLine();
                foreach (MatchType type in Types)
                {
                    sb.AppendLine($"    {type}");
                }
            }
            else
            {
                sb.AppendLine("<none>");
            }


            return sb.ToString();
        }

        public static Settings Parse(Settings settings, XElement rootElement)
        {
            if (rootElement == null) return settings;

            string behaviorAttribute = rootElement.GetAttributeValue(nameof(Behavior));
            if (behaviorAttribute != null)
            {
                Behaviors behavior = Behaviors.None;
                foreach (string value in behaviorAttribute.Split(','))
                {
                    if (Enum.TryParse(value, out Behaviors @enum))
                        behavior |= @enum;
                }
                settings.Behavior = behavior;
            }

            if (bool.TryParse(rootElement.GetAttributeValue(nameof(AutoInit)) ?? bool.TrueString,
                out bool autoInit))
            {
                settings.AutoInit = autoInit;
            }

            if (bool.TryParse(rootElement.GetAttributeValue(nameof(GenerateRegistrations)) ?? bool.TrueString,
                out bool generateRegistrations))
            {
                settings.GenerateRegistrations = generateRegistrations;
            }

            if (Enum.TryParse(rootElement.GetAttributeValue(nameof(DebugLogLevel)) ?? nameof(DebugLogLevel.Default),
                out DebugLogLevel debugLogLevel))
            {
                settings.DebugLogLevel = debugLogLevel;
            }

            foreach (XElement assemblyNode in rootElement.DescendantNodes().OfType<XElement>()
                .Where(x => string.Equals(x.Name.LocalName, "Assembly", StringComparison.OrdinalIgnoreCase)))
            {
                string assemblyName = assemblyNode.GetAttributeValue("Name");
                if (string.IsNullOrWhiteSpace(assemblyName)) continue;

                settings.Assemblies.Add(new MatchAssembly(assemblyName));
            }

            foreach (XElement typeNode in rootElement.DescendantNodes().OfType<XElement>()
                .Where(x => string.Equals(x.Name.LocalName, "Type", StringComparison.OrdinalIgnoreCase)))
            {
                string typePattern = typeNode.GetAttributeValue("Name");
                if (string.IsNullOrWhiteSpace(typePattern)) continue;
                string lifetimeStr = typeNode.GetAttributeValue("Lifetime");
                if (lifetimeStr == null || !Enum.TryParse(lifetimeStr, out Lifetime lifetime))
                {
                    lifetime = DefaultLifetime;
                }

                settings.Types.Add(new MatchType(typePattern, lifetime));
            }

            foreach (XElement mapNode in rootElement.DescendantNodes().OfType<XElement>()
                .Where(x => string.Equals(x.Name.LocalName, "Map", StringComparison.OrdinalIgnoreCase)))
            {
                string from = mapNode.GetAttributeValue("From");
                if (string.IsNullOrWhiteSpace(from)) continue;
                string to = mapNode.GetAttributeValue("To");
                if (string.IsNullOrWhiteSpace(to)) continue;
                if (!bool.TryParse(mapNode.GetAttributeValue("Force") ?? bool.FalseString, out bool force))
                {
                    force = false;
                }
                Lifetime? lifetime = null;
                string lifetimeStr = mapNode.GetAttributeValue("Lifetime");
                if (lifetimeStr != null && Enum.TryParse(lifetimeStr, out Lifetime parsedLifetime))
                {
                    lifetime = parsedLifetime;
                }
                
                settings.Maps.Add(new Map(from, to, force, lifetime));
            }

            return settings;
        }

    }
}