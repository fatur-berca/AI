using DFIS.Universal;
using DFIS.Universal.BusinessLogics;
using DFIS.Universal.Domain.DTOs;
using DFIS.Universal.Domain.Outputs;
using DFIS.Utils;
using DocumentFormat.OpenXml.Spreadsheet;
using hms_tom_dev.Models.Common;
using hms_tom_dev.Models.Masters;
using Microsoft.AspNet.Identity;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using System.Xml;
using TOM.EntitiesDAL.EDMX;
using TOM.Master.BusinessLogics;
using TOM.Master.Domain.DTOs;




namespace hms_tom_dev.Controllers
{
    //[AuthorizeAD]
    public class BaseController : Controller
    {
        //public List<string> ButtonAccess = new List<string>();
        //protected int PageID;
        protected string Page = null;
        protected string Ignore = null;
        protected List<String> listButton = new List<string>();
        private InsertUpdateData<MasterUserLocationMappingViewModel> _bulkData;
        public int idRolePage;

        public TOMContextDB _db;

        public BaseController()
        {
            _db = new TOMContextDB();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db?.Dispose();
            }
            base.Dispose(disposing);
        }





        public void SetPage(string Page)
        {
            this.Page = Page;
        }

        public string GetBaseUrl()
        {
            return Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
        }

        public void NoCache()
        {
            // Stop Caching in IE
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            // Stop Caching in Firefox
            Response.Cache.SetNoStore();
        }

        public int GetPageID()
        {
            var currentSession = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var pageID = currentSession.Page.Where(x => x.FunctionName == Page).Select(x => x.IDFunction).FirstOrDefault();
            //var pageID = currentSession.Page.Where(x => x.FunctionName == Page).Select(x => x.IDFunction).FirstOrDefault();
            return pageID;
        }

        public DateTime GetLockDateRangeList()
        {
            var today = DateTime.Now;
            var result = new DateTime();
            MasterConfigurationBLL bll = DependencyResolver.Current.GetService<MasterConfigurationBLL>();
            var mstConf = bll.GetRangeValueConfiguration(Page, EnumHelper.GetDescription(Enums.MasterConfigurationValue.ClosingPage));
            var dateNow = today.Day; // tanggal hari ini
            var dateConf = Int32.Parse(mstConf.Value); // tanggal closing page
            
            if (dateNow <= dateConf) // jika tnggl hari ini > tnggl closing page, closing di bulan selanjutnya
            {
                result = DateTime.Parse(mstConf.Value + "/" + today.Month + "/" + today.Year);
            }
            else if (dateNow > dateConf)
            {
                result = DateTime.Parse(mstConf.Value + "/" + today.AddMonths(1).Month + "/" + today.Year);
            }
            // jika hari ini 16 nov
            // jika 16 > 10 maka closing 

            return result;
        }

        public bool ClosingPageState(DateTime inputDate)
        {
            var currentDate = DateTime.Now; //current date
            MasterConfigurationBLL bll = DependencyResolver.Current.GetService<MasterConfigurationBLL>();
            var mstConf = bll.GetRangeValueConfiguration(Page, EnumHelper.GetDescription(Enums.MasterConfigurationValue.ClosingPage));
            bool returnVal = true;
            List<UserRole> roles = GetListUserRole();
            var rolesname = roles.Select(x => x.RoleName).ToList();
            if (rolesname.Contains(EnumHelper.GetDescription(Enums.RoleUserList.SA)))
            {
                rolesname.Add("MANAGEMENT");
            }
            if (!rolesname.Contains(EnumHelper.GetDescription(Enums.RoleUserList.SA)) )
            {
                var maxCurrentDate = new DateTime(currentDate.Year, currentDate.Month, Int32.Parse(mstConf.Value) + 1, 0,
                    0, 0); // closing date + 1  11 maret 2018
                //var closingDate = new DateTime(currentDate.Year, currentDate.Month, Int32.Parse(mstConf.Value), 0, 0, 0); // closing date 10 maret 2018
                int result = DateTime.Compare(currentDate, maxCurrentDate);
                    // 10/03/2018 00:00:00  ,  //13/03/2018 16:50:00

                //if(currentDate < maxCurrentDate){
                //  tnggl 1 feb 
                //}
                //else if(currentDate == maxCurrentDate){
                //  tnggl 1 mar 
                //}
                var allowedDate = new DateTime();
                if (result < 0)
                {
                    allowedDate = new DateTime(currentDate.Year, currentDate.Month - 1, 1, 0, 0, 0);
                }
                else if (result == 1)
                {
                    allowedDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0);
                }
                int result2 = DateTime.Compare(inputDate, allowedDate);
                if (result2 > -1)
                {
                    returnVal = false;
                }
            }
            else
            {
                returnVal = false;
            }

            return returnVal;
        }

        public UserSession CurrentUser
        {
            get
            {
                var currentUserFromSession = Session["CurrentUser"] as UserSession;
                //if (Session["CurrentUser"] == null)
                //{
                if (!IsAjaxRequest || currentUserFromSession == null) setCurrentUser();
                //}
                /*
                
                if (currentUserFromSession != null)
                {
                    if (!currentUserFromSession.Name.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        setCurrentUser();
                    }
                }
                */
                return (UserSession)Session["CurrentUser"];

                //return new UserSession()
                //{
                //    Name = (HttpContext.User as CustomPrincipal) != null ? (HttpContext.User as CustomPrincipal).Identity.Name : "pmi\admin",
                //    Location = new UserSessionLocation() { Code = "SKT", Name = "SKT" }
                //};
            }
        }

        //public void setCurrentUser()
        //{
        //    MasterUserBLL bll = DependencyResolver.Current.GetService<MasterUserBLL>();
        //    UtilitiesBLL util = DependencyResolver.Current.GetService<UtilitiesBLL>();
        //    MasterUser user = bll.GetLogin(User.Identity.Name);
        //    UserSession login = null;
        //    if (user != null)
        //    {
        //        login = new UserSession();
        //        login.Name = user.IDUser.ToLower();
        //        login.Username = user.FullName;
        //        List<MasterUserLocationMapping> listLocationMap = user.MasterUserLocationMappings.ToList();
        //        var listRole = user.MasterUserRoleMappings.Select(x => x.IDRole).ToList();
        //        if (listRole.Any())
        //        {
        //            var res = util.GetResponsibilityPage(login.Name, listLocationMap);
        //            login.Page = res.Page;
        //            login.Location = res.Location;
        //            login.Button = res.Button;
        //            login.Role = res.Role;
        //        }
        //        else
        //        {
        //            login.Page = null;
        //            login.Location = null;
        //        }
        //    }
        //    Session["CurrentUser"] = login;
        //}

        


    public bool IsUserInGroup(string samAccountName, string targetGroupName)
    {
            //rem to test ; 
            return true; 
        if (string.IsNullOrWhiteSpace(samAccountName))
            throw new ArgumentException("samAccountName is required.", "samAccountName");
        if (string.IsNullOrWhiteSpace(targetGroupName))
            throw new ArgumentException("targetGroupName is required.", "targetGroupName");

        // Replace with your DC FQDN and base DN
        string ldapServer = "LDAP://pmintl.net";  // Or use GC://dc1.yourdomain.net for Global Catalog
        string baseDn = "DC=pmintl,DC=net";       // Default naming context

        // Optional: supply credentials if process identity can't access AD
        // var directoryRoot = new DirectoryEntry(
        //     string.Format("{0}/{1}", ldapServer, baseDn),
        //     "YOURDOMAIN\\svc_account",
        //     "password",
        //     AuthenticationTypes.Secure);

        var directoryRoot = new DirectoryEntry(
            string.Format("{0}/{1}", ldapServer, baseDn),
            null,
            null,
            AuthenticationTypes.Secure);

        using (var searcher = new DirectorySearcher(directoryRoot))
        {
            // Exclude disabled accounts: userAccountControl bit 2
            // NOTE: EscapeLdapValue is called for samAccountName to avoid filter injection issues.
            searcher.Filter =
                string.Concat(
                    "(&",
                    "(objectClass=user)",
                    "(!(objectClass=computer))",
                    "(!(userAccountControl:1.2.840.113556.1.4.803:=2))",
                    "(sAMAccountName=", EscapeLdapValue(samAccountName), "))");

            searcher.PropertiesToLoad.Add("memberOf");
            searcher.ClientTimeout = TimeSpan.FromSeconds(5);
            searcher.ServerTimeLimit = TimeSpan.FromSeconds(5);

            SearchResult res = searcher.FindOne();
            if (res == null) return false;

            // Direct memberships check
            if (res.Properties.Contains("memberOf"))
            {
                foreach (object dnObj in res.Properties["memberOf"])
                {
                    string dn = dnObj != null ? dnObj.ToString() : string.Empty;
                    string cn = ExtractCn(dn);
                    if (string.Equals(cn, targetGroupName, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            // Optional: nested membership check via tokenGroups (more reliable for nested groups)
            DirectoryEntry entry = res.GetDirectoryEntry();
            try
            {
                entry.RefreshCache(new[] { "tokenGroups" });
                PropertyValueCollection rawTok = entry.Properties["tokenGroups"];
                if (rawTok != null && rawTok.Count > 0)
                {
                    // Resolving SIDs to names requires additional lookup.
                    // If you need nested group resolution by name, consider using AccountManagement API (see below),
                    // or implement SID-to-group-name resolution using SecurityIdentifier and another search.
                }
            }
            catch (Exception)
            {
                // Some environments may restrict tokenGroups; ignore or log as needed.
            }
        }
        return false;
    }


    //pmi\cwiratma -> ning ad onok
        // cek ning db 
            // ora enek -> insert anyar 
                // user, masteruser

    public async Task<bool> EnsureUserExistsFromAdAsync(string domainSam)
    {


            Debug.WriteLine("Get data EnsureUserExistsFromAdAsync = " + domainSam);

            // Validasi input "DOMAIN\sam"
            if (string.IsNullOrWhiteSpace(domainSam))
                throw new ArgumentException("samAccountName is required.", "samAccountName");

            //string netbios = parts[0];
            //string sam = parts[1];

            string ldapServer = "LDAP://pmintl.net";
            string baseDn = "DC=pmintl,DC=net";

            DirectoryEntry directoryRoot = null;
            DirectorySearcher searcher = null;

            try
            {
                try
                {
                    directoryRoot = new DirectoryEntry(
                        $"{ldapServer}/{baseDn}",
                        null,  // atau "PMI\\svc_account"
                        null,  // atau password
                        AuthenticationTypes.Secure
                    );

                    searcher = new DirectorySearcher(directoryRoot)
                    {
                        // FIX: gunakan '&' bukan '&amp;' di literal C#
                        Filter = string.Concat(
                            "(&",
                            "(objectClass=user)",
                            "(!(objectClass=computer))",
                            "(!(userAccountControl:1.2.840.113556.1.4.803:=2))",
                            "(sAMAccountName=", EscapeLdapValue(domainSam), "))"
                        ),
                        ClientTimeout = TimeSpan.FromSeconds(5),
                        ServerTimeLimit = TimeSpan.FromSeconds(5)
                    };

                    searcher.PropertiesToLoad.Add("displayName");
                    searcher.PropertiesToLoad.Add("mail");
                    searcher.PropertiesToLoad.Add("telephoneNumber");
                }
                catch (DirectoryServicesCOMException ex)
                {
                    // Kesalahan koneksi/bind ke LDAP
                    Console.WriteLine("LDAP BIND/SEARCH INIT ERROR => " + GetDeepMessage(ex));
                    Console.WriteLine(ex.ToString());
                    throw;
                }
                catch (InvalidOperationException ex)
                {
                    // Misconfig pada DirectoryEntry/Searcher
                    Console.WriteLine("LDAP INVALID OPERATION => " + ex.Message);
                    Console.WriteLine(ex.ToString());
                    throw;
                }

                SearchResult res;
                try
                {
                    res = searcher.FindOne();
                }
                catch (DirectoryServicesCOMException ex)
                {
                    Console.WriteLine("LDAP SEARCH ERROR => " + GetDeepMessage(ex));
                    Console.WriteLine(ex.ToString());
                    throw;
                }

                if (res == null)
                    throw new Exception("User tidak ditemukan di AD: " + domainSam);

                string fullName = GetProp(res, "displayName");
                if (string.IsNullOrWhiteSpace(fullName)) fullName = domainSam;
                string email = GetProp(res, "mail");
                string phone = GetProp(res, "telephoneNumber");

                Console.WriteLine("INFO -> Name  = " + fullName);
                Console.WriteLine("INFO -> Email = " + email);
                Console.WriteLine("INFO -> Phone = " + phone);

                using (var db = new TOMContextDB())
                {
                    // Aktifkan logging EF (opsional, sangat membantu debug)
                    db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);


                    var IdUserFind = @"PMI\" + domainSam;
                    var rows = db.Database.SqlQuery<MasterUserViewModel>(@"
                        SELECT 
                            ID,
                            Name,
                            Email
                        FROM Users
                        WHERE ID = @p0
                    ", IdUserFind).ToList();


                    if (rows.Count == 0)
                    {
                        // 2) Jika tidak ada, INSERT
                        var now = DateTime.Now;

                        var affected = db.Database.ExecuteSqlCommand(@"
                                INSERT INTO Users
                                    (ID, Name, Email)
                                VALUES
                                    (@p0,    @p1,      @p2)
                            ",
                            IdUserFind,                          // @p0  IDUser
                            string.IsNullOrWhiteSpace(fullName) ? domainSam : fullName,  // @p1 FullName
                            string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email // @p2 Email
                        );

                        Console.WriteLine("INFO -> Insert [Users] commit.");
                    }
                    else
                    {
                        Console.WriteLine("INFO -> User sudah ada di [Users]; skip insert.");
                    }



                    // ===== MASTER USERS =====
                    MasterUser existingMaster = null;
                    try
                    {
                        existingMaster = await db.MasterUsers.FirstOrDefaultAsync(x => x.IDUser == IdUserFind);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine("INVALID OPERATION saat FirstOrDefaultAsync(MasterUsers) => " + ex.Message);
                        Console.WriteLine(ex.ToString());
                        throw;
                    }

                    if (existingMaster == null)
                    {
                        var mu = new MasterUser
                        {
                            IDUser = IdUserFind,
                            FullName = fullName,
                            Email = email,
                            Phone = phone,
                            Address = "",
                            IsActive = true,
                            CreatedBy = "system",
                            CreatedDate = DateTime.Now,
                            UpdatedBy = "system",
                            UpdatedDate = DateTime.Now
                        };

                        db.MasterUsers.Add(mu);
                        Console.WriteLine("INFO -> Insert [MasterUsers] siap dikommit.");
                    }
                    else
                    {
                        Console.WriteLine("INFO -> User sudah ada di [MasterUsers]; skip insert.");
                    }

                    // Save 2
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        var errors = ex.EntityValidationErrors
                            .SelectMany(e => e.ValidationErrors)
                            .Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
                        var message = "VALIDATION ERROR (MasterUsers) => " + string.Join("; ", errors);
                        Console.WriteLine(message);
                        Console.WriteLine(ex.ToString());
                        throw new Exception(message, ex);
                    }
                    catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                    {
                        Console.WriteLine("DB UPDATE ERROR (MasterUsers) => " + GetDeepMessage(ex));
                        Console.WriteLine(ex.ToString());
                        throw;
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine("INVALID OPERATION (MasterUsers Save) => " + ex.Message);
                        Console.WriteLine(ex.ToString());
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("UNHANDLED (MasterUsers Save) => " + ex.Message);
                        Console.WriteLine(ex.ToString());
                        throw;
                    }
                }

                return true;
            }
            finally
            {
                if (searcher != null) searcher.Dispose();
                if (directoryRoot != null) directoryRoot.Dispose();
            }
        }

        // Helper untuk inner exception terdalam
        private static string GetDeepMessage(Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            return ex.Message;
        }




        private static string GetProp(SearchResult r, string name)
        {
            if (r?.Properties != null &&
                r.Properties.Contains(name) &&
                r.Properties[name] != null &&
                r.Properties[name].Count > 0)
            {
                var v = r.Properties[name][0];
                return v != null ? v.ToString() : string.Empty;
            }
            return string.Empty;
        }




        private static string ExtractCn(string dn)
        {
            if (string.IsNullOrEmpty(dn)) return string.Empty;
            if (!dn.StartsWith("CN=", StringComparison.OrdinalIgnoreCase)) return dn;
            int commaIdx = dn.IndexOf(",", StringComparison.OrdinalIgnoreCase);
            if (commaIdx > 3) return dn.Substring(3, commaIdx - 3);
            return dn.Substring(3);
        }


        private static string EscapeLdapValue(string value)
        {
            // Escape special LDAP filter characters: * ( ) \ NUL and backslash
            if (value == null) return string.Empty;

            return value
                .Replace("\\", "\\5c")
                .Replace("*", "\\2a")
                .Replace("(", "\\28")
                .Replace(")", "\\29")
                .Replace("\0", "\\00");
        }

        private static string NormalizeSam(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            // Pastikan hanya satu backslash
            return input.Replace("\\\\", "\\");
        }



        public void setCurrentUser()
        {

            // Fallback ke user default jika Name kosong/null
            /*string userData = string.IsNullOrWhiteSpace(User.Identity.Name) ? "PMI\\aakbar11" : User.Identity.Name;
            string userDataFind = string.IsNullOrWhiteSpace(User.Identity.Name) ? "aakbar11" : User.Identity.Name;*/

            // Global.asax / Controller
            var windowsIdentity = System.Web.HttpContext.Current.User?.Identity as System.Security.Principal.WindowsIdentity;
            var domainBackslashUser = windowsIdentity?.Name; // "DOMAIN\\username"
            var samAccount = domainBackslashUser?.Split('\\').Last();



            string rawIdentity = User.Identity?.Name;
            string sam = string.IsNullOrWhiteSpace(rawIdentity)
                ? "aakbar11"
                : rawIdentity.Contains("\\") ? rawIdentity.Split('\\').Last() : rawIdentity;

            string userData = string.IsNullOrWhiteSpace(rawIdentity) ? "PMI\\aakbar11" : rawIdentity; // tetap domain\sam untuk IDUser
            string userDataFind = sam; // <-- Kirim samAccountName murni, mis: "aakbar11"
            //rem to test Fatur; 
            userDataFind = "pmi\\pherluki";
            userData = "pmi\\pherluki";
            Debug.WriteLine("Check userData (" +userData+ ") -> SSO -> login windows auth : userDataFind (" + userDataFind + ")");

            EnsureUserExistsFromAdAsync(userDataFind);

            using (var db = new TOMContextDB())
            {
                //var rows = db.Database.SqlQuery<MappingSigaRoleViewModel>(@"
                //    SELECT id, nama, role_id, flag
                //    FROM MappingSigaRole
                //    WHERE flag = 1
                //    ORDER BY id
                //").ToList();

                var rows = db.MasterRoles.Where(x => x.IsActive == true).ToList();


                bool matched = true;

            


                foreach (var item in rows)
                {
                    // PMI\aakbar11 PMI 
                    Debug.WriteLine("Check Remarks (" + item.Remarks + ")");
                    if (IsUserInGroup(userDataFind, item.Remarks))
                    {

                        /*var existing = db.MasterUserRoleMappings
                                         .FirstOrDefault(x => x.IDUser == userData); // tanpa IsActive

                        if (existing != null)
                        {
                            existing.IDRole = item.IDRole;
                            existing.UpdatedBy = userData;
                            existing.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                        }*/


                        //db.SaveChanges();
                        matched = false;


                        idRolePage = item.IDRole;

                        Debug.WriteLine($"Masuk nomor {item.IDRole} — CN: {item.RoleName}, Role: {item.Remarks}");


                        // activate the user 
                        var mu = db.MasterUsers.FirstOrDefault(x => x.IDUser == userData);
                        if (mu != null)
                        {
                            mu.IsActive = true;
                            mu.UpdatedBy = userData;
                            mu.UpdatedDate = DateTime.Now;
                            db.SaveChanges();
                        }

                        break;
                    }

                }



                Debug.WriteLine($"[DEBUG] Sebelum masuk IF matched: {matched}");


                if (matched)
                {



                    Debug.WriteLine($"[DEBUG] Sesudah masuk IF matched: {matched}");
                    // disable the user
                    var mu = db.MasterUsers.FirstOrDefault(x => x.IDUser == userData);
                    if (mu != null)
                    {
                        mu.IsActive = false;
                        mu.UpdatedBy = userData;
                        mu.UpdatedDate = DateTime.Now;
                        db.SaveChanges();
                    }

                }


                Debug.WriteLine($"[DEBUG] Check Userdata: {userData}");


                TOM.Master.BusinessLogics.UtilitiesBLL.MasterUserBLL bll = DependencyResolver.Current.GetService<TOM.Master.BusinessLogics.UtilitiesBLL.MasterUserBLL>();
                UtilitiesBLL util = DependencyResolver.Current.GetService<UtilitiesBLL>();
                // MasterUser user = bll.GetLogin(userData);

                var user = db.MasterUsers.FirstOrDefault(x => x.IDUser == userData);
                UserSession login = null;


                Debug.WriteLine($"[DEBUG] Sebelum check idRolePage: {idRolePage}");
                Debug.WriteLine(user);


                if (user != null)
                {
                    login = new UserSession();
                    login.Name = user.IDUser.ToLower();
                    login.Username = user.FullName;
                    login.Email = user.Email;
                    var res = util.GetResponsibilityPage(user.IDUser.ToLower(), idRolePage);
                    login.Page = res.Page;
                    login.Role = res.Role;
                    login.Location = res.Location;
                    login.Button = res.Button;
                }


                Session["CurrentUser"] = login;




            }

        


        }

        public string GetUserName()
        {
            var currentSession = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var fullname = currentSession.Username;
            return fullname;
        }

        public string GetUserId()
        {
            var currentSession = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var iduser = currentSession.Name;
            return iduser;
        }

        public SelectList GetUserRegionSelectList()
        {
            var currentSession = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var listRegion = currentSession.Location.Where(c => c.Type == "Region").Select(c => new { UserRegion = c.LocationName }).ToList();
            //var listRegion = CurrentUser.Location.Where(c => c.Type == "Region").Select(c => new { UserRegion = c.LocationName }).ToList();
            return new SelectList(listRegion, "UserRegion", "UserRegion");
        }

        public List<FunctionPage> GetPages()
        {
            var currentSession = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var listPage = currentSession.Page.ToList();
            return listPage;
        }

        public List<FunctionPage> GetButtons()
        {
            var currentSession = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var listButtons = currentSession.Button.Where(x=>x.ParentIdFunction == GetPageID()).ToList();
            return listButtons;
        }

        public List<UserLocationMap> GetListLocation()
        {
            var currentSession = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var listlocation = currentSession.Location.ToList();
            return listlocation;
        }

        public List<UserRole> GetListUserRole()
        {
            var currentSession = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var listrole = currentSession.Role.ToList();
            return listrole;
        }

        //public UserLocationMap CurrentUserLocation
        //{
        //    get { return CurrentUser.Location[0]; }
        //}

        public List<NewsHighlightDTO> GetNews()
        {
            UtilitiesBLL util = DependencyResolver.Current.GetService<UtilitiesBLL>();
            var dbResult = util.GetNews();
            return dbResult;
        }

        protected void PermissionIgnore(string action)
        {
            this.Ignore = action;
        }
        bool IsAjaxRequest = false;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            IsAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();
            string MicrosoftAccount = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            //Debug.WriteLine("Debug microsoft account = " + MicrosoftAccount);

     
            var user = CurrentUser;
            //var user1 = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var descriptor = filterContext.ActionDescriptor;
            var actionName = descriptor.ActionName;
            var controllerName = descriptor.ControllerDescriptor.ControllerName;
            UtilitiesBLL util = DependencyResolver.Current.GetService<UtilitiesBLL>();

            var skipPermission = false;

            if (this.Ignore != null)
            {
                var ignoreAction = Ignore.Split(';').ToList();
                if (ignoreAction.Contains(actionName))
                {
                    skipPermission = true;
                }
            }

            if (user == null)
            {
                if (controllerName != "Login")
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        // the controller action was invoked with an AJAX request
                        Response.ClearContent();

                        Response.StatusCode = 401;
                        Response.End();
                    }
                    else
                    {

                        filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary { { "controller", "Error" }, { "action", "UserNotRegistered" } });
                        return;
                    }
                }
            }

            if (controllerName != "Login" && controllerName != "Home" && !skipPermission)
            {
                if (user.Page == null)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        // the controller action was invoked with an AJAX request
                        Response.ClearContent();
                        Response.StatusCode = 401;
                        Response.End();
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary { { "controller", "Home" }, { "action", "index" } });
                        return;
                    }
                }
                var superAdmin = false;
                var listRole = util.GetListRole(user.Name);
                foreach (var role in listRole)
                {
                    if (role.IDRole == 1)
                    {
                        superAdmin = true;
                        break;
                    }
                }
                if (!superAdmin)
                {
                    if (GetPageID() == 0 && controllerName != "CustomReportState")
                    {
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary { { "controller", "Error" }, { "action", "UserNotAuthorized" } });
                        return;
                    }
                }

                //var button = util.GetResponsibilityButton(user.Name, GetPageID());
                //ViewBag.ButtonAccess = button;
                //var page = util.GetRolePage(user.Name);
                var page = GetPages();
                ViewBag.PageAccess = page.Select(x => x.FunctionName).ToList();
                //if (listButton.Count == 0)
                //{
                //    listButton = util.GetResponsibilityButton(GetUserId(), GetPageID());    
                //}
                
                //ViewBag.ButtonAccess = listButton;
                var buttons = GetButtons();
                ViewBag.ButtonAccess = buttons.Select(x => x.FunctionName).ToList();
                //ViewBag.ListLocation = GetListLocation();
                //ViewBag.ListUserRole = GetListUserRole();
            }

            //if (controllerName != "Login")
            //{
            //    var listRole = util.GetListRole(CurrentUser.Name);
            //    if (listRole.Count == 0)
            //    {
            //        filterContext.Result = new RedirectToRouteResult(
            //        new RouteValueDictionary { { "controller", "Error" }, { "action", "UserNotAuthorized" } });
            //        return;
            //    }
            //    //ViewBag.listResponsibility = Mapper.Map<List<UtilSecurityResponsibilitiesViewModel>>(listRes); ;
            //    //ViewBag.ResponName = user.Responsibility.ResponsibilityName;
            //}


            base.OnActionExecuting(filterContext);


        }

        static Random rd = new Random();
        private bool bulkData;

        public string RandemUrl()
        {
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
            char[] chars = new char[10];

            for (int i = 0; i < 10; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return "?" + new string(chars);
        }
    }
}