using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Caliburn.Micro
{
    /// <summary>
    /// A source of assemblies that are inspectable by the framework.
    /// </summary>
    public static class AssemblySource
    {
        /// <summary>
        /// The singleton instance of the AssemblySource used by the framework.
        /// </summary>
        public static readonly IObservableCollection<Assembly> Assemblies = new BindableCollection<Assembly>();

        /// <summary>
        /// Finds a type which matches one of the elements in the sequence of names.
        /// </summary>
        public static Type FindTypeByNames(IEnumerable<string> names)
        {
            return names?
                .Join(Assemblies.SelectMany(a => a.GetExportedTypes()), name => name, exportedType => exportedType.FullName, (name, exportedType) => exportedType)
                .FirstOrDefault();
        }
    }
}
