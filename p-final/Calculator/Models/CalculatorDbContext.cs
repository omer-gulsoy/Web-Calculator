using Microsoft.EntityFrameworkCore;

namespace Calculator.Data
{
	public class CalculatorDbContext : DbContext
	{
		public CalculatorDbContext(DbContextOptions<CalculatorDbContext> options) : base(options) { }

		public DbSet<Calculator.Models.CalculatorResult> CalculatorResults { get; set; }
	}
}
