using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE uspReportSales(@DateStart DATE, @DateEnd DATE)
AS
BEGIN

	SELECT 
		C.Title ConcertName,
		SUM(S.Total) Total
	FROM Sale S (NOLOCK)
	INNER JOIN Concert C (NOLOCK) ON S.ConcertId = C.Id
	AND C.[Status] = 1
	WHERE S.[Status] = 1
	AND CAST(S.SaleDate AS DATE) BETWEEN @DateStart AND @DateEnd
	GROUP BY C.Title
	ORDER BY 2 DESC

END
GO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE uspReportSales");
        }
    }
}
