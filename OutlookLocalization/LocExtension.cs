using System;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Reflection;
using System.Windows;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "OutlookLocalization")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2007/xaml/presentation", "OutlookLocalization")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2008/xaml/presentation", "OutlookLocalization")]

namespace OutlookLocalization
{
    [MarkupExtensionReturnType(typeof(object))]
    [ContentProperty("Key")]
    public class LocExtension : MarkupExtension
    {
        public string Key { get; set; }
        public string Format { get; set; }
        object _targetProperty;
        WeakReference _targetObject;
        List<WeakReference> _targetObjects;
        internal bool IsAlive
        {
            get
            {
                if (_targetObjects != null)
                {
                    foreach (var item in _targetObjects)
                    {
                        if (item.IsAlive)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                return _targetObject.IsAlive;
            }
        }

        public LocExtension()
        {

        }

        public LocExtension(string key)
        {
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (service != null)
            {
                if (service.TargetProperty is DependencyProperty)
                {
                    _targetProperty = service.TargetProperty;

                    if (service.TargetObject is DependencyObject)
                    {
                        var targetObject = new WeakReference(service.TargetObject);

                        // Verify if the extension is used in a template
                        // and has been already registered

                        if (_targetObjects != null)
                        {
                            _targetObjects.Add(targetObject);
                        }
                        else
                        {
                            _targetObject = targetObject;

                            LocalizationManager.AddLocalization(this);
                        }
                    }
                    else
                    {
                        // The extension is used in a template

                        _targetObjects = new List<WeakReference>();

                        LocalizationManager.AddLocalization(this);

                        return this;
                    }
                }
                else if (service.TargetProperty is PropertyInfo)
                {
                    _targetProperty = service.TargetProperty;

                    _targetObject = new WeakReference(service.TargetObject);

                    LocalizationManager.AddLocalization(this);
                }
            }

            return GetValue(Key, Format);
        }

        internal void UpdateTargetValue()
        {
            var targetProperty = _targetProperty;

            if (targetProperty != null)
            {
                if (targetProperty is DependencyProperty)
                {
                    if (_targetObject != null)
                    {
                        var targetObject = _targetObject.Target as DependencyObject;

                        if (targetObject != null)
                        {
                            targetObject.SetValue((DependencyProperty)targetProperty, GetValue(Key, Format));
                        }
                    }
                    else if (_targetObjects != null)
                    {
                        foreach (var item in _targetObjects)
                        {
                            var targetObject = item.Target as DependencyObject;

                            if (targetObject != null)
                            {
                                targetObject.SetValue((DependencyProperty)targetProperty, GetValue(Key, Format));
                            }
                        }
                    }
                }
                else if (targetProperty is PropertyInfo)
                {
                    var targetObject = _targetObject.Target;

                    if (targetObject != null)
                    {
                        ((PropertyInfo)targetProperty).SetValue(targetObject, GetValue(Key, Format), null);
                    }
                }
            }
        }

        static object GetValue(string key, string format)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            var manager = LocalizationManager.ResourceManager;

            object value;

#if DEBUG
            //value = manager == null ? string.Empty : manager.GetObject(key) ?? "[Resource: " + key + "]";

            if (manager == null)
            {
                value = "";
            }
            else
            {
                value = manager.GetObject(key);

                if (value == null)
                {
                    throw new ArgumentOutOfRangeException("key", key, "Resource not found.");
                }
            }
#else
            value = manager == null ? string.Empty : manager.GetObject(key) ?? string.Empty;
#endif

            if (string.IsNullOrEmpty(format))
            {
                return value;
            }
            else
            {
                return string.Format(format, value);
            }
        }
    }
}