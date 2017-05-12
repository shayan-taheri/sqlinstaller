//-----------------------------------------------------------------------
// <copyright file="Arguments.cs" company="JHOB Technologies, LLC">
//     Copyright © JHOB Technologies, LLC. All rights reserved.
// </copyright>
// <license>Microsoft Public License</license>
// <author>Brian Schloz</author>
//-----------------------------------------------------------------------
namespace SQLInstaller.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    public sealed class Arguments<T> where T : new()
    {
        private Collection<ValidationResult> validationResults;

        public Arguments(IEnumerable<string> args, T instance)
        {
            this.OriginalArgs = args;
            this.Instance = instance;
            this.validationResults = new Collection<ValidationResult>();

            this.Parse();
        }

        public Arguments(IEnumerable<string> args)
            : this(args, new T())
        {
        }

        public IEnumerable<string> OriginalArgs { get; private set; }

        public T Instance { get; private set; }

        public bool IsValid
        {
            get { return this.validationResults.Count == 0; }
        }

        public string ValidationErrors
        {
            get
            {
                StringBuilder error = new StringBuilder();
                foreach (ValidationResult result in this.validationResults)
                {
                    error.Append(result.ErrorMessage);
                    error.Append(" ");
                }

                return error.ToString();
            }
        }

        private void Parse()
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this.Instance);

            foreach (string arg in this.OriginalArgs)
            {
                if (arg.Length <= 1 || !arg.StartsWith("/", StringComparison.InvariantCulture))
                {
                    continue;
                }

                string key;
                string value = string.Empty;

                int equalIdx = arg.IndexOf('=');
                if (equalIdx > 0)
                {
                    key = arg.Substring(1, equalIdx - 1);
                    value = arg.Substring(equalIdx + 1);
                }
                else
                {
                    key = arg.Substring(1);
                }

                PropertyDescriptor property = this.FindProperty(key, properties);

                if (property != null)
                {
                    if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                    {
                        property.SetValue(this.Instance, true);
                    }
                    else
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(property.PropertyType);
                        if (typeConverter != null && typeConverter.CanConvertFrom(typeof(string)))
                        {
                            property.SetValue(this.Instance, typeConverter.ConvertFromInvariantString(value));
                        }
                    }
                }
            }

            Validator.TryValidateObject(this.Instance, new ValidationContext(this.Instance, null, null), this.validationResults, true);
        }

        /// <summary>
        /// Find an unambiguous matching property for the given argument.
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        private PropertyDescriptor FindProperty(string keyName, PropertyDescriptorCollection properties)
        {
            List<PropertyDescriptor> matches = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor prop in properties)
            {
                // Ignore property hacks for xml serialization.
                if (prop.Name.EndsWith("Specified", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // An exact match on the argument/property will always succeed. Otherwise, do a partial match.
                // E.g. Two arguments - Foo and FooBar. /Foo will succeed /FooB will succeed. /Fo or /F will fail (ambiguous).
                if (string.Compare(prop.Name, keyName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    matches.Clear();
                    matches.Add(prop);
                    break;
                }
                else if (keyName.Length == 1)
                {
                    if (prop.Name.StartsWith(keyName, StringComparison.OrdinalIgnoreCase))
                    {
                        matches.Add(prop);
                    }
                }
                else if (prop.Name.IndexOf(keyName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    matches.Add(prop);
                }
            }

            if (matches.Count == 1)
            {
                return matches[0];
            }
            else
            {
                return null;
            }
        }
    }
}
