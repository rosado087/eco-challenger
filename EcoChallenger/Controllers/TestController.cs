using System.Reflection;
using EcoChallenger.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoChallenger.Controllers
{
    public class TestController : BaseApiController
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _ctx;

        public TestController(IWebHostEnvironment env, AppDbContext context) {
            _ctx = context;
            _environment = env;

            if(!_environment.IsEnvironment("Test"))
                throw new Exception("Application must be running in test mode");
        }

#if DEBUG
        /// <summary>
        /// Method just used in automation testing to retrieve recovery tokens
        /// </summary>
        /// <returns>List of recovery tokens</returns>
        [AllowAnonymous]
        [HttpGet("get-recovery-tokens")]
        public IActionResult getRecovertTokens() {                       
            return Ok(FakeEmailService.Tokens);
        }

        /// <summary>
        /// Resets the database data, this is used in between the tests.
        /// The method uses Reflection to find all the entities!!
        /// </summary>
        /// <returns>List of recovery tokens</returns>
        [AllowAnonymous]
        [HttpPost("reset-db")]
        public IActionResult ResetDatabase()
        {
            // Get all the entity types that are used in DB
            var entityTypes = _ctx.Model.GetEntityTypes();
            
            foreach (var entityType in entityTypes)
            {
                var clrType = entityType.ClrType;

                /*
                * Here we try to fetch the Set method from AppDbContext, but since it has
                * multiple overloads, we must use this contraption to make sure we filter and get
                * the one we want.
                */
                var method = typeof(DbContext)
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.Name == "Set" && m.IsGenericMethod && m.GetParameters().Length == 0)
                    .First();

                var genericMethod = method.MakeGenericMethod(clrType);
                var dbSet = genericMethod.Invoke(_ctx, null);

                var removeRangeMethod = dbSet!.GetType().GetMethod("RemoveRange", new[] { typeof(IEnumerable<>).MakeGenericType(clrType) });

                if (dbSet is IQueryable queryable && removeRangeMethod != null)
                {
                    var toListMethod = typeof(Enumerable)
                        .GetMethod("ToList", BindingFlags.Public | BindingFlags.Static)?
                        .MakeGenericMethod(clrType);

                    if(toListMethod == null) throw new NullReferenceException("Could not get AppDbContext toList method");

                    var entities = toListMethod.Invoke(null, new object[] { queryable });

                    removeRangeMethod.Invoke(dbSet, new[] { entities });
                }
            }

            _ctx.SaveChanges();
            return Ok();
        }
        #endif
    }


}
