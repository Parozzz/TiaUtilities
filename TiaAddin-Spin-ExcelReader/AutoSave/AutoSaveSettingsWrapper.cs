using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Utility;

namespace TiaXmlReader.AutoSave
{
    class AutoSaveSettingsWrapper
    {
        private readonly ISettingsAutoSave obj;
        private readonly Dictionary<IAutoSaveWrapperReflectionHelper, object> snapshotDict;
        private readonly List<AutoSaveSettingsWrapper> subWrapperList;

        public AutoSaveSettingsWrapper(ISettingsAutoSave @object) 
        {
            this.obj = @object;

            this.snapshotDict = new Dictionary<IAutoSaveWrapperReflectionHelper, object>();
            this.subWrapperList = new List<AutoSaveSettingsWrapper>();
        }

        public void Scan()
        {
            this.snapshotDict.Clear();
            this.subWrapperList.Clear();

            var settingsAutoSaveType = typeof(ISettingsAutoSave);

            foreach(var helper in GetAllPublicValues())
            {
                var type = helper.GetReflectedType();
                if(type.GetInterfaces().Contains(settingsAutoSaveType))
                {
                    var value = helper.GetReflectedValue(obj);
                    if (value is ISettingsAutoSave subObject)
                    {
                        var subWrapper = new AutoSaveSettingsWrapper(subObject);
                        subWrapper.Scan();
                        subWrapperList.Add(subWrapper);
                    }
                }
                else
                {
                    var value = helper.GetReflectedValue(this.obj);
                    snapshotDict.Add(helper, value); //Fill with starting values!
                }
            }
        }

        public bool CompareSnapshot() //TRUE IF ALL EQUALS
        {
            var snapshotChanged = false;
            foreach(var entry in snapshotDict)
            {
                var helper = entry.Key;
                var snapshotValue = entry.Value;
                var actualValue = helper.GetReflectedValue(this.obj);
                if (Utils.AreValuesDifferent(snapshotValue, actualValue))
                {
                    snapshotChanged = true;
                    break;
                }
            }

            if(snapshotChanged)
            {
                UpdateSnapshot(); //This way because this iterates the dict and would cause problems.
                return false;
            }

            //Since these settings are always updated one at the time (Almost never you gonna change two at the same time) i will give priority to the first one found
            //that has different values ignoring the other after (returning false immediately)
            foreach (var subWrapper in subWrapperList)
            {
                if(!subWrapper.CompareSnapshot())
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdateSnapshot()
        {
            foreach (var helper in snapshotDict.Keys.ToList()) //ToList to allow changing the dict while iterating!
            {
                snapshotDict[helper] = helper.GetReflectedValue(this.obj);
            }
        }

        private List<IAutoSaveWrapperReflectionHelper> GetAllPublicValues()
        {
            var reflectionGetterValue = new List<IAutoSaveWrapperReflectionHelper>();

            var type = obj.GetType();
            foreach (var field in type.GetFields().Where(f => f.IsPublic && !f.IsStatic))
            {
                reflectionGetterValue.Add(new FieldReflectionHelper(field));
            }

            foreach (var property in type.GetProperties().Where(p => p.CanRead && p.GetMethod.IsPublic))
            {
                reflectionGetterValue.Add(new PropertyReflectionHelper(property));
            }

            return reflectionGetterValue;
        }
    }


    interface IAutoSaveWrapperReflectionHelper
    {
        string GetReflectedName();
        Type GetReflectedType();
        object GetReflectedValue(object obj);
    }

    class FieldReflectionHelper : IAutoSaveWrapperReflectionHelper
    {
        private readonly FieldInfo fieldInfo;
        public FieldReflectionHelper(FieldInfo fieldInfo) 
        {
            this.fieldInfo = fieldInfo;
        }

        public string GetReflectedName()
        {
            return "FIELD_" + fieldInfo.Name;
        }

        public Type GetReflectedType()
        {
            return fieldInfo.FieldType;
        }

        public object GetReflectedValue(object obj)
        {
            return fieldInfo.GetValue(obj);
        }
    }

    class PropertyReflectionHelper : IAutoSaveWrapperReflectionHelper
    {
        private readonly PropertyInfo propertyInfo;
        public PropertyReflectionHelper(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
        }

        public string GetReflectedName()
        {
            return "PROPERTY_" + propertyInfo.Name;
        }

        public Type GetReflectedType()
        {
            return propertyInfo.PropertyType;
        }

        public object GetReflectedValue(object obj)
        {
            return propertyInfo.GetValue(obj);
        }
    }
}
