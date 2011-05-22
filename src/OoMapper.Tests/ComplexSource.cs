using System;

namespace OoMapper.Tests
{
    internal class ComplexSource
    {
        public ComplexSourceChild Some { get; set; }

        #region Nested type: ComplexSourceChild

        internal class ComplexSourceChild
        {
            public string Property { get; set; }
        }

        #endregion
    }
}