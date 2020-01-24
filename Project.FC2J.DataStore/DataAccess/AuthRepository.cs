using Project.FC2J.DataStore.Interfaces;
using Project.FC2J.DataStore.Internal.DataAccess;
using Project.FC2J.Models;
using Project.FC2J.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.User;

namespace Project.FC2J.DataStore
{
    public class AuthRepository : IAuthRepository
    {

        private readonly string _spInsertUser = "InsertUser";
        private readonly string _spUpdateUser = "UpdateUser";
        private readonly string _spRemoveUser = "RemoveUser";
        private readonly string _spGetUsers = "GetUsers";
        private readonly string _spGetUserByUserName = "GetUserByUserName";
        private readonly string _spGetModulesByRole = "GetModulesByRole";
        private User _user;

        public async Task<UserForLoginDto> GetHash(UserForLoginDto value)
        {
            var user = await GetUserByUserNameAsync(value.Username);

            if (user == null)
            {
                return null;
            }

            return new UserForLoginDto { PasswordHash = user.PasswordHash, PasswordSalt = user.PasswordSalt };
        }

        public async Task<User> Login(string userName, string password)
        {
            var user = await GetUserByUserNameAsync(userName);

            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        public async Task<User> Login(UserForLoginDto value)
        {
            var user = await GetUserByUserNameAsync(value.Username);

            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(user.PasswordHash, value.PasswordHash))
            {
                return null;
            }

            return user;
        }

        private bool VerifyPasswordHash(byte[] passwordHash1, byte[] passwordHash2)
        {
            for (int i = 0; i < passwordHash1.Length; i++)
            {
                if (passwordHash1[i] != passwordHash2[i])
                {
                    return false;
                }
            }
            return true;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            var sqlParameters = new[]
            {
                new SqlParameter("@UserName", userName)
            };

            var user = await _spGetUserByUserName.GetRecord<User>(sqlParameters);
            user.UserRole.Id = user.UserRoleId;
            user.UserRole.RoleName = user.UserRoles;
            user.UserModules = await GetModulesByRole(user.UserRoleId);
            return user;

        }

        private async Task<List<Module>> GetModulesByRole(int roleId)
        {
            var list = new List<Module>();
            var sqlParameters = new[]
            {
                new SqlParameter("@RoleId", roleId)
            };

            list = await _spGetModulesByRole.GetList<Module>(sqlParameters);

            return list;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            var sqlParameters = new[]
                {
                    new SqlParameter("@UserName", user.UserName),
                    new SqlParameter("@PasswordHash", SqlDbType.VarChar, 200)
                    {
                        Direction = ParameterDirection.Input,
                        Value = Convert.ToBase64String(user.PasswordHash)
                    },
                    new SqlParameter("@PasswordSalt", SqlDbType.VarChar, 200)
                    {
                        Direction = ParameterDirection.Input,
                        Value = Convert.ToBase64String(user.PasswordSalt)
                    }
                };

            await _spInsertUser.ExecuteNonQueryAsync(sqlParameters);

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string userName)
        {
            var user = await GetUserByUserNameAsync(userName);
            if (user == null)
            {
                return false;
            }
            return true;

        }

        public async Task<List<User>> GetList()
        {
            return await _spGetUsers.GetList<User>();
        }

        public async Task<User> Save(User value)
        {
            _user = value;

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(_user.PasswordX, out passwordHash, out passwordSalt);
            _user.PasswordHash = passwordHash;
            _user.PasswordSalt = passwordSalt;
            _user.Id = await _spInsertUser.GetRecordId(GetSqlParameters().ToArray());
            return _user;
        }

        private List<SqlParameter> GetSqlParameters()
        {
            var sqlParameters = new List<SqlParameter>();
            try
            {
                sqlParameters.Add(new SqlParameter("@UserName", _user.UserName.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@PasswordHash", SqlDbType.VarChar, 200)
                {
                    Direction = ParameterDirection.Input,
                    Value = Convert.ToBase64String(_user.PasswordHash)
                });
                sqlParameters.Add(new SqlParameter("@PasswordSalt", SqlDbType.VarChar, 200)
                {
                    Direction = ParameterDirection.Input,
                    Value = Convert.ToBase64String(_user.PasswordSalt)
                });

                sqlParameters.Add(new SqlParameter("@Primary", _user.Primary.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@Email", _user.Email.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@ContactNo", _user.ContactNo.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@LastName", _user.LastName.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@FirstName", _user.FirstName.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@MiddleName", _user.MiddleName.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@Address1", _user.Address1.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@Address2", _user.Address2.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@RoleId", _user.UserRole.Id));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return sqlParameters;

        }

        public async Task Update(User value)
        {
            _user = value;
            var sqlParams = GetSqlParameters();
            sqlParams.Add(new SqlParameter("@Id", _user.Id));
            sqlParams.Add(new SqlParameter("@Locked", SqlDbType.Bit)
            {
                Direction = ParameterDirection.Input,
                Value = _user.Locked
            });
            sqlParams.Add(new SqlParameter("@Tries", _user.Tries));

            await _spUpdateUser.ExecuteNonQueryAsync(sqlParams.ToArray());
        }

        public async Task Remove(long id)
        {
            var sqlParameters = new[] { new SqlParameter("@Id", id) };
            await _spRemoveUser.ExecuteNonQueryAsync(sqlParameters);
        }

    }
}
