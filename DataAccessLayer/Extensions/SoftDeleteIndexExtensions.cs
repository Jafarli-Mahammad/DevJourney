using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Extensions
{
    public static class SoftDeleteIndexExtensions
    {
        /// <summary>
        /// SQL Server filter for "row is not soft-deleted". Keep in sync with unique-index tests.
        /// </summary>
        public const string DeletedAtNullFilterSql = "[DeletedAt] IS NULL";

        /// <summary>
        /// Declares a unique index scoped to non-deleted rows only (<c>DeletedAt IS NULL</c>).
        /// </summary>
        public static IndexBuilder IsUniqueWhenNotDeleted(this IndexBuilder builder) =>
            builder.IsUnique().HasFilter(DeletedAtNullFilterSql);

        /// <summary>
        /// Same as <see cref="IsUniqueWhenNotDeleted(IndexBuilder)"/> plus an extra SQL predicate combined with AND
        /// (for example <c>[DocumentSerialNumber] IS NOT NULL</c> on a nullable column).
        /// </summary>
        /// <param name="additionalSqlPredicate">SQL expression without a leading AND.</param>
        public static IndexBuilder IsUniqueWhenNotDeleted(this IndexBuilder builder, string additionalSqlPredicate)
        {
            var extra = additionalSqlPredicate.Trim();
            return builder.IsUnique().HasFilter($"({DeletedAtNullFilterSql}) AND ({extra})");
        }
    }
}
