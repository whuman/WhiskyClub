using System;
using System.Collections.Generic;
using System.Linq;
using WhiskyClub.DataAccess.Entities;

namespace WhiskyClub.DataAccess.Repositories
{
    public class WhiskyRepository : EntityFrameworkRepositoryBase, IWhiskyRepository
    {
        public Models.Whisky GetWhisky(int whiskyId)
        {
            var whisky = GetOne<Whisky, int>(whiskyId);

            return new Models.Whisky
                       {
                           WhiskyId = whisky.WhiskyId,
                           Name = whisky.Name,
                           Brand = whisky.Brand,
                           Age = whisky.Age,
                           Country = whisky.Country,
                           Region = whisky.Region,
                           Description = whisky.Description,
                           Price = whisky.Price,
                           Volume = whisky.Volume
                       };
        }

        public List<Models.Whisky> GetAllWhiskies()
        {
            var items = from whisky in GetAll<Whisky>()
                        select new Models.Whisky
                                   {
                                       WhiskyId = whisky.WhiskyId,
                                       Name = whisky.Name,
                                       Brand = whisky.Brand,
                                       Age = whisky.Age,
                                       Country = whisky.Country,
                                       Region = whisky.Region,
                                       Description = whisky.Description,
                                       Price = whisky.Price,
                                       Volume = whisky.Volume
                                   };

            return items.ToList();
        }

        public Models.Whisky InsertWhisky(string name, string brand, int age, string country, string region, string description, decimal? price, int? volume)
        {
            try
            {
                var whisky = new Whisky();
                whisky.Name = name;
                whisky.Brand = brand;
                whisky.Age = age;
                whisky.Country = country;
                whisky.Region = region;
                whisky.Description = description;
                whisky.Price = price;
                whisky.Volume = volume;
                whisky.InsertedDate = DateTime.Now;
                whisky.UpdatedDate = DateTime.Now;

                Insert(whisky);

                CommitChanges();

                return new Models.Whisky
                           {
                               WhiskyId = whisky.WhiskyId,
                               Name = whisky.Name,
                               Brand = whisky.Brand,
                               Age = whisky.Age,
                               Country = whisky.Country,
                               Region = whisky.Region,
                               Description = whisky.Description,
                               Price = whisky.Price,
                               Volume = whisky.Volume
                           };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool UpdateWhisky(int whiskyId, string name, string brand, int age, string country, string region, string description, decimal? price, int? volume)
        {
            try
            {
                var whisky = GetOne<Whisky, int>(whiskyId);
                whisky.Name = name;
                whisky.Brand = brand;
                whisky.Age = age;
                whisky.Country = country;
                whisky.Region = region;
                whisky.Description = description;
                whisky.Price = price;
                whisky.Volume = volume;
                whisky.UpdatedDate = DateTime.Now;

                Update(whisky);

                CommitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteWhisky(int whiskyId)
        {
            try
            {
                var whisky = new Whisky();
                whisky.WhiskyId = whiskyId;

                Delete(whisky);

                CommitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public bool UpdateImage(int whiskyId, byte[] image)
        {
            try
            {
                var whisky = GetOne<Whisky, int>(whiskyId);
                whisky.Image = image;
                whisky.UpdatedDate = DateTime.Now;

                Update(whisky);

                CommitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public List<Models.Whisky> GetWhiskiesForEvent(int eventId)
        {
            var items = from eventWhisky in GetAll<EventWhisky>()
                        where eventWhisky.EventId == eventId
                        select new Models.Whisky
                        {
                            WhiskyId = eventWhisky.Whisky.WhiskyId,
                            Name = eventWhisky.Whisky.Name,
                            Brand = eventWhisky.Whisky.Brand,
                            Age = eventWhisky.Whisky.Age,
                            Country = eventWhisky.Whisky.Country,
                            Region = eventWhisky.Whisky.Region,
                            Description = eventWhisky.Whisky.Description,
                            Price = eventWhisky.Whisky.Price,
                            Volume = eventWhisky.Whisky.Volume
                        };

            return items.ToList();
        }
    }
}
