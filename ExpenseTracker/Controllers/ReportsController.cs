using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using System.IO;
using Syncfusion.Pdf.Grid;
using ExpenseTracker.Models;

namespace ExpenseTracker.Controllers
{
    public class ReportsController : Controller
    {

        private readonly ApplicationDbContext _context;
        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateDocument()
        {
            //Generate a new PDF document.
            PdfDocument doc = new();
            //Add a page.
            PdfPage page = doc.Pages.Add();
            //Create a PdfGrid.
            PdfGrid pdfGrid = new();
            //Add values to list
            List<object> data = new();

            _context.Categories.OrderByDescending(l => l.Title).ToList().ForEach(x => data.Add(new 
            {
                Title = x.Title,
                Total = x.Type=="Expense" ? "-" + _context.Transactions
                                            .Where(t => t.CategoryId == x.CategoryId).Sum(t => t.Amount).ToString()
                                            : "+" + _context.Transactions.Where(t => t.CategoryId == x.CategoryId).Sum(t => t.Amount).ToString(),
                Type = x.Type
            }));

            //Add list to IEnumerable
            IEnumerable<object> dataTable = data;
            //Assign data source.
            pdfGrid.DataSource = dataTable;
            //Draw grid to the page of PDF document.
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(10, 10));
            //Write the PDF document to stream
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            //If the position is not set to '0' then the PDF will be empty.
            stream.Position = 0;
            //Close the document.
            doc.Close(true);
            //Defining the ContentType for pdf file.
            string contentType = "application/pdf";
            //Define the file name.
            string fileName = "Output.pdf";
            //Creates a FileContentResult object by using the file contents, content type, and file name.
            return File(stream, contentType, fileName);
        }
    }
}
