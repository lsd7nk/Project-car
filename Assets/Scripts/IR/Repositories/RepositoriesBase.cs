using System;
using System.Collections.Generic;
using ProjectCar.Interactors;

namespace ProjectCar
{
    namespace Repositories
    {
        public sealed class RepositoriesBase
        {
            private Dictionary<Type, Repository> _repositoriesDict;

            public int RepositoriesAmount => _repositoriesDict.Count;

            public RepositoriesBase() => _repositoriesDict = new Dictionary<Type, Repository>();

            public T GetRepository<T>() where T : Repository => (T) _repositoriesDict[typeof(T)];

            public void AddRepository<T>(Interactor interactor) where T : Repository
            {
                var repository = interactor.Repository;
                var type = typeof(T);

                _repositoriesDict.Add(type, repository);
            }
        }
    }
}