using System;
using System.Collections.Generic;
using System.Text;

namespace Leo.Chimp.Test
{
    public class GuidTypeHandler : Dapper.SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(System.Data.IDbDataParameter parameter, Guid guid)
        {
            parameter.Value = guid.ToString();
        }

        public override Guid Parse(object value)
        {
            // Dapper may pass a Guid instead of a string
            if (value is Guid)
                return (Guid)value;
            return new Guid((string)value);
        }
    }
}