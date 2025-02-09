using System;

namespace WatchOnlyBitcoinWallet.MVVM
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DependsOnPropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DependsOnPropertyAttribute"/> using depending properties names.
        /// </summary>
        /// <param name="dependingPropertyNames">Names of the properties that the property with this attribute depends on.</param>
        public DependsOnPropertyAttribute(params string[] dependingPropertyNames)
        {
            DependentProps = dependingPropertyNames;
        }

        /// <summary>
        /// Names of all the properties that the property with this attribute depends on.
        /// </summary>
        public readonly string[] DependentProps;
    }
}
