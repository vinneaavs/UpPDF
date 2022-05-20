using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//ITEXTSHARP REALIZA O MERGE DO DOCUMENTO
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MvcLoadDownTest.Controllers
{
    public class GalleryController : Controller
    {
        //REFERENCIA LOCAL AMBIENTE/PASTA
        IWebHostEnvironment _appEnvironment;
        public GalleryController(IWebHostEnvironment env)
        {
            _appEnvironment = env;
        }

        //private readonly string wwwrootDirectory =
        //    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


        // RECEBE O FILE
        //COPIA PARA WWWROOT
        //CRIA O MERGE E COPIA PARA WWWROOT
        [HttpPost]
        public async Task<IActionResult> UploadToApp(List<IFormFile> file)

        {

            List<string> uploadedFiles = new List<string>();
            List<PdfReader> pdfReaderList = new List<PdfReader>();

            var localeUpload = _appEnvironment.WebRootPath + "\\Original\\";
            var localeUploadMerge = _appEnvironment.WebRootPath + "\\Merge\\";

            if (!Directory.Exists(localeUpload))
            {
                Directory.CreateDirectory(localeUpload);
            }  
            
            if (!Directory.Exists(localeUploadMerge))
            {
                Directory.CreateDirectory(localeUploadMerge);
            }



            foreach (var files in file)
            {

                if (files == null || files.Length == 0)
                { ViewData["ERRO"] = "ERRO: Selecione algum arquivo"; return View(ViewData); }
            }


            foreach (var files in file)
            {
                
                var nameUpload = DateTime.Now.Ticks.ToString() + Path.GetExtension(files.FileName);
                var path = Path.Combine(
                    localeUpload, nameUpload);
            }
                foreach (IFormFile postedFile in file)
            {
            string fileName = Path.GetFileName(postedFile.FileName);
            using (FileStream stream = new FileStream(Path.Combine(localeUpload, fileName), FileMode.Create))
            {
                postedFile.CopyTo(stream);
                uploadedFiles.Add(Path.Combine(localeUpload, fileName));
            }

            }

            foreach (string pdfFile in uploadedFiles)
            {

                PdfReader pdfReader = new PdfReader(Path.Combine(localeUpload, pdfFile));

                pdfReaderList.Add(pdfReader);
            }

            Document document = new Document(PageSize.A4, 0, 0, 0, 0);


            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(Path.Combine(localeUploadMerge, RandomString(8).ToString()+".pdf"), FileMode.Create));
            document.Open();
            foreach (PdfReader reader in pdfReaderList)
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    PdfImportedPage page = writer.GetImportedPage(reader, i);
                    document.Add(Image.GetInstance(page));
                }
            }
            document.Close();


            RedirectToAction("Index");
            { ViewData["OK"] = "OK: Merge realizado"; return View(ViewData); }

        }

        private static Random random = new Random();


        //GERADOR NOME RANDOM
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}

