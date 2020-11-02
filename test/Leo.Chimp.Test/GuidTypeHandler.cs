using System;
using System.Collections.Generic;
using System.Text;

namespace Leo.Chimp.Test
{
    public class GuidTypeHandler : Dapper.SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(System.Data.IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToByteArray();
        }

        public override Guid Parse(object value)
        {
            var inVal = (byte[])value;
            return new Guid(inVal);
        }
    }
}