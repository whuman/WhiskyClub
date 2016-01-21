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

        public List<Models.Whisky> GetWhiskiesForEvent(int eventId)
        {
            var items = from ew in GetAll<EventWhisky>()
                        where ew.EventId == eventId
                        select new Models.Whisky
                        {
                            WhiskyId = ew.Whisky.WhiskyId,
                            Name = ew.Whisky.Name,
                            Brand = ew.Whisky.Brand,
                            Age = ew.Whisky.Age,
                            Country = ew.Whisky.Country,
                            Region = ew.Whisky.Region,
                            Description = ew.Whisky.Description,
                            Price = ew.Whisky.Price,
                            Volume = ew.Whisky.Volume
                        };

            return items.ToList();
        }

        public Models.Whisky InsertWhisky(string name, string brand, int? age, string country, string region, string description, decimal? price, int? volume)
        {
            try
            {
                var whisky = new Whisky
                {
                    Name = name,
                    Brand = brand,
                    Age = age,
                    Country = country,
                    Region = region,
                    Description = description,
                    Price = price,
                    Volume = volume,
                    InsertedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

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

        public bool UpdateWhisky(int whiskyId, string name, string brand, int? age, string country, string region, string description, decimal? price, int? volume)
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
                var whisky = new Whisky { WhiskyId = whiskyId };

                Delete(whisky);

                CommitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public byte[] GetWhiskyImage(int whiskyId)
        {
            var whisky = GetOne<Whisky, int>(whiskyId);

            return whisky.Image;
        }

        public bool UpdateWhiskyImage(int whiskyId, byte[] image)
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

        public bool AddEventWhisky(int eventId, int whiskyId)
        {
            try
            {
                var eventWhisky = new EventWhisky
                {
                    EventId = eventId,
                    WhiskyId = whiskyId,
                    InsertedDate = DateTime.Now
                };

                Insert(eventWhisky);

                CommitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveEventWhisky(int eventId, int whiskyId)
        {
            try
            {
                try
                {
                    // As we do not have the EventWhiskyId (PK), we need to load the entity from the datastore before deleting
                    var eventWhisky = GetAll<EventWhisky>().FirstOrDefault(ew => ew.EventId == eventId && ew.WhiskyId == whiskyId);

                    // Yes this will break if we do not find the eventWhisky...
                    Delete(eventWhisky);

                    CommitChanges();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
