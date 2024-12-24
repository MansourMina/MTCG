using MTCG.Models;

namespace MTCG.Database.Repositories.Interfaces
{
    public interface IPackageRepository
    {
        void Add(Package package);
        Card? Get(string id);
        List<Package> GetAll();

        void Delete(string id);
    }
}