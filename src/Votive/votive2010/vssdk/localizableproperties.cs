//-------------------------------------------------------------------------------------------------
// <copyright file="localizableproperties.cs" company="Outercurve Foundation">
//   Copyright (c) 2004, Outercurve Foundation.
//   This software is released under Microsoft Reciprocal License (MS-RL).
//   The license and further copyright text can be found in the file
//   LICENSE.TXT at the root directory of the distribution.
// </copyright>
//-------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Designer.Interfaces;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Package
{
    /// <summary>
    /// Enables a managed object to expose properties and attributes for COM objects.
    /// </summary>
    [ComVisible(true)]
    public class LocalizableProperties : ICustomTypeDescriptor
    {
        #region ICustomTypeDescriptor
        public virtual AttributeCollection GetAttributes()
        {
            AttributeCollection col = TypeDescriptor.GetAttributes(this, true);
            return col;
        }

        public virtual EventDescriptor GetDefaultEvent()
        {
            EventDescriptor ed = TypeDescriptor.GetDefaultEvent(this, true);
            return ed;
        }

        public virtual PropertyDescriptor GetDefaultProperty()
        {
            PropertyDescriptor pd = TypeDescriptor.GetDefaultProperty(this, true);
            return pd;
        }

        public virtual object GetEditor(Type editorBaseType)
        {
            object o = TypeDescriptor.GetEditor(this, editorBaseType, true);
            return o;
        }

        public virtual EventDescriptorCollection GetEvents()
        {
            EventDescriptorCollection edc = TypeDescriptor.GetEvents(this, true);
            return edc;
        }

        public virtual EventDescriptorCollection GetEvents(System.Attribute[] attributes)
        {
            EventDescriptorCollection edc = TypeDescriptor.GetEvents(this, attributes, true);
            return edc;
        }

        public virtual object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public virtual PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection pcol = GetProperties(null);
            return pcol;
        }

        public virtual PropertyDescriptorCollection GetProperties(System.Attribute[] attributes)
        {
            ArrayList newList = new ArrayList();
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this, attributes, true);

            for (int i = 0; i < props.Count; i++)
                newList.Add(CreateDesignPropertyDescriptor(props[i]));

            return new PropertyDescriptorCollection((PropertyDescriptor[])newList.ToArray(typeof(PropertyDescriptor)));
        }

        public virtual Microsoft.VisualStudio.Package.DesignPropertyDescriptor CreateDesignPropertyDescriptor(PropertyDescriptor propertyDescriptor)
        {
            return new Microsoft.VisualStudio.Package.DesignPropertyDescriptor(propertyDescriptor);
        }

        public virtual string GetComponentName()
        {
            string name = TypeDescriptor.GetComponentName(this, true);
            return name;
        }

        public virtual TypeConverter GetConverter()
        {
            TypeConverter tc = TypeDescriptor.GetConverter(this, true);
            return tc;
        }

        public virtual string GetClassName()
        {
            return this.GetType().FullName;
        }

        #endregion ICustomTypeDescriptor
    }
}
