using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace web.Controllers
{
	public class FileController : Controller
	{

		[HttpPost]
		public async Task<IActionResult> UploadFile(IFormFile file)
		{
			if (file != null && file.Length > 0)
			{
				using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
				{
					string fileContent = await reader.ReadToEndAsync();
					var processedContent = ProcessFileContent(fileContent);

					// İşlenen veriyi AJAX ile döndür
					return Json(processedContent);
				}
			}
			return Json("File not loaded.");
		}

		private string ProcessFileContent(string content)
		{
			var lines = content.Split('\n');
			StringBuilder result = new StringBuilder();

			foreach (var line in lines)
			{
				var parts = line.Trim().Split(' ');

				// 3 karakter: Sayı, Operatör, Sayı
				if (parts.Length == 3)
				{
					string num1 = parts[0];
					string operatorSymbol = parts[1];
					string num2 = parts[2];

					// Sayıları ve operatörü işleyelim
					result.AppendLine($"İşlem: {num1} {operatorSymbol} {num2}");

					// Örneğin işlem yapabiliriz
					double number1 = double.Parse(num1);
					double number2 = double.Parse(num2);
					double calculationResult = 0;

					switch (operatorSymbol)
					{
						case "+":
							calculationResult = number1 + number2;
							break;
						case "-":
							calculationResult = number1 - number2;
							break;
						case "*":
							calculationResult = number1 * number2;
							break;
						case "/":
							calculationResult = number1 / number2;
							break;
						default:
							calculationResult = 0;
							break;
					}

					result.AppendLine($"Result: {calculationResult}");
				}
			}

			return result.ToString();
		}
	}
}
