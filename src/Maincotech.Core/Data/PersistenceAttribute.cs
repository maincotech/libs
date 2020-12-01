using System;

namespace Maincotech.Data
{
    public class PersistenceAttribute : Attribute
    {
        public PersistenceAttribute()
        {
        }

        public PersistenceAttribute(bool igore)
            : this(igore, null)
        {
        }

        public PersistenceAttribute(bool igore, string sourceColumn)
        {
            Igore = igore;
            SourceColumn = sourceColumn;
        }

        public bool Igore { get; set; }

        public string SourceColumn { get; set; }
    }
}