﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.33440.
// 
namespace OPPF.XML
{
    using System.Xml.Serialization;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.oppf.ox.ac.uk/xsd/RI/PlateTypes.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.oppf.ox.ac.uk/xsd/RI/PlateTypes.xsd", IsNullable = false)]
    public partial class PlateType
    {

        private string idField;

        private string nameField;

        private int numRowsField;

        private int numColumnsField;

        private int numDropsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int NumRows
        {
            get
            {
                return this.numRowsField;
            }
            set
            {
                this.numRowsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int NumColumns
        {
            get
            {
                return this.numColumnsField;
            }
            set
            {
                this.numColumnsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int NumDrops
        {
            get
            {
                return this.numDropsField;
            }
            set
            {
                this.numDropsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.oppf.ox.ac.uk/xsd/RI/PlateTypes.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.oppf.ox.ac.uk/xsd/RI/PlateTypes.xsd", IsNullable = false)]
    public partial class PlateTypes
    {

        private PlateType[] plateTypeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PlateType")]
        public PlateType[] PlateType
        {
            get
            {
                return this.plateTypeField;
            }
            set
            {
                this.plateTypeField = value;
            }
        }
    }
}
