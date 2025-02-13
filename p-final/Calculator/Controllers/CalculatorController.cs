using System.Linq;
using Calculator.Data;
using Calculator.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.Controllers
{
	public class CalculatorController : Controller
	{
		//MODEL CONNECTIONS
		private readonly CalculatorDbContext _context;
		public CalculatorController(CalculatorDbContext context)
		{
			_context = context;
		}

		//VIEW
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		//CALCULATE FUNCTIONS
		[HttpPost]
		public IActionResult Index(CalculatorResult model, string button)
		{
			switch (button)
			{
				//calculate
				case "Hesapla":
					model.Date = DateTime.Now;
					switch (model.Operation)
					{
						//collection - plus
						case "+":
							model.Result = model.Number_1 + model.Number_2;
							break;
						//extraction - minus
						case "-":
							model.Result = model.Number_1 - model.Number_2;
							break;
						//impact - x
						case "*":
							model.Result = model.Number_1 * model.Number_2;
							break;
						//divide - slash
						case "/":
							if (model.Number_2 != 0)
								model.Result = model.Number_1 / model.Number_2;
							else
								ModelState.AddModelError("", "İkinci Sayı 0 Olamaz!!");
							break;
					}
					//saving result
					if (ModelState.IsValid)
					{
						var newEntry = new CalculatorResult
						{
							Number_1 = model.Number_1,
							Number_2 = model.Number_2,
							Operation = model.Operation,
							Result = model.Result,
							Date = model.Date
						};
						_context.CalculatorResults.Add(newEntry);
						_context.SaveChanges();
					}
					break;
			}
			return View(model);
		}

		//MEMORY FUNCTIONS
		public static double yrk = 0;
		//create
		[HttpPost]
		public IActionResult AddToMemory(double? result)
		{
			if (result.HasValue)
			{
				yrk = result.Value;
				return Json(new { success = true, message = "Değer başarıyla kaydedildi: " + yrk });
			}
			return Json(new { success = false, message = "Sonuç değeri geçerli değil." });
		}
		//read
		[HttpPost]
		public IActionResult GetYrkValue()
		{
			return Json(new { yrk = yrk });
		}
		//delete
		[HttpPost]
		public IActionResult ClearMemory()
		{
			yrk = 0; // `yrk` değerini sıfırlıyoruz
			return Json(new { success = true, message = "Memory sıfırlandı." });
		}

		//RESULT LIST
		[HttpGet]
		public IActionResult GetResult()
		{
			// Son 10 veriyi almak için Take(10) kullanıyoruz
			var results = _context.CalculatorResults
								   .OrderByDescending(r => r.Date) // Sonuçları tarihe göre azalan sırada sıralıyoruz
								   .Take(5)  // Sadece son 10 kaydı alıyoruz
								   .ToList();
			return Json(results);
		}

		//FILE FUNCTIONS
		//upload file
		[HttpPost]
		public IActionResult UploadFile(IFormFile file)
		{
			if (file != null && file.Length > 0)
			{
				using (var reader = new StreamReader(file.OpenReadStream()))
				{
					var content = reader.ReadToEnd();
					// İçeriği AJAX'a gönderecek şekilde döndür
					return Ok(content);
				}
			}
			return BadRequest("File could not be loaded.");
		}
		//process file
		[HttpPost]
		public IActionResult ProcessFile(IFormFile file)
		{
			if (file != null && file.Length > 0)
			{
				var results = new List<string>();

				using (var reader = new StreamReader(file.OpenReadStream()))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						var parts = line.Split(';');

						if (parts.Length == 3)
						{
							try
							{
								double num1 = Convert.ToDouble(parts[0]);
								string operatorSymbol = parts[1];
								double num2 = Convert.ToDouble(parts[2]);

								double result = 0;

								switch (operatorSymbol)
								{
									case "+":
										result = num1 + num2;
										break;
									case "-":
										result = num1 - num2;
										break;
									case "*":
										result = num1 * num2;
										break;
									case "/":
										if (num2 != 0)
											result = num1 / num2;
										else
											results.Add("Hata: Sıfıra bölme!");
										break;
									default:
										results.Add("Geçersiz operatör!");
										continue;
								}

								results.Add(result.ToString());
							}
							catch (Exception ex)
							{
								results.Add("Hata: " + ex.Message);
							}
						}
						else
						{
							results.Add("Hata: Geçersiz format!");
						}
					}
				}

				// Sonuçları JSON olarak döndür
				return Json(new { success = true, results });
			}

			return Json(new { success = false, message = "Dosya alınamadı!" });
		}

		// İndirilebilir bir dosya olarak sonucu sağlayan yeni bir Action
		[HttpPost]
		public IActionResult DownloadResults([FromBody] List<string> results)
		{
			var tempFilePath = Path.Combine(Path.GetTempPath(), "CalculationResults.txt");
			System.IO.File.WriteAllLines(tempFilePath, results, Encoding.UTF8);

			byte[] fileBytes = System.IO.File.ReadAllBytes(tempFilePath);
			return File(fileBytes, "text/plain", "CalculationResults.txt");
		}
	}
}
