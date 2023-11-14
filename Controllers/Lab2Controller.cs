using lab2WEB.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using lab2WEB.Models.ViewModels;
using System.Security.Cryptography;
using System.Web.Security;
using System.Text;

namespace lab2WEB.Controllers
{
    public class Lab2Controller : Controller
    {
        // ЕСЛИ ЧТОТО СЛОМАЛОСЬ ИЛИ НЕ РАБОТАЕТ писать vk.com/durov
        //  Я ОТКРЫЛ ВИЗУАЛ СТУДИО 2019
        //короче админ TestUser UserPa$$w0rd
        //а обычный чувак User UserPa$$word ТИПО НОЛИК НЕ ПИШЕТСЯ 

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserVM webUser)
        {
            if (ModelState.IsValid)
                using (PrintsevEntities context = new PrintsevEntities())
                {
                    User user = null;
                    user = context.User.Where(u => u.Login == webUser.Login).FirstOrDefault();
                    if (user != null)
                    {
                        string passwordHash = ReturnHashCode(webUser.Password + user.Salt.ToString().ToUpper());
                        if (passwordHash == user.PasswordHash)
                        {
                            string userRole = "";
                            switch (user.UserRole)
                            {
                                case 1:
                                    userRole = "Admin";
                                    break;
                                case 2:
                                    userRole = "Participant";
                                    break;
                            }

                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                                        1,
                                                        user.Login,
                                                        DateTime.Now,
                                                        DateTime.Now.AddDays(1),
                                                        false,
                                                        userRole
                                                        );
                            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
                            return RedirectToAction("ListOfStudents", "Lab2");
                        }
                    }
                }
            ViewBag.Error = "Вы что-то не так ввели =(";
            return View(webUser);
        }

        string ReturnHashCode(string loginAndSalt)
        {
            string hash = "";
            using (SHA1 sha1Hash = SHA1.Create())
            {
                byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(loginAndSalt));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i<data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                hash = sBuilder.ToString().ToUpper();
            }
            return hash;
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("ListOfStudents", "Lab2");
        }

        #region лабы 1-4
        // lab4 поехали
        [HttpGet]
        [Authorize(Roles="Admin")]
        public ActionResult EditStudent(Guid studentID)
        {
            //StudentVM model;
            Студенты model;
            using (var context = new PrintsevEntities())
            {
                Студенты student = context.Студенты.Find(studentID);
                //model = new StudentVM
                model = new Студенты
                {
                    //id lastname name patronumic pall adres code date
                    ID_студента = student.ID_студента,
                    Фамилия = student.Фамилия,
                    Имя = student.Имя,
                    Отчество = student.Отчество,
                    Пол = student.Пол,
                    Адрес_проживания = student.Адрес_проживания,
                    Дата_рождения = student.Дата_рождения,
                    //Код_студента = 1
                };
            }
            ViewBag.Пол = new SelectList(GetGenderList(), "Item1", "Item2", model.Пол);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult EditStudent(/*StudentVM model*/Студенты model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new PrintsevEntities())
                {
                    Студенты editedStudent = new Студенты
                    {//id lastname name patronumic pall adres code
                        ID_студента = model.ID_студента,
                        Фамилия = model.Фамилия,
                        Имя = model.Имя,
                        Отчество = model.Отчество,
                        Пол = model.Пол,
                        Адрес_проживания = model.Адрес_проживания,
                        Дата_рождения = model.Дата_рождения
                    };
                    context.Студенты.Attach(editedStudent);
                    context.Entry(editedStudent).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
                return RedirectToAction("ListOfStudents");
            }
            ViewBag.Пол = new SelectList(GetGenderList(), "Item1", "Item2", model.Пол);
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles="Admin")]
        public ActionResult DeleteStudent(Guid studentID)
        {
            Студенты studentToDelete;
            using (var context = new PrintsevEntities())
            {
                studentToDelete = context.Студенты.Find(studentID);
            }
            ViewBag.Пол = GetGenderList().First(x => x.Item1).Item2;
            return View(studentToDelete);
        }

        [HttpPost, ActionName("DeleteStudent")]
        public ActionResult DeleteConfirmed(Guid studentID)
        {
            using (var context = new PrintsevEntities())
            {
                //Удаление объекта, вариант 1
                Студенты studentToDelete = new Студенты
                {
                    ID_студента = studentID
                };
                context.Entry(studentToDelete).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
            return RedirectToAction("ListOfStudents");
        }

        [ChildActionOnly]
        public ActionResult YearsOld(Guid studentID)
        {
            string message = "";
            using (var context = new PrintsevEntities())
            {
                DateTime BirthDate = context.Студенты.Find(studentID).Дата_рождения;
                message = $"{DateTime.Today.Year - BirthDate.Year} лет";
            }
            return PartialView("YearsOld", message);
        }
        // GET: Lab2
        [AllowAnonymous]
        public ActionResult ListOfStudents()
        {
            List<Студенты> students = new List<Студенты>();
            using (var db = new PrintsevEntities())
            {
                students = db.Студенты.OrderByDescending(x => x.Дата_рождения)
                             .ThenBy(x => x.Фамилия)
                             .ThenBy(x => x.Имя).ToList();
            }
            return View(students);
        }

        [HttpGet]
        [Authorize]
        public ActionResult StudentDetails(Guid studentID)
        {
            Студенты model = new Студенты();
            using (var db = new PrintsevEntities())
            {
                model = db.Студенты.Find(studentID);
            }
            ViewBag.Пол = GetGenderList().First(x => x.Item1).Item2;
            return View(model);
        }

        List<Tuple<bool, string>> GetGenderList()
        {
            var genders = new List<Tuple<bool, string>>
            {
                new Tuple<bool,string>(true, "Мужской"),
                new Tuple<bool,string>(false,"Женский")
            };
            return genders;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateStudent()
        {
            ViewBag.Пол = new SelectList(GetGenderList(), "Item1", "Item2");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStudent(StudentVM newstudent)
        {
            if (ModelState.IsValid)
            {
                using (var context = new PrintsevEntities())
                {
                    Студенты student = new Студенты()
                    {
                        ID_студента = Guid.NewGuid(),
                        Фамилия = newstudent.Фамилия,
                        Имя = newstudent.Имя,
                        Отчество = newstudent.Отчество,
                        Пол = newstudent.Пол,
                        Адрес_проживания = newstudent.Адрес_проживания,
                        Дата_рождения = newstudent.Дата_рождения,
                        //Код_студента = 1
                    };
                    context.Студенты.Add(student);
                    context.SaveChanges();
                }
                    return RedirectToAction("ListOfStudents");
            }
            ViewBag.Пол = new SelectList(GetGenderList(), "Item1", "Item2");
            return View(newstudent);
        }
        #endregion
    }
}