using Domain.Models;
using System.Collections.Generic;

namespace Web.Services.Abstraction
{
    public interface IDogPackBuilder
    {
        IEnumerable<DogPack> Build(IEnumerable<Dog> dogs, IEnumerable<PriceRate> rates);
    }
}
