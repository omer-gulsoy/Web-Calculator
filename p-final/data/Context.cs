using Calculator.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data
{
	public class Context : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("server=ELITEBOOK\\MSSQLSERVER2012;database=CalculatorDB;integrated security=true;TrustServerCertificate=True;");
		}
		public DbSet<AllModels> AllModels { get; set; }
	}
}
