using System;
using System.Collections.Generic;
using System.Text;

namespace Leo.Chimp.DapperAdapter
{
    public interface ISqlAdapter
    {
        /// <summary>
        /// Builds an SQL query suitable for performing page based queries to the database
        /// </summary>
        /// <param name="partedSql">partedSql</param>
        /// <param name="sqlArgs">Arguments to any embedded parameters in the SQL query</param>
        /// <param name="skip">The number of rows that should be skipped by the query</param>
        /// <param name="take">The number of rows that should be retruend by the query</param>
        string PagingBuild(ref PartedSql partedSql, object sqlArgs, long skip, long take);
    }
}
