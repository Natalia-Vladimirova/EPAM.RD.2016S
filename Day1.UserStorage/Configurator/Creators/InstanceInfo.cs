using System;

namespace Configurator.Creators
{
    /// <summary>
    /// Used to contain information about type name and list of parameters.
    /// </summary>
    [Serializable]
    public class InstanceInfo
    {
        /// <summary>
        /// Creates an instance of InstanceInfo with specified type name and parameters.
        /// </summary>
        /// <param name="typeName">
        /// Full name of type including assembly name.
        /// </param>
        /// <param name="parameters">
        /// Parameters that constructor needs to create object.
        /// </param>
        public InstanceInfo(string typeName, params object[] parameters)
        {
            TypeName = typeName;
            Parameters = parameters;
        }

        /// <summary>
        /// Full name of type including assembly name.
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Parameters for constructor.
        /// </summary>
        public object[] Parameters { get; }
    }
}
