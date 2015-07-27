using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using BinLaw.Account.MobileService.DataObjects;
using BinLaw.Account.MobileService.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace BinLaw.Account.MobileService.Controllers
{
    public class BillModelController : TableController<BillModel>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<BillModel>(context, Request, Services);
        }

        // GET tables/BillModel
        [AuthorizeLevel(AuthorizationLevel.User)]
        public IQueryable<BillModel> GetAllBillModel()
        {
            var currentUser = User as ServiceUser;

            return Query().Where(todo => todo.UserId == currentUser.Id);
        }

        // GET tables/BillModel/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [AuthorizeLevel(AuthorizationLevel.User)]
        public SingleResult<BillModel> GetBillModel(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/BillModel/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [AuthorizeLevel(AuthorizationLevel.User)]
        public Task<BillModel> PatchBillModel(string id, Delta<BillModel> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/BillModel/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [AuthorizeLevel(AuthorizationLevel.User)]
        public async Task<IHttpActionResult> PostBillModel(BillModel item)
        {
            // Get the logged-in user.
            var currentUser = User as ServiceUser;

            // Set the user ID on the item.
            item.UserId = currentUser.Id;

            BillModel current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/BillModel/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [AuthorizeLevel(AuthorizationLevel.User)]
        public Task DeleteBillModel(string id)
        {
             return DeleteAsync(id);
        }

    }
}