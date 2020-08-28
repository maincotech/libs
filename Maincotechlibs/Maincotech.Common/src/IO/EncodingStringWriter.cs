using System;
using System.IO;
using System.Text;

namespace Maincotech.IO
{
    /// <summary>
    /// 继承StringWriter，并且重写Encoding，使StringWriter可以扩展Encoding方法。
    /// </summary>
    public class EncodingStringWriter : StringWriter
    {
        private Encoding _encoding;

        /// <summary>
        /// StringWriter的Encoding方法
        /// </summary>
        public override Encoding Encoding
        {
            get
            {
                return _encoding;
            }
        }

        /// <summary>
        /// 构造函数，可以指定Encoding
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="sb"></param>
        /// <param name="formatProvider"></param>
        public EncodingStringWriter(Encoding encoding, StringBuilder sb, IFormatProvider formatProvider)
            : base(sb, formatProvider)
        {
            this._encoding = encoding;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="formatProvider"></param>
        public EncodingStringWriter(StringBuilder sb, IFormatProvider formatProvider)
            : base(sb, formatProvider)
        {
            _encoding = Encoding.UTF8;
        }

        /// <summary>
        /// 由于不能Overwrite Set方法，所以提供一个Method来更改Encoding方法
        /// </summary>
        /// <param name="encoding"></param>
        public void SetEncoding(Encoding encoding)
        {
            this._encoding = encoding;
        }
    }
}