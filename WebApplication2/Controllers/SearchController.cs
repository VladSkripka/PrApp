using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication2.Data;
using WebApplication2.Models;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
using ceTe.DynamicPDF.PageElements.BarCoding;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mime;
using NuGet.Packaging;
using WebApplication2.Infrastructure.Interfaces;
using System.IO;

namespace WebApplication2.Controllers
{
    public class SearchController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISendGridApiService _sendGridApiService;
        private const double EarthRadiusKm = 6371;

        public SearchController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ISendGridApiService sendGridApiService)
        {
            _context = context;
            _userManager = userManager;
            _sendGridApiService = sendGridApiService;
        }


        // GET: SearchController
        public ActionResult Index()
        {
            //SendEmail();
            return View();
        }

        public ActionResult Search(Search? model, string? lat, string? lon, string? nearestModel)
        {
            model.searchTime = DateTime.Now;

            /*lat = "50,438607"; // test data for geo
            *lon = "30,486810";
            */

            List<VacantDeparture> results;
            if (nearestModel == null)
            {
                results = _context.VacantDepartures
                .Where(departure =>
                    departure.departureP.Contains(model.departurePoint) &&
                    departure.arrivalP.Contains(model.destinationPoint) &&
                    departure.departureD > model.searchTime.AddMinutes(5) &&
                    departure.departureD < model.departureDate.AddDays(7))
                .OrderBy(departure => departure.departureD)
                .ToList();
            }
            else
            {
                var newModel = JsonConvert.DeserializeObject<Search>(nearestModel);
                results = _context.VacantDepartures
               .Where(departure =>
                   departure.departureP.Contains(newModel.departurePoint) &&
                   departure.arrivalP.Contains(newModel.destinationPoint) &&
                   departure.departureD > newModel.searchTime.AddMinutes(10) &&
                   departure.departureD > newModel.departureDate)
               .OrderBy(departure => departure.departureD)
               .ToList();
            }

            var filteredResults = new List<VacantDeparture>();

            foreach (var result in results)
            {
                var SoldTicketCount = _context.TicketModels.Count(ticket => ticket.vacantDepatureID == result.vdID && ticket.status == "sold");
                if (result.seatsCount - SoldTicketCount > 0)
                {
                    string[] coords = result.departureCoord.Split(' ');

                    if (result.departureD > model.searchTime.AddMinutes(10))
                    {
                        filteredResults.Add(result);
                    }
                    else if (result.departureD < model.searchTime.AddMinutes(10) && CalculateDistance(lat, lon, coords[0], coords[1]) < 100)
                    {
                        filteredResults.Add(result);
                    }

                }
            }

            TempData["SearchModel"] = JsonConvert.SerializeObject(model);
            ViewBag.SearchModel = model;
            return View(filteredResults);
        }

        public ActionResult SecondTicketSearch(string selectedCarriage, string selectedSeat, int depID, DateTime returnDate)
        {
            var depModel = _context.VacantDepartures.FirstOrDefault(dearture => dearture.vdID == depID);

            Search searchModel = new Search();
            searchModel.searchTime = DateTime.Now;
            searchModel.departurePoint = depModel.arrivalP;
            searchModel.destinationPoint = depModel.departureP;
            searchModel.departureDate = returnDate;

            var results = _context.VacantDepartures
            .Where(departure =>
                departure.departureP == searchModel.departurePoint &&
                departure.arrivalP == searchModel.destinationPoint &&
                departure.departureD > searchModel.searchTime.AddMinutes(10) &&
                departure.departureD > searchModel.departureDate)
            .OrderBy(departure => departure.departureD)
            .ToList();

            var filteredResults = new List<VacantDeparture>();

            foreach (var result in results)
            {
                var SoldTicketCount = _context.TicketModels.Count(ticket => ticket.vacantDepatureID == result.vdID && ticket.status == "sold");
                if (result.seatsCount - SoldTicketCount > 0)
                {
                    filteredResults.Add(result);
                }
            }

            ViewBag.MainVacDep = depID;
            ViewBag.MainChosenSeats = selectedSeat;
            ViewBag.MainChosenCars = selectedCarriage;

            return View(filteredResults);
        }

        public async Task<IActionResult> Ordering(int? id)
        {
            if (id == null || _context.VacantDepartures == null)
            {
                return NotFound();
            }

            var model = JsonConvert.DeserializeObject<Search>(TempData["SearchModel"].ToString());

            ViewBag.returnDate = model.returnDate;

            var vacantDeparture = await _context.VacantDepartures
                .Include(vd => vd.Train) // Include the Train navigation property
                .Include(vd => vd.provider) // Include the provider navigation property
                .FirstOrDefaultAsync(m => m.vdID == id);

            if (vacantDeparture == null)
            {
                return NotFound();
            }

            vacantDeparture.Train = _context.Trains.FirstOrDefault(train => train.trainTypeID == vacantDeparture.Train.trainTypeID);

            ViewBag.SeatsCount = vacantDeparture.Train.trainSeats;
            ViewBag.CarriageCount = vacantDeparture.carriageCount;
            if (vacantDeparture.Train.trainRows != 0)
            {
                ViewBag.Rows = vacantDeparture.Train.trainRows;
            }
            else
            {
                ViewBag.Rows = 2;
            }

            List<List<int>> occupiedSeats = new List<List<int>>();
            for (int i = 1; i <= vacantDeparture.carriageCount; i++)
            {
                occupiedSeats.Add(GetVacantSeats(vacantDeparture, i));
            }
            ViewBag.Seats = occupiedSeats;

            return View(vacantDeparture);
        }

        public async Task<IActionResult> OrderingSecondTicket(int? returnId, int mainVacDepId, string mainChosenSeats, string mainChosenCars)
        {
            if (returnId == null || _context.VacantDepartures == null)
            {
                return NotFound();
            }

            var vacantDeparture = await _context.VacantDepartures
                .Include(vd => vd.Train) // Include the Train navigation property
                .Include(vd => vd.provider) // Include the provider navigation property
                .FirstOrDefaultAsync(m => m.vdID == returnId);

            if (vacantDeparture == null)
            {
                return NotFound();
            }

            vacantDeparture.Train = _context.Trains.FirstOrDefault(train => train.trainTypeID == vacantDeparture.Train.trainTypeID);

            ViewBag.SeatsCount = vacantDeparture.Train.trainSeats;
            ViewBag.CarriageCount = vacantDeparture.carriageCount;

            if (vacantDeparture.Train.trainRows != 0)
            {
                ViewBag.Rows = vacantDeparture.Train.trainRows;
            }
            else
            {
                ViewBag.Rows = 2;
            }

            ViewBag.mainVacDepId = mainVacDepId;
            ViewBag.mainChosenSeats = mainChosenSeats;
            ViewBag.mainChosenCars = mainChosenCars;

            List<List<int>> occupiedSeats = new List<List<int>>();
            for (int i = 1; i <= vacantDeparture.carriageCount; i++)
            {
                occupiedSeats.Add(GetVacantSeats(vacantDeparture, i));
            }
            ViewBag.Seats = occupiedSeats;

            return View(vacantDeparture);

        }

        public ActionResult SummingUp(int id, string SelectedCarriage, string SelectedSeat, int? mainVacDepId, string? mainChosenSeats, string? mainChosenCars)
        {
            var returnDep = _context.VacantDepartures.FirstOrDefault(departure => departure.vdID == id);


            if (returnDep == null)
            {
                return NotFound();
            }

            var uid = _userManager.GetUserId(User);
            var currentUser = _context.Users.FirstOrDefault(u => u.Id == uid);
            if (currentUser == null)
            {
                ViewBag.isPhoneEmpty = false;
            }
            else
            {
                ViewBag.isPhoneEmpty = string.IsNullOrEmpty(currentUser.PhoneNumber);
            }

            List<TicketModel> tickets = new List<TicketModel>();

            var returnSepSeats = SelectedSeat.Split(',');
            var returnSepCars = SelectedCarriage.Split(',');
            double sum = 0;

            if (mainVacDepId != null)
            {
                var mainVacDep = _context.VacantDepartures.FirstOrDefault(departure => departure.vdID == mainVacDepId);

                if (mainVacDep == null)
                {
                    return NotFound();
                }

                var mainSepSeats = mainChosenSeats.Split(',');
                var mainSepCars = mainChosenCars.Split(',');
                for (int i = 0; i < mainSepSeats.Length; i++)
                {
                    sum += mainVacDep.price;
                    tickets.Add(TicketConstructor(mainVacDep, mainSepSeats[i], mainSepCars[i]));
                }
            }

            for (int i = 0; i < returnSepSeats.Length; i++)
            {
                sum += returnDep.price;
                tickets.Add(TicketConstructor(returnDep, returnSepSeats[i], returnSepCars[i]));

            }

            ViewBag.tickets = tickets;
            ViewBag.TotalPrice = sum;
            return View(tickets);
        }

        public async Task<IActionResult> DownloadTicket(string tickets, string passengers, string totalSum, string? email, string? phone, string ticketsType)
        {
            var ticketList = JsonConvert.DeserializeObject<List<TicketModel>>(tickets);
            var currentUser = new IdentityUser();
            var ticketsPaths = new List<string>(ticketList!.Capacity);
            string[] passangersWithAdditionalOptions = passengers.Split(",");
            string[] passanger = new string[passangersWithAdditionalOptions.Length];
            string[] additionalOptions = new string[passangersWithAdditionalOptions.Length];

            string[] ticketType = ticketsType.Split(",");

            int index = 0;
            foreach (var item in passangersWithAdditionalOptions)
            {
                passanger[index] = item.Split(":")[0];
                additionalOptions[index] = item.Split(":")[1];
                index++;
            }

            if (User.Identity.IsAuthenticated)
            {
                var id = _userManager.GetUserId(User);
                currentUser = _context.Users.FirstOrDefault(user => user.Id == id);
            }
            else
            {
                currentUser.Id = Guid.NewGuid().ToString();
                currentUser.Email = email;
                currentUser.PhoneNumber = phone;
            }

            index = 0;

            foreach (var item in ticketList)
            {
                item.user = currentUser;
                item.status = "sold";
                item.passengerFullName = passanger[index];
                item.PurchaseDate = DateTime.Now;
                item.addtionalOptions = additionalOptions[index];
                item.ticketAgeType = ticketType[index];
                ticketsPaths.Add(GeneratePDFTicket(item));
                index++;
                if (ModelState.IsValid)
                {
                    _context.Add(item);
                    await _context.SaveChangesAsync();
                }
            }
            SendEmail(ticketsPaths, currentUser);
            return View(ticketList);
        }

        public IActionResult Download(string tickets)
        {
            var ticketList = JsonConvert.DeserializeObject<List<TicketModel>>(tickets);
            var fileResults = new List<FileContentResult>();

            // Create FileContentResult for each file
            foreach (var ticket in ticketList)
            {
                // Get the file path for the PDF
                string projectDirectory = Directory.GetCurrentDirectory();
                string relativePath = @"wwwroot/tickets";

                string fullPath = System.IO.Path.Combine(projectDirectory, relativePath);

                string filePath = System.IO.Path.Combine(fullPath, $"{ticket.ticketPID}.pdf");

                // Check if the file exists
                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    var fileResult = new FileContentResult(fileBytes, MediaTypeNames.Application.Pdf)
                    {
                        FileDownloadName = $"{ticket.ticketPID}.pdf"
                    };
                    fileResults.Add(fileResult);
                }
            }

            // Return the first file directly and others as attachment
            var response = new FileContentResult(fileResults[0].FileContents, fileResults[0].ContentType)
            {
                FileDownloadName = fileResults[0].FileDownloadName
            };

            // If there are multiple files, add others as attachments
            if (fileResults.Count > 1)
            {
                for (int i = 1; i < fileResults.Count; i++)
                {
                    response.FileDownloadName = fileResults[i].FileDownloadName;
                }
            }

            return response;
        }

        //Support methods
        public List<int> GetVacantSeats(VacantDeparture model, int index)
        {
            List<int> seats = new List<int>();
            var ticketsPerOneCarriage = _context.TicketModels
                .Where(
                        ticket => ticket.vacantDepatureID == model.vdID &&
                        ticket.carriageNumber == index &&
                        ticket.status == "sold")
                .ToList();
            foreach (var item in ticketsPerOneCarriage)
            {
                seats.Add(item.seat);
            }
            return seats;
        }
        public TicketModel TicketConstructor(VacantDeparture vD, string seat, string carriage)
        {
            TicketModel ticket = new TicketModel();
            ticket.seat = Convert.ToInt32(seat);
            ticket.carriageNumber = Convert.ToInt32(carriage);
            ticket.vacantDepatureID = vD.vdID;
            ticket.ticketPID = (vD.PID + ticket.seat + ticket.carriageNumber) * ticket.seat;
            ticket.arrivalP = vD.arrivalP;
            ticket.departureP = vD.departureP;
            ticket.arrivalD = vD.arrivalD;
            ticket.departureD = vD.departureD;

            return ticket;
        }
        public static double CalculateDistance(string lat1, string lon1, string lat2, string lon2)
        {
            // Convert latitude and longitude from degrees to radians
            try
            {
                double lat = Convert.ToDouble(lat1.Replace(".", ","));
                double lon = Convert.ToDouble(lon1.Replace(".", ","));
                double lat_d = Convert.ToDouble(lat2);
                double lon_d = Convert.ToDouble(lon2);
                Console.WriteLine(lat + " " + lon + " " + lat_d + " " + lon_d);

                double radLat1 = Math.PI * lat / 180;
                double radLon1 = Math.PI * lon / 180;
                double radLat2 = Math.PI * lat_d / 180;
                double radLon2 = Math.PI * lon_d / 180;

                // Calculate the change in coordinates
                double deltaLat = radLat2 - radLat1;
                double deltaLon = radLon2 - radLon1;

                // Apply Haversine formula
                double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                           Math.Cos(radLat1) * Math.Cos(radLat2) *
                           Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                double distance = EarthRadiusKm * c * 1000; // Distance in meters
                return distance;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }

        }
        public static string GeneratePDFTicket(TicketModel ticket)
        {
            Document document = new Document();
            Page page = new Page(PageSize.Letter, PageOrientation.Portrait, 54.0f);
            document.Pages.Add(page);

            QrCode qr = new QrCode($"{ticket.ticketPID}", 20, 20); // Adjust X, Y position for QR code
            page.Elements.Add(qr);

            float labelY = 120;

            Table2 table = new Table2(0, labelY, 600, 600);

            // Add columns to the table
            Column2 column1 = table.Columns.Add(150);
            column1.CellDefault.Align = TextAlign.Center;
            table.Columns.Add(90);
            table.Columns.Add(90);
            table.Columns.Add(90);

            // Add header row
            Row2 headerRow = table.Rows.Add(40, Font.TimesRoman, 12, Grayscale.Black, Grayscale.Gray);
            headerRow.CellDefault.Align = TextAlign.Center;
            headerRow.CellDefault.VAlign = VAlign.Center;
            headerRow.Cells.Add("Departure");
            headerRow.Cells.Add("Arrival");
            headerRow.Cells.Add("Departure Date");
            headerRow.Cells.Add("Arrival Date");

            // Add ticket row
            Row2 ticketRow = table.Rows.Add(30);
            ticketRow.Cells.Add(ticket.departureP);
            ticketRow.Cells.Add(ticket.arrivalP);
            ticketRow.Cells.Add(ticket.departureD.ToString("yyyy-MM-dd HH:mm:ss")); // Format the date
            ticketRow.Cells.Add(ticket.arrivalD.ToString("yyyy-MM-dd HH:mm:ss")); // Format the date

            // Set table properties
            table.CellDefault.Padding.Value = 5.0f;
            table.CellSpacing = 5.0f;
            table.Border.Top.Color = RgbColor.Blue;
            table.Border.Bottom.Color = RgbColor.Blue;
            table.Border.Top.Width = 2;
            table.Border.Bottom.Width = 2;
            table.Border.Left.LineStyle = LineStyle.None;
            table.Border.Right.LineStyle = LineStyle.None;

            // Add table to the page
            page.Elements.Add(table);

            // Add QR code
            labelY += 200; // Starting Y position for labels

            Label passengerLabel = new Label($"Passenger: {ticket.passengerFullName}", 20, labelY, 504, 100, Font.Helvetica, 18);
            page.Elements.Add(passengerLabel);

            labelY += 30; // Adjust Y position for next label

            Label seatLabel = new Label($"Seat (№): {ticket.seat}", 20, labelY, 504, 100, Font.Helvetica, 18);
            page.Elements.Add(seatLabel);

            labelY += 30; // Adjust Y position for next label

            Label carriageLabel = new Label($"Carriage (№): {ticket.carriageNumber}", 20, labelY, 504, 100, Font.Helvetica, 18);
            page.Elements.Add(carriageLabel);

            labelY += 30; // Adjust Y position for next label

            Label ticketTypeLabel = new Label($"Ticket Type: {ticket.ticketAgeType}", 20, labelY, 504, 100, Font.Helvetica, 18);
            page.Elements.Add(ticketTypeLabel);

            labelY += 30; // Adjust Y position for next label

            Label additionalOptionsLabel = new Label($"Additional Options: {ticket.addtionalOptions}", 20, labelY, 504, 100, Font.TimesRoman, 18);
            page.Elements.Add(additionalOptionsLabel);

            string projectDirectory = Directory.GetCurrentDirectory();
            string relativePath = @"wwwroot\tickets"; // Use backslash for directory separator

            string fullPath = System.IO.Path.Combine(projectDirectory, relativePath);
            string ticketPath = System.IO.Path.Combine(fullPath, $"{ticket.ticketPID}.pdf");

            document.Draw(ticketPath);

            return ticketPath;
        }
        public async Task SendEmail(List<string> ticketsPaths, IdentityUser user)
        {
            var api = _sendGridApiService.GetSendGridApiKey();
            var client = new SendGridClient(api);

            var from = new EmailAddress("skripka402@ukr.net", "TicketPoint");
            var subject = "Квитки";
            var to = new EmailAddress($"{user.Email}");
            var plainTextContent = $"Шановний клієнт, дякуємо, що обрали саме нас. Ваші замовлені квитки знаходяться у закріплених файлах, доданих до даного листа";
            var htmlContent = $"<p>Шановний клієнт,<br> дякуємо, що обрали саме нас. Ваші замовлені квитки знаходяться у закріплених файлах, доданих до даного листа</p>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            
            foreach (var item in ticketsPaths)
            {
                var name = item.Split("\\")[item.Split("\\").Length - 1];
                var attachment = await GetAttachmentContentAsync(item);
                msg.AddAttachment($"{name}.pdf", attachment, "application/pdf");
            }

            var response = client.SendEmailAsync(msg);
        }

        /*public async Task SendEmail()
        {
            Console.WriteLine("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
            string projectDirectory = Directory.GetCurrentDirectory();
            string relativePath = @"wwwroot/tickets/6634.pdf";
            string fullPath = System.IO.Path.Combine(projectDirectory, relativePath);
            string file = fullPath.Split("/")[fullPath.Split("/").Length - 1];
            Console.WriteLine(file + " ffffffffffffffffffffff");
        }
        */
        private async Task<string> GetAttachmentContentAsync(string attachmentFilePath)
        {
            byte[] bytes = await System.IO.File.ReadAllBytesAsync(attachmentFilePath);
            return Convert.ToBase64String(bytes);
        }
    }
}