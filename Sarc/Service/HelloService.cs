using Sarc.Model.Entity;
using Sarc.Repository.Interface;
using Sarc.Service.Interface;

namespace Sarc.Service
{
    public class HelloService : IHelloService
    {
        private readonly IHelloRepository _repo;

        public HelloService(IHelloRepository repo)
        {
            _repo = repo;
        }

        public string Hello() => _repo.Hello();
    }
}