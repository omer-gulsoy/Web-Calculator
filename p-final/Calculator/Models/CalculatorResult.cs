using System.ComponentModel.DataAnnotations;

namespace Calculator.Models
{
	public class CalculatorResult
	{
		[Key]
		public int Id { get; set; }
		public double Number_1 { get; set; }
		public double Number_2 { get; set; }
		public string? Operation { get; set; }
		public double? Result { get; set; }
		public DateTime? Date { get; set; }
	}
}
