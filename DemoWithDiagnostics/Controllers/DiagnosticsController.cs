namespace DemoWithDiagnostics.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;

    using NUnit.Framework;

    public class DiagnosticsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            // Replace with your favorite container, conventions, etc.
            var tests = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        from method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        let hasTestAttribute = method.GetCustomAttribute<TestAttribute>() != null
                        where hasTestAttribute
                        let instance = Activator.CreateInstance(type)
                        select RunTest(instance, method);

            if (Request.ContentType.Contains("json")) 
                return Json(tests.ToArray(), JsonRequestBehavior.AllowGet);

            return this.View(tests.ToArray());
        }

        private static TestResult RunTest(object instance, MethodBase method)
        {
            var result = new TestResult { Name = method.Name };

            var sw = new Stopwatch();
            sw.Start();
            try
            {
                method.Invoke(instance, null);
                result.WasSuccessful = true;
            }
            catch (Exception ex)
            {
                result.WasSuccessful = false;
                result.Message = ex.ToString();
            }
            finally
            {
                sw.Stop();

                result.Duration = sw.ElapsedMilliseconds;
            }

            return result;
        }
    }

    public class TestResult
    {
        public string Message { get; set; }

        public bool WasSuccessful { get; set; }

        public string Name { get; set; }

        public long Duration { get; set; }
    }
}
