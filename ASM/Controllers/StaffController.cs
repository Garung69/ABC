﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using ASM.EF;
using ASM.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ASM.Controllers
{
    public class StaffController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCategory(CourseCategoryEntity a)
        {
            using (var abc = new EF.CMSContext())
            {
                abc.courseCategoryEntities.Add(a);
                abc.SaveChanges();
            }

            TempData["message"] = $"Successfully add class {a.Name} to system!";

            return RedirectToAction("ShowCategory");
        }

        public ActionResult ShowCategory()
        {
            using (var classes = new EF.CMSContext())
            {
                var Classroom = classes.courseCategoryEntities.OrderBy(a => a.Id).ToList();
                return View(Classroom);
            }
        }


        [HttpGet]
        public ActionResult EditCategory(int id)
        {
            using (var classes = new EF.CMSContext())
            {
                var Class = classes.courseCategoryEntities.FirstOrDefault(c => c.Id == id);
                return View(Class);
            }
        }

        [HttpPost]
        public ActionResult EditCategory(int id, CourseCategoryEntity a)
        {
            using (var abc = new EF.CMSContext())
            {
                abc.Entry<CourseCategoryEntity>(a).State = System.Data.Entity.EntityState.Modified;

                abc.SaveChanges();
            }

            return RedirectToAction("ShowCategory");
        }

        [HttpGet]
        public ActionResult DeleteCategory(int id, CourseCategoryEntity a)
        {
            using (var classes = new EF.CMSContext())
            {
                var Class = classes.courseCategoryEntities.FirstOrDefault(c => c.Id == id);
                return View(Class);
            }
        }

        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            using (var abc = new EF.CMSContext())
            {
                var xxx = abc.courseCategoryEntities.FirstOrDefault(b => b.Id == id);
                if (xxx != null)
                {
                    abc.courseCategoryEntities.Remove(xxx);
                    abc.SaveChanges();
                }
                TempData["message"] = $"Successfully delete book with Id: {xxx.Id}";
                return RedirectToAction("ShowCategory");
            }
        }
        //-------------------------------------------------------------------------------------------------//

        private List<SelectListItem> getList()
        {
            using (var abc = new EF.CMSContext())
            {
                var stx = abc.courseCategoryEntities.Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList();
                return stx;
            }
        }

        [HttpGet]
        public ActionResult AddCourse()
        {
            ViewBag.Class = getList();
            return View();
        }

        [HttpPost]
        public ActionResult AddCourse(CourseEntity a)
        {
            using (var abc = new EF.CMSContext())
            {
                abc.Courses.Add(a);
                abc.SaveChanges();
            }

            TempData["message"] = $"Successfully add class {a.Name} to system!";

            return RedirectToAction("ShowCourse");
        }

        [HttpGet]
        public ActionResult EditCourse(int id, CourseEntity a)
        {
            ViewBag.Class = getList();
            using (var classes = new EF.CMSContext())
            {
                var Class = classes.Courses.FirstOrDefault(c => c.Id == id);
                return View(Class);
            }
        }

        [HttpPost]
        public ActionResult EditCourse(CourseEntity a)
        {
            using (var abc = new EF.CMSContext())
            {
                abc.Entry<CourseEntity>(a).State = System.Data.Entity.EntityState.Modified;

                abc.SaveChanges();
            }

            return RedirectToAction("ShowCategory");
        }

        public ActionResult ShowCourse()
        {
            using (var classes = new EF.CMSContext())
            {
                var Classroom = classes.Courses.OrderBy(a => a.Id).ToList();
                return View(Classroom);
            }
        }

        [HttpGet]
        public ActionResult DeleteCourse(int id, CourseCategoryEntity a)
        {
            using (var classes = new EF.CMSContext())
            {
                var Class = classes.courseCategoryEntities.FirstOrDefault(c => c.Id == id);
                return View(Class);
            }
        }

        [HttpPost]
        public ActionResult DeleteCourse(int id)
        {
            using (var abc = new EF.CMSContext())
            {
                var xxx = abc.Courses.FirstOrDefault(b => b.Id == id);
                if (xxx != null)
                {
                    abc.Courses.Remove(xxx);
                    abc.SaveChanges();
                }
                TempData["message"] = $"Successfully delete book with Id: {xxx.Id}";
                return RedirectToAction("ShowCategory");
            }
        }
        //================================================================================================//


        private List<CourseEntity> Convert(
             EF.CMSContext bwCtx,
             string formatIds)
        {
            var abc = formatIds.Split(',')
                                        .Select(id => Int32.Parse(id))
                                        .ToArray();
            return bwCtx.Courses.Where(f => abc.Contains(f.Id)).ToList();

        }
        private void SetViewBag()
        {
            using (var bwCtx = new EF.CMSContext())
            {
                ViewBag.Publishers = bwCtx.Courses
                                  .Select(p => new SelectListItem
                                  {
                                      Text = p.Name,
                                      Value = p.Id.ToString()
                                  })
                                  .ToList();

                ViewBag.Formats = bwCtx.Courses.ToList(); //select *
            }
        }
        [HttpGet]
        public ActionResult CreateTrainee()
        {
            SetViewBag();
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> CreateTrainee(UserInfor a, FormCollection f)
        {
            if (f["classId[]"].IsEmpty())
            {
                var context = new CMSContext();
                var store = new UserStore<UserInfor>(context);
                var manager = new UserManager<UserInfor>(store);

                var user = await manager.FindByEmailAsync(a.Email);

                if (user == null)
                {
                    user = new UserInfor
                    {
                        UserName = a.Email.Split('@')[0],
                        Email = a.Email,
                        Name = a.Email.Split('@')[0],
                        Role = "trainee",

                    };
                    await manager.CreateAsync(user, a.PasswordHash);
                    await CreateRole(a.Email, "trainee");
                }
                return RedirectToAction("ShowTrainee");
            }
            else
            {
                var context = new CMSContext();
                var store = new UserStore<UserInfor>(context);
                var manager = new UserManager<UserInfor>(store);

                
                var user = await manager.FindByEmailAsync(a.Email);
                if (user == null)
                {
                    user = new UserInfor
                    {
                        UserName = a.Email.Split('@')[0],
                        Email = a.Email,
                        Name = a.Email.Split('@')[0],
                        Role = "trainee",
                       
                    };
                    user.listCourse = Convert(context, f["formatIds[]"]);
                    await manager.CreateAsync(user, a.PasswordHash);
                    await CreateRole(a.Email, "trainee");
                }
                return View();
            }
            
        }

        public async Task<ActionResult> CreateRole(string email, string role)
        {
            var context = new CMSContext();
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var userStore = new UserStore<UserInfor>(context);
            var userManager = new UserManager<UserInfor>(userStore);

            if (!await roleManager.RoleExistsAsync(SecurityRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole { Name = SecurityRoles.Admin });
            }

            if (!await roleManager.RoleExistsAsync(SecurityRoles.Staff))
            {
                await roleManager.CreateAsync(new IdentityRole { Name = SecurityRoles.Staff });
            }
            if (!await roleManager.RoleExistsAsync(SecurityRoles.Trainee))
            {

                await roleManager.CreateAsync(new IdentityRole { Name = SecurityRoles.Trainee });

            }
            if (!await roleManager.RoleExistsAsync(SecurityRoles.Trainer))
            {
                await roleManager.CreateAsync(new IdentityRole { Name = SecurityRoles.Trainer });

            }

            var User = await userManager.FindByEmailAsync(email);

            if (!await userManager.IsInRoleAsync(User.Id, SecurityRoles.Admin) && role == "admin")
            {
                userManager.AddToRole(User.Id, SecurityRoles.Admin);
            }
            if (!await userManager.IsInRoleAsync(User.Id, SecurityRoles.Staff) && role == "staff")
            {
                userManager.AddToRole(User.Id, SecurityRoles.Staff);
            }
            if (!await userManager.IsInRoleAsync(User.Id, SecurityRoles.Trainer) && role == "trainer")
            {
                userManager.AddToRole(User.Id, SecurityRoles.Trainer);
            }
            if (!await userManager.IsInRoleAsync(User.Id, SecurityRoles.Trainee) && role == "trainee")
            {
                userManager.AddToRole(User.Id, SecurityRoles.Trainee);
            }
            return Content("done!");
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var context = new CMSContext();
            var store = new UserStore<UserInfor>(context);
            var manager = new UserManager<UserInfor>(store);

            
                var a = manager.Users.FirstOrDefault(b => b.Id == id);

            if (a != null) // if a book is found, show edit view
            {
                //ViewBag.Publishers = GetPublishersDropDown();
                SetViewBag();
                return View(a);
            }
            else // if no book is found, back to index
            {
                return RedirectToAction("Index"); //redirect to action in the same controller
            }
        }


        public ActionResult ShowTrainee()
        {
            using (var ASMCtx = new EF.CMSContext())
            {
                var Staff = ASMCtx.Users.Where(s => s.Role == "trainee").ToList();
                return View(Staff);
            }
        }
    }
}