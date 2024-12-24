using MTCG.Database.Repositories;
using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using MTCG.Services.Interfaces;
using System.Xml.Linq;

namespace MTCG.Services
{
    public class PackageService
    {
        private readonly IPackageRepository _packageRepository;

        public PackageService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository ?? throw new ArgumentNullException(nameof(packageRepository));
        }

        public PackageService()
        {
            _packageRepository = new PackageRepository();
        }

        private void CheckValidation(Package package)
        {
            foreach(var card in package.Cards)
            {
                if(string.IsNullOrWhiteSpace(card.Name.Trim()))
                    throw new ArgumentException("Card name is invalid");
                if (card.Damage <= 0)
                    throw new ArgumentException($"Card '{card.Name}' has invalid damage value: {card.Damage}");
                if (string.IsNullOrWhiteSpace(card.Id.Trim()))
                    throw new ArgumentException($"Card '{card.Name}' has invalid id value");
            }
        }
        public void Add(Package package) {
            CheckValidation(package);
            _packageRepository?.Add(package);
        }

        public Package? PopRandom()
        {
           List<Package> packages = _packageRepository.GetAll();
           if (packages.Count == 0) return null;
           Random rnd = new Random();
           Package package = packages[rnd.Next(packages.Count)];
           _packageRepository.Delete(package.Id);
           return package;
        }
    }
}
