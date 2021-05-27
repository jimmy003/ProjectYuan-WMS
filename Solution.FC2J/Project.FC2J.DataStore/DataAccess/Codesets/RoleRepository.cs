using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.FC2J.DataStore.Interfaces.Codesets;
using Project.FC2J.DataStore.Internal.DataAccess;
using Project.FC2J.Models.User;

namespace Project.FC2J.DataStore.DataAccess.Codesets
{
    public class RoleRepository : IRoleRepository
    {
        private readonly string _spGetList = "GetRoles";


        public async Task<List<Role>> GetList()
        {
            return await _spGetList.GetList<Role>();
        }

        public Task Remove(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Role> Save(Role value)
        {
            throw new NotImplementedException();
        }

        public Task Update(Role value)
        {
            throw new NotImplementedException();
        }
    }
}
