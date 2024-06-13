using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZintNet
{
    /// <summary>
    /// Type class for holding the symbol names and their enumerated values.
    /// </summary>
    internal sealed class BarcodeNames
    {
        private Symbology id;
        private string name;

        public BarcodeNames(Symbology id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public string SymbolName
        {
            get { return name; }
        }

        public Symbology SymbolID
        {
            get { return id; }
        }
    }
}
