using E_TutorApp.Domain.Entities.Concretes;
using E_TutorApp.Domain.ViewModels;
using E_TutorApp.Persistence.Db_Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace E_TutorApp.Web.Controllers
{
    public class CheckOutController : Controller
    {
        private TutorDbContext _context;
        private readonly UserManager<User> _userManager;

        public CheckOutController(TutorDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Success(string basicsId)
        {
            TempData["basicId"] = basicsId;
            return View();
        }


        public IActionResult OrderConfirmation(string basicId)
        {
            var service = new SessionService();
            Session session = service.Get(TempData["Session"].ToString());

            if(session.PaymentStatus == "paid")
            {
                var transaction = session.PaymentIntentId.ToString();

                //var basicsId = TempData["CourseBasicId"];
                TempData["basicId"] = basicId;
                return View("Success" );
                //return View("Success");
            }
            return RedirectToAction("Login", "Account");
        }


        //[Authorize(Roles = "Student")]
        [HttpGet]
        public async Task < IActionResult> Paymant(string id )   // Burada alinan Id   BasicCourseId -dir.
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user!.Id;
            var userDetail = _context.DetailStudents.Include(u => u.Student).FirstOrDefault(u => u.StudentId == userId) ;


            var coursesBasic = _context.CourseBasics.Select(c => new CourseBasicInfoViewModel
            {
                Id = c.Id,
                Title = c.Title,
                Subtitle = c.Subtitle,
                Salecount = c.SaleCount,
                Rating = c.RatingScore,
                ImgUrl = c.AdvanceInfos!.ThumbnailUrl,
                CategoryName = c!.Category!.Name,
                Price = c.Price,
            }).ToList();



            var domain = "https://localhost:7298/";

            var options = new SessionCreateOptions()
            {
                SuccessUrl = domain + $"CheckOut/OrderConfirmation/?basicId={id}",
                CancelUrl = domain + $"Account/Login",
                LineItems = new List< SessionLineItemOptions>(),
                Mode ="payment",
                ///CustomerEmail= "mamedly99@gmail.com"
            };

            foreach(var item in coursesBasic)
            {
                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)item.Price,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Title.ToString()
                        }
                    },
                    Quantity = 2
                };
                
                options.LineItems.Add(sessionListItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);


            TempData["Session"] = session.Id;

            
            var course = await _context.CourseBasics.FirstOrDefaultAsync(c => c.Id == id);
           // course!.Students!.Append(userDetail);
           // await _context.SaveChangesAsync();
            var student = await _context.DetailStudents.FirstOrDefaultAsync (s => s.Id == userId);
           // student!.EnrolledCoursesOfStudent!.Append(course);
          //  await _context.SaveChangesAsync();
            TempData["CourseBasicId"] = id;


            return new StatusCodeResult(303);


            //return View(coursesBasic);
        }
    }
}
