using System.Collections.Generic;
using System.Transactions;
using WindyCityUniversity.DAL;
using WindyCityUniversity.Models.BaseTypes;

namespace WindyCityUniversity.Service
{
    public class BulkLoadService
    {
        public void Load<T>(List<T> entities) where T: BaseEntity
        {

            // fast bulk load method using EF
            // curtesey of http://stackoverflow.com/questions/5940225/fastest-way-of-inserting-in-entity-framework
            // with some modifications to handle generic types.
            using (TransactionScope scope = new TransactionScope())
            {
                SchoolContext context = null;
                try
                {
                    context = new SchoolContext();
                    context.Configuration.AutoDetectChangesEnabled = false;

                    int count = 0;
                    foreach (var entityToInsert in entities)
                    {
                        ++count;
                        context = AddToContext(context, entityToInsert, count, 100, true);
                    }

                    context.SaveChanges();
                }
                finally
                {
                    if (context != null)
                        context.Dispose();
                }

                scope.Complete();
            }
        }
        private SchoolContext AddToContext<T>(SchoolContext context, T entity, int count, int commitCount, bool recreateContext) where T : BaseEntity
        {
            context.Set<T>().Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new SchoolContext();
                    context.Configuration.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }
    }
}