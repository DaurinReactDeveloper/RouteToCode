﻿using Microsoft.Extensions.Logging;
using RouteToCode.Domain.Entities;
using RouteToCode.Infrastructure.Core;
using RouteToCode.Infrastructure.Exceptions;
using RouteToCode.Infrastructure.Extensions;
using RouteToCode.Infrastructure.Interfaces;
using RouteToCode.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteToCode.Infrastructure.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {

        private readonly DBBLOGContext dBBLOGContext;
        private readonly ILogger<UserRepository> logger;

        public UserRepository(ILogger<UserRepository> logger,
            DBBLOGContext DbContext) : base(DbContext)
        {
            this.logger = logger;
            this.dBBLOGContext = DbContext;
        }

        //Login
        public UserModel GetUser(string Name, string Password)
        {

            UserModel user = new UserModel();

            try
            {

                user = (from us in this.dBBLOGContext.Users
                        where us.Name == Name &&
                              us.Password == Password
                        select new UserModel()
                        {
                            Name = us.Name,
                            Password = us.Password,
                        }

                    ).FirstOrDefault();

            }
            catch (Exception ex)
            {
                this.logger.LogError($"Ha Ocurrido un Error Obteniendo el Usuario",ex.ToString());
            }

            return user;
        }

        //GetID
        public UserModel GetUserById(int Id)
        {

            UserModel user = new UserModel();

            try
            {

                user = this.GetById(Id).UserModelConverter();

            }
            catch (Exception ex)
            {

                this.logger.LogError("Error obteniendo el Usuario", ex.ToString());
            }

            return user;

        }

        //Add
        public override void Add(User entety)
        {
            base.Add(entety);
            SaveChanged();
        }

        //Update
        public override void Update(User entety)
        {

            try
            {

                User user = this.GetById(entety.UserId);

                if (user is null)
                {

                    throw new UserExceptions("Ha Ocurrido Un Error Obteniendo el id del Usuario");
                }

                user.Name = entety.Name;
                user.Password = entety.Password;
                user.Email = entety.Email;
                user.Address = entety.Address;

                base.Update(user);
                base.SaveChanged();

            }
            catch (Exception ex)
            {

                this.logger.LogError($"Ocurrió un error actualizando el usuario: {ex.Message}", ex.ToString());
            }



        }

        //Remove
        public override void Remove(User entety)
        {
            try
            {

                User user = this.GetById(entety.UserId);

                if (user is null)
                {

                    throw new UserExceptions("Ha Ocurrido Un Error Obteniendo el id del Usuario");
                }

                base.Remove(user);
                base.SaveChanged();

            }
            catch (Exception ex)
            {
                this.logger.LogError($"Ocurrió un error Eliminando el Usuario: {ex.Message}", ex.ToString());
            }
        }

    }
}
