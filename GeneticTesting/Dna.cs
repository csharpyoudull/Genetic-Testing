using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticTesting
{
    public class Dna
    {
        private char _value;
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public char Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _binary = Convert.ToString(_value, 2).PadLeft(8, '0');
            }
        }

        private string _binary;
        /// <summary>
        /// Gets or sets the binary.
        /// </summary>
        /// <value>
        /// The binary.
        /// </value>
        public string Binary
        {
            get { return _binary; }
            set 
            {
                _binary = value;
                _value = (char)Convert.ToByte(_binary, 2);
            }
        }
    }
}
