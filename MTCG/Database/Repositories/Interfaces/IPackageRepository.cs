using MTCG.Models;

namespace MTCG.Database.Repositories.Interfaces
{
    public interface IPackageRepository
    {
        void Add(Package package);
    }
}